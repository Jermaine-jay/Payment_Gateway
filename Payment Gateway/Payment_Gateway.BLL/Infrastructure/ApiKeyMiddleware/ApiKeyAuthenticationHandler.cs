using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Models.Enums;
using System.Security.Claims;
using System.Text.Encodings.Web;
using IAuthenticationService = Payment_Gateway.BLL.Interfaces.IAuthenticationService;

namespace Payment_Gateway.BLL.Infrastructure.ApiKeyMiddleware
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IAuthenticationService _authenticationService;
        private string _header { get; set; }
        private ApplicationUser _user { get; set; }

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
            IAuthenticationService authenticationService) : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.ContainsKey("Authorization"))
            {
                var header = Request.Headers["Authorization"].ToString();
                var key = header.Substring(7);
                var user = await _authenticationService.GetApiKey(key);

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
            return AuthenticateResult.Fail("Authorization failed: No API key provided");
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                Response.StatusCode = 401;
                Response.ContentType = "text/plain";
                return Response.WriteAsync("Authorization failed: No API key provided");
            }
            else
            {
                var header = Request.Headers["Authorization"].ToString();
                
                var user = _authenticationService.GetApiKey(header.Substring(7)).Result;
                if (header.Length <= 7 || user == null)
                {
                    Response.StatusCode = 401;
                    Response.ContentType = "text/plain";
                    return Response.WriteAsync("Invalid API key");
                }
                else
                {
                    Response.StatusCode = 404;
                    Response.ContentType = "text/plain";
                    return Response.WriteAsync("Authorization failed");
                }
            }
        }
    }

}
