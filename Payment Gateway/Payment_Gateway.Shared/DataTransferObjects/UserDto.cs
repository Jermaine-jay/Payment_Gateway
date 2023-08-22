namespace Payment_Gateway.Shared.DataTransferObjects
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? WalletId { get; set; }
        public string? Birthday { get; set; }
        public long? Balance { get; set; }
        public string? ApiSecretKey { get; set; }
    }
}
