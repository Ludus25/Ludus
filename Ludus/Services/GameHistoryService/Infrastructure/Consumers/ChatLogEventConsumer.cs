using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Interfaces;
using Messaging;
using System.Text;
using System.Text.Json;
using Dto;
using Entities;

namespace Consumers
{
    public class ChatLogEventConsumer : BackgroundService
    {
        private readonly RabbitMQSettings _settings;
        private readonly IGameHistoryRepository _repo;
        private IConnection? _connection;
        private IChannel? _channel;

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
            _channel.QueueDeclareAsync(queue: _settings.ChatLogQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
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
                    if (dto != null)
                    {
                        await _repo.AppendChatMessageAsync(dto.MatchId, dto.Messages);
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    else
                    {
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                    }
                }
                catch
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };

            _channel.BasicConsumeAsync(queue: _settings.ChatLogQueue, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
            base.Dispose();
        }
    }

}
