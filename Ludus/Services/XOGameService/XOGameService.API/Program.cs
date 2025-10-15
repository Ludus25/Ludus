using MassTransit;
using StackExchange.Redis;
using XOGameService.API.Hubs;
using XOGameService.API.Middlewares;
using XOGameService.API.Repositories;
using XOGameService.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("RedisCacheSettings:ConnectionString");
});


builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(builder.Configuration["RabbitMQ:HostAddress"]);
        });

builder.Services.AddScoped<IXOGameService, GameService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("RedisCacheSettings:ConnectionString"))
);

builder.Services.AddScoped<IXOGameRepository, RedisXOGameRepository>();
builder.Services.AddScoped<IXOGameService, GameService>();

// gRPC
builder.Services.AddGrpc();

const string CorsPolicy = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, p =>
        p.SetIsOriginAllowed(_ => true)
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors(CorsPolicy);

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapHub<GameHub>("/gamehub").RequireCors(CorsPolicy);

// gRPC endpoint
app.MapGrpcService<GameGrpcService>();

app.Run();