using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Interfaces;
using Messaging;
using System.Text;
using System.Text.Json;
using Dto;
using Entities;
using System.Threading.Tasks;

namespace Consumers
{
    public class ChatLogEventConsumer : BackgroundService
    {
        private readonly RabbitMQSettings _settings;
        private readonly IGameHistoryRepository _repo;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly int _maxRetries = 5;
        private readonly int _retryDelayMs = 30000; // 30s

        public ChatLogEventConsumer(IOptions<RabbitMQSettings> options, IGameHistoryRepository repo)
        {
            _settings = options.Value;
            _repo = repo;

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;

            var retryArgs = new System.Collections.Generic.Dictionary<string, object>
            {
                {"x-dead-letter-exchange", string.Empty},
                {"x-dead-letter-routing-key", _settings.ChatLogQueue},
                {"x-message-ttl", (long)_retryDelayMs}
            };

            _channel.QueueDeclareAsync(queue: _settings.ChatLogQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclareAsync(queue: _settings.ChatLogQueue + "_retry", durable: true, exclusive: false, autoDelete: false, arguments: retryArgs!);
            _channel.QueueDeclareAsync(queue: _settings.ChatLogQueue + "_dead", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
            {
                return Task.CompletedTask;
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                try
                {
                    var dto = JsonSerializer.Deserialize<ChatLogEventDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (dto == null)
                    {
                        await PublishToDead(ea.BasicProperties, body);
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                        return;
                    }

                    await _repo.AppendChatMessageAsync(dto.MatchId, dto.Messages);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception)
                {
                    int retries = 0;
                    try
                    {
                        if (ea.BasicProperties?.Headers != null && ea.BasicProperties.Headers.ContainsKey("x-retries"))
                        {
                            var headerVal = ea.BasicProperties.Headers["x-retries"];
                            if (headerVal is byte[] b) retries = int.Parse(Encoding.UTF8.GetString(b));
                            else retries = Convert.ToInt32(headerVal);
                        }
                    }
                    catch { retries = 0; }

                    if (retries >= _maxRetries)
                    {
                        await PublishToDead(ea.BasicProperties, body);
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    else
                    {
                        await PublishToRetry(ea.BasicProperties, body, retries + 1);
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                }
            };

            _channel.BasicConsumeAsync(queue: _settings.ChatLogQueue, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        private async Task PublishToRetry(IReadOnlyBasicProperties? props, byte[] body, int newRetries)
        {
            var properties = new BasicProperties
            {
                DeliveryMode = (DeliveryModes)2,
                Headers = props?.Headers ?? new System.Collections.Generic.Dictionary<string, object?>()
            };
            properties.Headers["x-retries"] = Encoding.UTF8.GetBytes(newRetries.ToString());

            ValueTask task = _channel!.BasicPublishAsync(exchange: string.Empty, routingKey: _settings.ChatLogQueue + "_retry", true, basicProperties: properties, body: body);
            await task;
        }

        private async Task PublishToDead(IReadOnlyBasicProperties? props, byte[] body)
        {
            var properties = new BasicProperties
            {
                DeliveryMode = (DeliveryModes)2,
                Headers = props?.Headers ?? new System.Collections.Generic.Dictionary<string, object?>()
            };

            ValueTask task = _channel!.BasicPublishAsync(exchange: string.Empty, routingKey: _settings.ChatLogQueue + "_dead", true, basicProperties: properties, body: body);
            await task;
        }

        public override void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
            base.Dispose();
        }
    }
}

