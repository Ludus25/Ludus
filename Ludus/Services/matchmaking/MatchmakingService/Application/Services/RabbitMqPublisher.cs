using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MatchmakingService.Application.Services
{
    public class RabbitMqPublisher : IEventPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchange = "matchmaking";

        public RabbitMqPublisher(string hostName)
        {
            var factory = new ConnectionFactory() { HostName = hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Fanout, durable: true);
        }

        public void PublishMatchCreated(object payload)
        {
            try
            {
                var json = JsonSerializer.Serialize(payload);
                var body = Encoding.UTF8.GetBytes(json);
                _channel.BasicPublish(exchange: _exchange, routingKey: "", basicProperties: null, body: body);
                Console.WriteLine($"[RABBITMQ] Published match created event: {json}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RABBITMQ-ERROR] Failed to publish event: {ex.Message}");
                Console.WriteLine($"[FALLBACK] Match created event: {JsonSerializer.Serialize(payload)}");
            }
        }

        public void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RABBITMQ-ERROR] Error during disposal: {ex.Message}");
            }
            finally
            {
                _channel?.Dispose();
                _connection?.Dispose();
            }
        }
    }
}