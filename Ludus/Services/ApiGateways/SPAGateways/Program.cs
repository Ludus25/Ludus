using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

//builder.Services
//    .AddAuthentication(options =>
//    {
//        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    })
//    .AddJwtBearer("Bearer", options =>
//    {
//        options.Authority = builder.Configuration["Jwt:Authority"];
//        options.Audience = builder.Configuration["Jwt:Audience"];
//        options.RequireHttpsMetadata = bool.TryParse(builder.Configuration["Jwt:RequireHttpsMetadata"], out var https) && https;

//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true
//        };
//        options.Events = new JwtBearerEvents
//        {
//            OnMessageReceived = ctx =>
//            {
//                var accessToken = ctx.Request.Query["access_token"];
//                var path = ctx.HttpContext.Request.Path;

//                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/ws"))
//                {
//                    ctx.Token = accessToken;
//                }
//                return Task.CompletedTask;
//            }
//        };
//    });

//builder.Services.AddAuthorization();

const string CorsPolicy = "AllowAll";
builder.Services.AddCors(o => o.AddPolicy(CorsPolicy, p =>
    p.SetIsOriginAllowed(_ => true)
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()));


builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "API Gateway up & running");
app.MapGet("/health", () => Results.Ok(new { status = "ok", env = app.Environment.EnvironmentName }));

app.UseCors(CorsPolicy);

app.UseWebSockets();

//app.UseAuthentication();
//app.UseAuthorization();

await app.UseOcelot();

app.Run();
