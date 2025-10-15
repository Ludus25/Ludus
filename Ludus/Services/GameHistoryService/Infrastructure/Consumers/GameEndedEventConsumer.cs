using MassTransit;
using MediatR;

using Commands;
using Interfaces;
using Common.Entities;
using Common.Dto;


namespace Consumers
{
    public class GameEndedEventConsumer : IConsumer<GameEndedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GameEndedEventConsumer> _logger;

        public GameEndedEventConsumer(IMediator mediator, ILogger<GameEndedEventConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GameEndedEvent> context)
        {
            GameEndedEvent dto = context.Message;

            _logger.LogInformation("Received GameEndedEvent for match {MatchId}", dto.MatchId);

            var history = new GameHistory
            {
                MatchId = dto.MatchId,
                PlayerUserIds = dto.PlayerUserIds,
                StartedAt = dto.StartedAt,
                EndedAt = dto.EndedAt,
                MoveHistory = dto.MoveHistory,
                WinnerUserId = dto.WinnerUserId,
                PlayerEmails = dto.PlayerEmails
            };
            await _mediator.Send(new SaveGameCommand(history));
        }
    }
}

