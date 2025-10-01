using Microsoft.EntityFrameworkCore;

using Messaging;
using Data;
using Interfaces;
using Services;
using Consumers;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // RabbitMQ configuration
        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

        // DbContext
        builder.Services.AddDbContext<GameHistoryDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnectionString")));

        // Add services to the container.
        builder.Services.AddScoped<IGameHistoryRepository, GameHistoryRepository>();
        builder.Services.AddScoped<IGameHistoryService, GameHistoryService>();

        builder.Services.AddHostedService<GameEndedEventConsumer>();
        builder.Services.AddHostedService<ChatLogEventConsumer>();


        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GameHistoryDbContext>();
            db.Database.Migrate();
        }

        app.Run();
    }
}