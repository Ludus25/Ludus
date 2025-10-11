using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using MassTransit;

using Interfaces;
using Messaging;
using System.Text;
using System.Text.Json;
using Common.Dto;


namespace Consumers
{
    public class ChatLogEventConsumer : IConsumer<ChatLogEvent>
    {
        private readonly IGameHistoryService _historyService;
        private readonly ILogger<ChatLogEventConsumer> _logger;

        public ChatLogEventConsumer(IGameHistoryService historyService, ILogger<ChatLogEventConsumer> logger)
        {
            _historyService = historyService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ChatLogEvent> context)
        {
            ChatLogEvent dto = context.Message;

            _logger.LogInformation("Received ChatLogEvent for match {MatchId}", dto.MatchId);

            await _historyService.AppendChatAsync(dto.MatchId, dto.Messages);
        }
    }
}