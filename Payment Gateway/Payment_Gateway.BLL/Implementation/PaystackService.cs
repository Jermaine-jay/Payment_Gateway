using Payment_Gateway.BLL.Interfaces;
using System.Transactions;

namespace Payment_Gateway.BLL.Implementation
{
    public class PaystackService
    {
        public Task<PaymentResponseDto> InitializeTransactionAsync(string email, decimal amount, string currency)
        {
            throw new NotImplementedException();
        }

        public Task<Transaction> VerifyTransactionAsync(string reference)
        {
            throw new NotImplementedException();
        }
    }
}
