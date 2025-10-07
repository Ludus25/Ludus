using System.Text.Json;
using XOGameService.API.Exceptions;

namespace XOGameService.API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext ctx)
        {
            try
            {
                await _next(ctx);
            }
            catch (XOGameException ex)
            {
                ctx.Response.ContentType = "application/json";
                ctx.Response.StatusCode = StatusCodes.Status409Conflict;

                var payload = new
                {
                    code = (int)ex.Code,
                    message = ex.Message,
                    traceId = ctx.TraceIdentifier
                };

                await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        }
    }
}
