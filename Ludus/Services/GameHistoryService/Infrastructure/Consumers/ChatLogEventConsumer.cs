public class ChatLogEventConsumer : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Setup RabbitMQ consumer for ChatLogEvent
        // Deserialize event and store/update in DB
        return Task.CompletedTask;
    }
}
