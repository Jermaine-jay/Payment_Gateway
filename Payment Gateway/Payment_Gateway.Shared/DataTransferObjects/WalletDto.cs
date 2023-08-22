namespace Payment_Gateway.Shared.DataTransferObjects
{
    public class WalletDto
    {
        public string WalletId { get; set; }
        public long Balance { get; set; }
        public string? Currency { get; set; }
        public string? UserId { get; set; }
    }
}
