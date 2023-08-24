namespace Payment_Gateway.Shared.DataTransferObjects
{
    public class TransactionHistoryDto
    {
        public string? Transactionid { get; set; }
        public string? WalletId { get; set; }

        public IEnumerable<PayoutDto> DebitTransactionList { get; set; }
        public IEnumerable<TransactionResponse> CreditTransactionList { get; set; }
    }
}
