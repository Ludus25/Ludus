# Ludus

## GameHistory Service

This service consumes game-ended and chat-log events from RabbitMQ and persists game history + chat into Postgres.

### Quick start

- Ensure .Net 8 SDK is installed.
- Ensure PostreSQL running and reachable.
- Ensure RabbitMQ running and reachable.
- Configure 'appsettings.Development.json' connection string and RabbitMQ settings.
- Run dotnet ef database update --context GameHistoryDbContext

### Events expected
- 'game_ended': JSON payload matching 'GameEndedEventDto'
- 'chat_log': JSON payload matching 'ChatLogEventDto'

Clients (GameService, ChatService) should publish these messages to RabbitMQ queues.