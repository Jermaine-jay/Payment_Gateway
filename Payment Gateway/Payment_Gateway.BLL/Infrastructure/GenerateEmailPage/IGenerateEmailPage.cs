namespace Payment_Gateway.BLL.Infrastructure.GenerateEmailPage
{
    public interface IGenerateEmailPage
    {
        public string EmailVerificationPage(string name, string token);
        public string PasswordResetPage(string callbackurl);
        public string ChangePasswordPage(string code);
    }
}
