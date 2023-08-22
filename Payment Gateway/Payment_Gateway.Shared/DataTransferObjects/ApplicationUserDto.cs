namespace Payment_Gateway.Shared.DataTransferObjects
{
    public class ApplicationUserDto
    {
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RecoveryMail { get; set; }
        public string Birthday { get; set; }
        public string? Wallet { get; set; }
        public string? ApiSecretKey { get; set; }
    }
}
