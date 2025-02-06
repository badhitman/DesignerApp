////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.Metrics;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text.Json;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using SharedLib;

namespace RemoteCallLib;

/// <summary>
/// Удалённый вызов команд (RabbitMq client)
/// </summary>
public class RabbitClient : IRabbitClient
{
    readonly RabbitMQConfigModel RabbitConfigRepo;
    readonly ConnectionFactory factory;
    readonly ILogger<RabbitClient> loggerRepo;

    readonly string AppName;

    static Dictionary<string, object>? ResponseQueueArguments;
    /// <inheritdoc/>
    public static readonly JsonSerializerOptions SerializerOptions = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles, WriteIndented = true };

    /// <summary>
    /// Удалённый вызов команд (RabbitMq client)
    /// </summary>
    public RabbitClient(IOptions<RabbitMQConfigModel> rabbitConf, ILogger<RabbitClient> _loggerRepo, string appName)
    {
        AppName = appName;
        loggerRepo = _loggerRepo;
        RabbitConfigRepo = rabbitConf.Value;
        ResponseQueueArguments ??= new()
        {
            { "x-message-ttl", rabbitConf.Value.RemoteCallTimeoutMs },
            { "x-expires", rabbitConf.Value.RemoteCallTimeoutMs }
        };
        factory = new()
        {
            ClientProvidedName = RabbitConfigRepo.ClientProvidedName,
            HostName = RabbitConfigRepo.HostName,
            Port = RabbitConfigRepo.Port,
            UserName = RabbitConfigRepo.UserName,
            Password = RabbitConfigRepo.Password
        };
    }

    /// <inheritdoc/>
    public Task<T?> MqRemoteCall<T>(string queue, object? request = null, bool waitResponse = true)
    {
        // Custom ActivitySource for the application
        ActivitySource greeterActivitySource = new($"OTel.{AppName}");
        // Create a new Activity scoped to the method
        using Activity? activity = greeterActivitySource.StartActivity($"OTel.{queue}");

        Meter greeterMeter = new($"OTel.{AppName}", "1.0.0");
        Counter<long> countGreetings = greeterMeter.CreateCounter<long>(GlobalStaticConstants.Routes.DURATION_ACTION_NAME, description: "Длительность в мс.");

        activity?.Start();

        string response_topic = waitResponse ? $"{RabbitConfigRepo.QueueMqNamePrefixForResponse}{queue}_{Guid.NewGuid()}" : "";
        activity?.SetTag(nameof(response_topic), response_topic);

        using IConnection _connection = factory.CreateConnection(); ;
        using IModel _channel = _connection.CreateModel();

        IBasicProperties? properties = _channel.CreateBasicProperties();
        if (waitResponse)
        {
            properties.ReplyTo = response_topic;
            _channel.QueueDeclare(queue: response_topic, durable: false, exclusive: false, autoDelete: true, arguments: ResponseQueueArguments);
        }

        Stopwatch stopwatch = new();
        EventingBasicConsumer consumer = new(_channel);

        CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;
        ManualResetEventSlim mres = new(false);
        string _msg;
        TResponseMQModel<T?>? res_io = null;
        void MessageReceivedEvent(object? sender, BasicDeliverEventArgs e)
        {
            string msg;
            consumer.Received -= MessageReceivedEvent;
            string content = Encoding.UTF8.GetString(e.Body.ToArray());

            if (!content.Contains(GlobalStaticConstants.Routes.PASSWORD_CONTROLLER_NAME, StringComparison.OrdinalIgnoreCase))
                activity?.SetBaggage(nameof(content), content);
            else
                activity?.SetBaggage(nameof(content), $"MUTE: `{GlobalStaticConstants.Routes.PASSWORD_CONTROLLER_NAME}` - contains");

            try
            {
                res_io = JsonConvert.DeserializeObject<TResponseMQModel<T?>>(content, GlobalStaticConstants.JsonSerializerSettings)
                    ?? throw new Exception("parse error {0CBCCD44-63C8-4E93-8349-11A8BE63B235}");

                if (!res_io.Success())
                    loggerRepo.LogError(res_io.Message());

                countGreetings.Add(res_io.Duration().Milliseconds);
            }
            catch (Exception ex)
            {
                msg = $"error deserialisation: {content}.\n\nerror ";
                loggerRepo.LogError(ex, msg);
            }

            try
            {
                _channel.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                msg = "exception basic ask. error {A62029D4-1A23-461D-99AD-349C6B7500A8}";
                loggerRepo.LogError(ex, msg);
            }

            stopwatch.Stop();
            cts.Cancel();
            cts.Dispose();
        }

        consumer.Received += MessageReceivedEvent;

        if (waitResponse)
        {
            try
            {
                _channel.BasicConsume(response_topic, false, consumer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        activity?.Stop();
        _channel!.QueueDeclare(queue: queue,
                      durable: true,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);

#if DEBUG
        string request_payload_json = "";
        try
        {
            request_payload_json = JsonConvert.SerializeObject(request, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings);
        }
        catch (Exception ex)
        {
            loggerRepo.LogError(ex, $"Ошибка сериализации объекта [{request?.GetType().Name}]: {request}");
        }

        byte[] body = request is null ? [] : Encoding.UTF8.GetBytes(request_payload_json);
#else
        byte[] body = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(request, SerializerOptions);
#endif

        _channel!.BasicPublish(exchange: "",
                       routingKey: queue,
                       basicProperties: properties,
                       body: body);

        if (waitResponse)
        {
            stopwatch.Start();
            _ = Task.Run(async () =>
            {
                await Task.Delay(RabbitConfigRepo.RemoteCallTimeoutMs);
                cts.Cancel();
            });
            try
            {
                mres.Wait(token);
            }
            catch (OperationCanceledException)
            {
                loggerRepo.LogDebug($"response for {response_topic}");
            }
            catch (Exception ex)
            {
                _msg = "exception Wait response. error {8B621451-2214-467F-B8E9-906DD866662C}";
                loggerRepo.LogError(ex, _msg);
                stopwatch.Stop();
            }

            if (stopwatch.IsRunning)
            {
                _msg = $"Elapsed for `{queue}` -> `{response_topic}`: {stopwatch.Elapsed} > {TimeSpan.FromMilliseconds(RabbitConfigRepo.RemoteCallTimeoutMs)}";
                loggerRepo.LogError(_msg);
                stopwatch.Stop();
            }
            else
                loggerRepo.LogDebug($"Elapsed [{queue}] -> [{response_topic}]: {stopwatch.Elapsed} > {TimeSpan.FromMilliseconds(RabbitConfigRepo.RemoteCallTimeoutMs)}");
        }
        else
            return Task.FromResult(default(T));

        if ((typeof(T) != typeof(object) && (res_io is null || res_io.Response is null)))
        {
            _msg = $"Response MQ/IO is null [{queue}] -> [{response_topic}]: {stopwatch.Elapsed} > {TimeSpan.FromMilliseconds(RabbitConfigRepo.RemoteCallTimeoutMs)}";
            loggerRepo.LogError(_msg);
            return Task.FromResult(default(T));
        }
        else if (res_io is null)
            return Task.FromResult(default(T));

        return Task.FromResult(res_io.Response);
    }
}