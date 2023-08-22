using Microsoft.AspNetCore.Http;
using Payment_Gateway.BLL.Interfaces;

namespace Payment_Gateway.BLL.Infrastructure.ApiKeyMiddleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;


        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context, IAuthenticationService authenticationService)
        {

            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("API Key is not present");
                return;
            }

            var header = context.Request.Headers["Authorization"].ToString();
            var key = header.Substring(7);

            var str = await authenticationService.GetApiKey(key);
            if (str == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized User");
                return;
            }

            await _next(context);
        }
    }
}
