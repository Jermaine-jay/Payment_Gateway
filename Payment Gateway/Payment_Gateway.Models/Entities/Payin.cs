using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment_Gateway.Models.Entities
{
    public class Payin
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Transactionid { get; set; }
        public long Amount { get; set; }
        public string? UserId { get; set; }
        public string? Reference { get; set; }
        public string? Email { get; set; }
        public string? AccountName { get; set; }
        public string Bank { get; set; }
        public string? Status { get; set; }
        public string? GatewayResponse { get; set; }
        public string? CreatedAt { get; set; }
        public string? PaidAt { get; set; }
        public string? AuthorizationCode { get; set; }
        public string? WalletId { get; set; }
        public string? IpAddress { get; set; }
        public string? Channel { get; set; }
        public string? CardType { get; set; }

        [ForeignKey("TransactionHistory")]
        public string? TransactionHistoryId { get; set; }
        public TransactionHistory? TransactionHistory { get; set; }
    }
}
