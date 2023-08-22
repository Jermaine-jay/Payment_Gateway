using static Payment_Gateway.BLL.Implementation.Services.WalletService;

namespace Payment_Gateway.BLL.Interfaces.IServices
{
    public interface IWalletService
    {
        Task<bool> UpdateBlance(string userId, long amount);
        Task<DebitResponseDto> CheckBalance(string userId, long amount);
    }
}
