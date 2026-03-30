using TrackAPI.Exceptions;

namespace TrackAPI.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (WrongLoginException loginException)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(loginException.Message);
            }
            catch (NotUserFoundException notUserFoundException)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(notUserFoundException.Message);
            }
        }
    }
}
