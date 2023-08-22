using System.ComponentModel.DataAnnotations.Schema;

namespace Payment_Gateway.Models.Entities
{
    public class TransactionHistory : BaseEntity
    {

        [ForeignKey("Wallet")]
        public string WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public IEnumerable<Payout> DebitTransactionList { get; set; }
        public IEnumerable<Transaction> CreditTransactionList { get; set; }
    }
}
