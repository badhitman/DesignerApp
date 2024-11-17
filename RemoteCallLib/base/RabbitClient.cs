////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

    static Dictionary<string, object>? ResponseQueueArguments;
    public static readonly JsonSerializerOptions SerializerOptions = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles, WriteIndented = true };

    /// <summary>
    /// Удалённый вызов команд (RabbitMq client)
    /// </summary>
    public RabbitClient(IOptions<RabbitMQConfigModel> rabbitConf, ILogger<RabbitClient> _loggerRepo)
    {
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
    public Task<TResponseModel<T>> MqRemoteCall<T>(string queue, object? request = null)
    {
        string response_topic = $"{RabbitConfigRepo.QueueMqNamePrefixForResponse}{queue}_{Guid.NewGuid()}";

        using IConnection _connection = factory.CreateConnection(); ;
        using IModel _channel = _connection.CreateModel();

        IBasicProperties? properties = _channel.CreateBasicProperties();
        properties.ReplyTo = response_topic;
        _channel.QueueDeclare(queue: response_topic, durable: false, exclusive: false, autoDelete: true, arguments: ResponseQueueArguments);

        TResponseModel<T> res = new();
        Stopwatch stopwatch = new();
        EventingBasicConsumer consumer = new(_channel);

        CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;
        ManualResetEventSlim mres = new(false);
        string _msg;
        void MessageReceivedEvent(object? sender, BasicDeliverEventArgs e)
        {
            string msg;
            consumer.Received -= MessageReceivedEvent;
            string content = Encoding.UTF8.GetString(e.Body.ToArray());

            try
            {
                res = JsonConvert.DeserializeObject<TResponseModel<T>>(content, GlobalStaticConstants.JsonSerializerSettings)
                    ?? throw new Exception("parse error {0CBCCD44-63C8-4E93-8349-11A8BE63B235}");
            }
            catch (Exception ex)
            {
                msg = $"error deserialisation: {content}.\n\nerror ";
                loggerRepo.LogError(ex, msg);
                res!.AddError(msg);
                res.Messages.InjectException(ex);
            }

            try
            {
                _channel.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                msg = "exception basic ask. error {A62029D4-1A23-461D-99AD-349C6B7500A8}";
                loggerRepo.LogError(ex, msg);
                res.Messages.InjectException(ex);
                res.AddError(msg);
            }

            stopwatch.Stop();
            cts.Cancel();
            cts.Dispose();
        }

        consumer.Received += MessageReceivedEvent;
        _channel.BasicConsume(response_topic, false, consumer);

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
            //res.AddInfo("Обработка запроса прервана: OperationCanceledException");
            loggerRepo.LogDebug($"response for {response_topic}");
        }
        catch (Exception ex)
        {
            res.Messages.InjectException(ex);
            stopwatch.Stop();
        }

        if (stopwatch.IsRunning)
        {
            _msg = $"Elapsed for `{queue}` -> `{response_topic}`: {stopwatch.Elapsed} > {TimeSpan.FromMilliseconds(RabbitConfigRepo.RemoteCallTimeoutMs)}";
            loggerRepo.LogError(_msg);
            stopwatch.Stop();
            res.AddInfo(_msg);
        }
        else
            loggerRepo.LogDebug($"Elapsed [{queue}] -> [{response_topic}]: {stopwatch.Elapsed} > {TimeSpan.FromMilliseconds(RabbitConfigRepo.RemoteCallTimeoutMs)}");

        return Task.FromResult(res);
    }
}