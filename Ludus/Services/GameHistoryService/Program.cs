using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

        // JWT settings
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var jwtKey = jwtSection.GetValue<string>("Key");
        if (string.IsNullOrEmpty(jwtKey))
        {
            Console.WriteLine("Warning: JWT Key not configured. API will run without authentication if no key is provided.");
        }
        else
        {
            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = !string.IsNullOrEmpty(jwtSection.GetValue<string>("Issuer")),
                    ValidIssuer = jwtSection.GetValue<string>("Issuer"),
                    ValidateAudience = !string.IsNullOrEmpty(jwtSection.GetValue<string>("Audience")),
                    ValidAudience = jwtSection.GetValue<string>("Audience"),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateLifetime = true
                };
            });
        }

        // DbContext
        builder.Services.AddDbContext<GameHistoryDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnectionString")));

        // Add services to the container.
        builder.Services.AddScoped<IGameHistoryRepository, GameHistoryRepository>();
        builder.Services.AddScoped<IGameHistoryService, GameHistoryService>();

        // Hosted consumers for RabbitMQ
        builder.Services.AddHostedService<GameEndedEventConsumer>();
        builder.Services.AddHostedService<ChatLogEventConsumer>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        if (!string.IsNullOrEmpty(jwtKey))
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GameHistoryDbContext>();
            db.Database.Migrate();
        }

        app.Run();
    }
}
