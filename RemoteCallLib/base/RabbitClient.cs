using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedLib;
using System.Diagnostics;
using System.Text;

namespace RemoteCallLib;

/// <summary>
/// RabbitMq client
/// </summary>
public class RabbitClient : IRabbitClient
{
    readonly RabbitMQConfigModel RabbitConfigRepo;
    readonly ConnectionFactory factory;
    readonly ILogger<RabbitClient> loggerRepo;

    static Dictionary<string, object>? ResponseQueueArguments;

    IBasicProperties? properties;
    /// <summary>
    /// RabbitMq client
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
    public Task<TResponseModel<T?>> MqRemoteCall<T>(string queue, object? request = null)
    {
        string response_topic = $"{RabbitConfigRepo.QueueMqNamePrefixForResponse}-{queue}_{Guid.NewGuid()}";

        using IConnection _connection = factory.CreateConnection(); ;
        using IModel _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: response_topic, durable: false, exclusive: false, autoDelete: true, arguments: ResponseQueueArguments);
        properties = _channel.CreateBasicProperties();
        properties.ReplyTo = response_topic;

        TResponseModel<T?> res = new();
        Stopwatch stopwatch = new();
        EventingBasicConsumer consumer = new(_channel);

        CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;
        ManualResetEventSlim mres = new(false);

        void MessageReceivedEvent(object? sender, BasicDeliverEventArgs e)
        {
            string msg;
            consumer.Received -= MessageReceivedEvent;
            string content = Encoding.UTF8.GetString(e.Body.ToArray());
            try
            {
                res = JsonConvert.DeserializeObject<TResponseModel<T?>>(content)
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

        byte[] body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
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
            loggerRepo.LogDebug($"response for {response_topic}");
        }
        catch (Exception ex)
        {
            res.Messages.InjectException(ex);
            stopwatch.Stop();
        }

        if (stopwatch.IsRunning)
        {
            stopwatch.Stop();
            res.AddError($"Прервано по таймауту: {stopwatch.Elapsed} > {TimeSpan.FromMilliseconds(RabbitConfigRepo.RemoteCallTimeoutMs)}");
        }

        return Task.FromResult(res);
    }
}