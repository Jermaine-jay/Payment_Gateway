using Payment_Gateway.Models.Entities;

namespace Payment_Gateway.Shared.DataTransferObjects
{
    public class TransactionHistoryDto
    {
        public string? Id { get; set; }
        public string? WalletId { get; set; }
        public string? CreatedAt { get; set; }
/*
        public IEnumerable<PayOutDto> DebitTransactionList { get; set; }
        public IEnumerable<PayInDto> CreditTransactionList { get; set; }*/
    }
}
