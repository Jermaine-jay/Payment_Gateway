namespace Payment_Gateway.Shared.DataTransferObjects
{
    public class TransactionDto
    {
        public long Id { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNumber { get; set; }
    }

}
