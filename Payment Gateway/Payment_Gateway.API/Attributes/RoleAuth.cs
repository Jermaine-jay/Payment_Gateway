using Microsoft.AspNetCore.Authorization;

namespace Payment_Gateway.API.Attribute
{
    public class AuthRequirement : IAuthorizationRequirement
    {
        private readonly string _routeName;


    }
}
