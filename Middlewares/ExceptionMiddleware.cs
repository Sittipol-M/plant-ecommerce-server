using plant_ecommerce_server.Exceptions;
using plant_ecommerce_server.Responses;

namespace plant_ecommerce_server.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            if (exception is UnauthorizedException)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync<UnauthorizedExceptionResponse>(
                    new()
                    {
                        Message = exception.Message
                    });
            }
            else if (exception is ConflictException)
            {
                context.Response.StatusCode = 409;
                await context.Response.WriteAsJsonAsync<ConflictExceptionResponse>(
                    new()
                    {
                        Message = exception.Message
                    });
            } else if (exception is NotFoundException) {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync<ConflictExceptionResponse>(
                    new()
                    {
                        Message = exception.Message
                    });
            }
        }
    }
}