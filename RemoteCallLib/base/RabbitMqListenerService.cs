////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using SharedLib;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace RemoteCallLib;

/// <inheritdoc/>
public class RabbitMqListenerService<TQueue, TRequest, TResponse>
    : BackgroundService
    where TQueue : IResponseReceive<TRequest?, TResponse>
{
    readonly IConnection _connection;
    readonly IModel _channel;
    readonly IResponseReceive<TRequest?, TResponse> receiveService;
    readonly ConnectionFactory factory;

    static Dictionary<string, object>? ResponseQueueArguments;

    Type? _queueType;
    /// <summary>
    /// Queue type
    /// </summary>
    public Type QueueType { get { _queueType ??= typeof(TQueue); return _queueType; } }

    string? _queueName;
    /// <summary>
    /// Имя очереди MQ
    /// </summary>
    public string QueueName { get { _queueName ??= TQueue.QueueName; return _queueName; } }

    /// <inheritdoc/>
    public RabbitMqListenerService(IOptions<RabbitMQConfigModel> rabbitConf, IServiceProvider servicesProvider)
    {
        ResponseQueueArguments ??= new()
        {
            { "x-message-ttl", rabbitConf.Value.RemoteCallTimeoutMs },
            { "x-expires", rabbitConf.Value.RemoteCallTimeoutMs }
        };

        // Console.WriteLine(JsonConvert.SerializeObject(rabbitConf.Value));

        using (IServiceScope scope = servicesProvider.CreateScope())
        {
            receiveService = scope.ServiceProvider.GetServices<IResponseReceive<TRequest?, TResponse>>().First(o => o.GetType() == QueueType);
        }

        factory = new()
        {
            ClientProvidedName = rabbitConf.Value.ClientProvidedName,
            HostName = rabbitConf.Value.HostName,
            UserName = rabbitConf.Value.UserName,
            Password = rabbitConf.Value.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    /// <inheritdoc/>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        EventingBasicConsumer consumer = new(_channel);
        consumer.Received += async (ch, ea) =>
        {
            string content = Encoding.UTF8.GetString(ea.Body.ToArray()).Trim();

            TResponseModel<TResponse?> result_handler;
            try
            {
                TRequest? sr = content.Equals("null", StringComparison.OrdinalIgnoreCase)
                ? default
                : JsonConvert.DeserializeObject<TRequest?>(content);

                result_handler = await receiveService.ResponseHandleAction(sr);
            }
            catch (Exception ex)
            {
                result_handler = new();
                result_handler.Messages.InjectException(ex);
            }

            if (!string.IsNullOrWhiteSpace(ea.BasicProperties.ReplyTo))
                try
                {
                    _channel.BasicPublish(exchange: "", routingKey: ea.BasicProperties.ReplyTo, basicProperties: null, body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result_handler, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)));
                }
                finally
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
        };

        _channel.BasicConsume(QueueName, false, consumer);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}