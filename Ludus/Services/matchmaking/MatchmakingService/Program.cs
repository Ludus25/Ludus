using MatchmakingService.Application.Commands;
using MatchmakingService.Application.Services;
using MatchmakingService.Infrastructure.Grpc;
using MatchmakingService.Hubs;
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

// SignalR
builder.Services.AddSignalR();

// DI for repository and event publisher
builder.Services.AddSingleton<IMatchRepository, InMemoryMatchRepository>();

// RabbitMQ publisher
var rabbitHost = builder.Configuration["RABBIT_HOST"];
if (!string.IsNullOrEmpty(rabbitHost))
{
    builder.Services.AddSingleton<IEventPublisher>(sp => new RabbitMqPublisher(rabbitHost));
}
else
{
    builder.Services.AddSingleton<IEventPublisher>(sp => new ConsoleEventPublisher());
}

// Register command handler
builder.Services.AddScoped<JoinCommandHandler>();

// ✅ SAMO GameGrpcClient (UserGrpcClient uklonjen)
builder.Services.AddSingleton<GameGrpcClient>();

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
app.MapHub<MatchmakingHub>("/matchmakingHub");
app.Run();

public class ConsoleEventPublisher : IEventPublisher
{
    public void PublishMatchCreated(object payload)
    {
        Console.WriteLine("[EVENT-PUBLISHED] " + System.Text.Json.JsonSerializer.Serialize(payload));
    }
}