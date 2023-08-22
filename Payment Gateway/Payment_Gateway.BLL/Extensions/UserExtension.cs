using System.Security.Claims;

namespace Payment_Gateway.BLL.Extentions
{
    public static class UserExtension
    {
        public static string? GetUserId(this ClaimsPrincipal user)
        {

            return user.FindFirstValue("UserId");
        }
    }
}
