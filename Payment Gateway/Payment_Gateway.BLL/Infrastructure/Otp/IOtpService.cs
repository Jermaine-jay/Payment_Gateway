namespace Payment_Gateway.BLL.Infrastructure.Otp
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string userId, OtpOperation operation);
        Task<bool> VerifyOtpAsync(string userId, string otp, OtpOperation operation);
        Task<string> GenerateUnoqueOtpAsync(string userId, OtpOperation operation);

        Task<bool> VerifyUniqueOtpAsync(string userId, string otp, OtpOperation operation);
    }
}
