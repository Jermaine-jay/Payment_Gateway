namespace Payment_Gateway.Shared.DataTransferObjects.Response
{
    public class ChangePasswordResponse
    {
        public string? Message { get; set; }
        public string? Code { get; set; }
        public string? Token { get; set; }
        public bool Success { get; set; }
    }
}
