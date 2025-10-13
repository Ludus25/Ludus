using MassTransit;
using MediatR;

using Commands;
using Interfaces;
using Common.Dto;


namespace Consumers
{
    public class ChatLogEventConsumer : IConsumer<ChatLogEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ChatLogEventConsumer> _logger;

        public ChatLogEventConsumer(IMediator mediator, ILogger<ChatLogEventConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ChatLogEvent> context)
        {
            ChatLogEvent dto = context.Message;

            _logger.LogInformation("Received ChatLogEvent for match {MatchId}", dto.MatchId);

            await _mediator.Send(new AppendChatCommand(dto.MatchId, dto.Messages));
        }
    }
}