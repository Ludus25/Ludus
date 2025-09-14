# Ludus

## GameHistory Service
    - Responsible for storing completed matches and chat logs for those matches.
    - Listens for two events: GameEndedEvent and ChatLogEvent, over RabbitMQ.
    - Consumed only by the frontend, via Http APIs.
    - Stores game metadata, moves, chat logs.