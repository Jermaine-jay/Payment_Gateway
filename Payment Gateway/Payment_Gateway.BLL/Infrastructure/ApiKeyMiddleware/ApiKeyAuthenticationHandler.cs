using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payment_Gateway.Models.Enums;
using System.Security.Claims;
using System.Text.Encodings.Web;
using IAuthenticationService = Payment_Gateway.BLL.Interfaces.IAuthenticationService;


namespace Payment_Gateway.BLL.Infrastructure.ApiKeyMiddleware
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IAuthenticationService _authenticationService;

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
            IAuthenticationService authenticationService) : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Invalid API Key");
            }

            var header = Request.Headers["Authorization"].ToString();
            var key = header.Substring(7);

            var user = await _authenticationService.GetApiKey(key);
            if (user == null)
            {
                return AuthenticateResult.Fail("Unauthorized User");
            }

            var identity = new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.UserType.GetStringValue()),
                }, Scheme.Name);

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }


        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.ContentType = "text/plain";
            return Response.WriteAsync("Authorization Failed");
        }
    }

}
