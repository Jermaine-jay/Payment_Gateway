using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Payment_Gateway.BLL.Interfaces
{
    public interface IPaystackService
    {
        Task<PaymentResponseDto> InitializeTransactionAsync(string email, decimal amount, string currency);
        Task<Transaction> VerifyTransactionAsync(string reference);
    }

}
