using System.Text.Json;

namespace PayNestAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // proceed to the next middleware
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var problem = new
                {
                    status = 500,
                    title = "Internal Server Error",
                    detail = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred.",
                    stackTrace = _env.IsDevelopment() ? ex.StackTrace : null
                };

                var json = JsonSerializer.Serialize(problem);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
