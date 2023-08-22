namespace Payment_Gateway.BLL.Infrastructure.Otp
{
    public record OtpCodeDto
    {
        public string Otp { get; set; }

        public int Attempts { get; set; }

        public OtpCodeDto(string otp)
        {
            Otp = otp;
            Attempts = 0;
        }
    }
}
