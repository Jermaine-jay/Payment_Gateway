namespace Payment_Gateway.BLL.Interfaces
{
    public class PaymentResponseDto
    {
        public string AuthorizationUrl { get; set; }
        public string AccessCode { get; set; }
        public string Reference { get; set; }
    }

}
