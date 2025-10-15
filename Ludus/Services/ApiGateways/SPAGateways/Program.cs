using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddAuthentication("LudusAuth")
    .AddJwtBearer("LudusAuth", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/ws"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

const string CorsPolicy = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "API Gateway up & running");
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    env = app.Environment.EnvironmentName
}));

app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseWebSockets();

app.Use(async (context, next) =>
{
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        Console.WriteLine("[GATEWAY] --- Claims after Authentication (Manual Header Injection) ---");
        string userId = null;
        string userRole = null;
        string userEmail = null;

        foreach (var claim in context.User.Claims)
        {
            Console.WriteLine($"[GATEWAY] Claim: {claim.Type} = {claim.Value}");
            if (claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
            {
                userId = claim.Value;
            }
            if (claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            {
                userRole = claim.Value;
            }
            if (claim.Type.Contains("email", StringComparison.OrdinalIgnoreCase))
            {
                userEmail = claim.Value;
            }
        }
        Console.WriteLine("[GATEWAY] -------------------------------------------------------------");

        if (!string.IsNullOrEmpty(userId))
        {
            context.Request.Headers.TryAdd("X-UserId", userId);
            Console.WriteLine($"[GATEWAY] Manually injected X-UserId: {userId}");
        }
        if (!string.IsNullOrEmpty(userRole))
        {
            context.Request.Headers.TryAdd("X-UserRole", userRole);
            Console.WriteLine($"[GATEWAY] Manually injected X-UserRole: {userRole}");
        }
        if (!string.IsNullOrEmpty(userEmail))
        {
            context.Request.Headers.TryAdd("X-UserEmail", userEmail);
            Console.WriteLine($"[GATEWAY] Manually injected X-UserEmail: {userEmail}");
        }
        context.Request.Headers.TryAdd("X-Debug-Manual", "ManualInjectWorks");
        Console.WriteLine("[GATEWAY] Manually injected X-Debug-Manual: ManualInjectWorks");

    }
    else
    {
        Console.WriteLine("[GATEWAY] Custom Middleware: User not authenticated at this point (before Ocelot).");
    }

    await next();
});

app.Use(async (context, next) =>
{
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        Console.WriteLine("[GATEWAY] --- Claims after Authentication ---");
        foreach (var claim in context.User.Claims)
        {
            Console.WriteLine($"[GATEWAY] Claim: {claim.Type} = {claim.Value}");
        }
        Console.WriteLine("[GATEWAY] ---------------------------------");
    }
    else
    {
        Console.WriteLine("[GATEWAY] Custom Middleware: User not authenticated at this point.");
    }
    await next();
});

await app.UseOcelot();

app.Run();
