using Microsoft.AspNetCore.Authorization;
namespace Payment_Gateway.BLL.Infrastructure
{
    public class AuthorizationRequirment : IAuthorizationRequirement
    {
        public int Success { get; set; }
    }
}
