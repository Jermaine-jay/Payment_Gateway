using Payment_Gateway.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment_Gateway.Models.Entities
{
    public class Wallet
    {
        [Key]
        public string? WalletId { get; set; }
        public long Balance { get; set; }
        public Currency Currency { get; set; } = Currency.NGN;
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public string? TransactionHistoryId { get; set; }

        [ForeignKey(nameof(TransactionHistoryId))]
        public TransactionHistory? TransactionHistory { get; set; }

    }

}
