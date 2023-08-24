using Payment_Gateway.API.Extensions;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Request;

namespace Payment_Gateway.BLL.Interfaces
{
    public interface ITransactionService
    {
        Task<object> GetAllTransactions();
        Task<object> GetTransactionsByDate(string startdate, string enddate, string userId);
        Task<ServiceResponse<IEnumerable<Payout>>> GetDebitTransactions(string userId);
        Task<ServiceResponse<IEnumerable<Payin>>> GetCreditTransactions(string userId);
        Task<ServiceResponse<TransactionHistory>> GetTransaction(GetTransactionRequest request);
        Task<ServiceResponse<IEnumerable<TransactionHistory>>> GetTransactionsDetails(string walletId);

    }

}
