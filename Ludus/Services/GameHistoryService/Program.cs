using Consumers;
using Data;
using Interfaces;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // RabbitMQ configuration
        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<GameEndedEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration["RabbitMQ:HostAddress"]);

                string gameEndedQueueName = builder.Configuration["RabbitMQ:GameEndedQueue"];

                cfg.ReceiveEndpoint(gameEndedQueueName, e =>
                {
                    e.ConfigureConsumer<GameEndedEventConsumer>(context);
                    e.SetQueueArgument("x-dead-letter-exchange", $"{gameEndedQueueName}-dlx");
                    e.SetQueueArgument("x-dead-letter-routing-key", $"{gameEndedQueueName}.dlq");
                });

                cfg.ReceiveEndpoint($"{gameEndedQueueName}.dlq", e => e.Bind($"{gameEndedQueueName}-dlx", x => x.RoutingKey = $"{gameEndedQueueName}.dlq"));
            });
        });

        // DbContext
        builder.Services.AddDbContext<GameHistoryDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnectionString")));

        // Add services to the container.
        builder.Services.AddScoped<IGameHistoryRepository, GameHistoryRepository>();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler("/error");
        app.Map("/error", (HttpContext http) =>
            Results.Problem(title: "An unexpected error occurred.",
                            statusCode: StatusCodes.Status500InternalServerError));

        app.UseRouting();
        
        app.UseAuthorization();

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GameHistoryDbContext>();
            try
            {
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EF MIGRATE] Failed to run migrations: {ex}");
            }
        }

        app.Run();
    }
}
