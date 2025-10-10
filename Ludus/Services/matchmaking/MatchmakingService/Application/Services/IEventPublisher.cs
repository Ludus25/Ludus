namespace MatchmakingService.Application.Services
{
    public interface IEventPublisher
    {
        void PublishMatchCreated(object payload);
    }
}
