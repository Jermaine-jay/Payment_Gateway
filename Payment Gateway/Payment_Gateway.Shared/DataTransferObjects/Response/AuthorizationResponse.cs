using Microsoft.AspNetCore.Identity;

namespace Payment_Gateway.Shared.DataTransferObjects.Response
{
    public class AuthorizationResponse
    {
        public string? Message { get; set; }
        public bool? Succeeded { get; set; }
        public string? UserId { get; set; }
        //public string? Id { get; set; }
        public string? ApiSecretKey { get; set; }
        public IdentityResult? Result { get; set; }

    }
}
