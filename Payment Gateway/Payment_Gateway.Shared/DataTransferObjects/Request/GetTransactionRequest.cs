namespace Payment_Gateway.Shared.DataTransferObjects.Request
{
    public class GetTransactionRequest
    {
        public string? WalletId { get; set; }
        public string? IransactionId { get; set; }
    }
}
