using Payment_Gateway.API.Extensions;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Request;
using static Payment_Gateway.Shared.DataTransferObjects.TransactionService;

namespace Payment_Gateway.BLL.Interfaces
{
    public interface ITransactionService
    {
        Task<object> GetAllTransactions();
        Task<object> GetUsersTransactionHistory();
        Task<object> GetTransactionsByDate(string startdate, string enddate, string userId);
        Task<ServiceResponse<IEnumerable<PayOutDto>>> GetDebitTransactions(string walletId);
        Task<ServiceResponse<IEnumerable<Payin>>> GetCreditTransactions(string walletId);
        Task<ServiceResponse<TransactionHistory>> GetTransaction(GetTransactionRequest request);
        Task<ServiceResponse<IEnumerable<TransactionHistory>>> GetTransactionsDetails(string walletId);

    }

}
