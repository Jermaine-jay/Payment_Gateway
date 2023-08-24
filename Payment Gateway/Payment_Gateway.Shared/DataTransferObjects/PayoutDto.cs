namespace Payment_Gateway.Shared.DataTransferObjects
{
    public class PayoutDto
    {
        public string? Id { get; set; }
        public string? payoutId { get; set; }
        public long Amount { get; set; }
        public string? Reason { get; set; }
        public string? Recipient { get; set; }
        public string? Reference { get; set; }
        public string? Currency { get; set; }
        public string? Source { get; set; }
        public bool? Responsestatus { get; set; }
        public string? Status { get; set; }
        public string? WalletId { get; set; }
        public string? CreatedAt { get; set; }
    }
}
