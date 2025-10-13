using MatchmakingService.Application.Commands;
using MatchmakingService.Application.Services;
using MatchmakingService.Infrastructure.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI for repository and event publisher
builder.Services.AddSingleton<IMatchRepository, InMemoryMatchRepository>();

// If you have RabbitMQ locally, set host from configuration, else use NullEventPublisher
var rabbitHost = builder.Configuration["RABBIT_HOST"];
if (!string.IsNullOrEmpty(rabbitHost))
{
    builder.Services.AddSingleton<IEventPublisher>(sp => new RabbitMqPublisher(rabbitHost));
}
else
{
    // simple console publisher
    builder.Services.AddSingleton<IEventPublisher>(sp => new ConsoleEventPublisher());
}

// Register command handler (depends on repo & publisher)
builder.Services.AddScoped<JoinCommandHandler>();

// Optionally register gRPC client wrapper (address from config)
var userGrpcAddr = builder.Configuration["USER_GRPC_ADDRESS"];
if (!string.IsNullOrEmpty(userGrpcAddr))
{
    builder.Services.AddSingleton(new UserGrpcClient(userGrpcAddr));
}

var gameGrpcAddr = builder.Configuration["GAME_GRPC_ADDRESS"];
if (!string.IsNullOrEmpty(gameGrpcAddr))
{
    builder.Services.AddSingleton(new GameGrpcClient(gameGrpcAddr));
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();

// --- small ConsoleEventPublisher fallback (in same file or separate)
public class ConsoleEventPublisher : IEventPublisher
{
    public void PublishMatchCreated(object payload)
    {
        Console.WriteLine("[EVENT-PUBLISHED] " + System.Text.Json.JsonSerializer.Serialize(payload));
    }
}
