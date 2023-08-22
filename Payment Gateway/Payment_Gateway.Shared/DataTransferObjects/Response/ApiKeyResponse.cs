using Payment_Gateway.Models.Entities;

namespace Payment_Gateway.Shared.DataTransferObjects.Response
{
    public class ApiKeyResponse
    {
        public ApplicationUser User { get; set; }
        public bool Success { get; set; }
    }
}
