using Payment_Gateway.API.Extensions;
using Payment_Gateway.BLL.Implementation;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects;

namespace Payment_Gateway.BLL.Interfaces.IServices
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDto>> GetUserBalance(string userId);
        Task<ServiceResponse<UserDto>> GetUserDetails(string userId);
        Task<ServiceResponse<IEnumerable<TransactionHistory>>> GetTransactionsDetails(string userId);
        Task<IEnumerable<TransactionResponse>> GetAllTransactions(string walletId);
        Task<ServiceResponse<IEnumerable<Payout>>> GetDebitTransactions(string userId);
        Task<ServiceResponse<IEnumerable<Transaction>>> GetCreditTransactions(string userId);
    }
}
