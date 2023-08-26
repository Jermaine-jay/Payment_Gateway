using Payment_Gateway.API.Extensions;
using Payment_Gateway.Shared.DataTransferObjects;
using Payment_Gateway.Shared.DataTransferObjects.Response;

namespace Payment_Gateway.BLL.Interfaces.IServices
{
    public interface IAdminServices
    {
        Task<ServiceResponse<CheckBalanceResponse>> CheckBalance();
        Task<ServiceResponse<FetchLedgerResponse>> FetchLedger();
        Task<ServiceResponse<UserDto>> GetUserDetails(string walletId);
        Task<ServiceResponse<IEnumerable<ApplicationUserDto>>> GetAllUsers();
        Task<ServiceResponse<IList<UserDto>>> GetAllUsersWithBalance();

        Task<ServiceResponse<UserDto>> GetUser(string userId);

        Task<ServiceResponse> DeleteUser(string userId);
    }
}
