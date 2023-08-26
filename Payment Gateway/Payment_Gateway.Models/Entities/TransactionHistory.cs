using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment_Gateway.Models.Entities
{
    public class TransactionHistory
    {
        [Key]
        public string? Id { get; set; } = Guid.NewGuid().ToString();
        public string? WalletId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public IList<Payout>? DebitTransactionList { get; set; }
        public IList<Payin>? CreditTransactionList { get; set; }
    }
}
