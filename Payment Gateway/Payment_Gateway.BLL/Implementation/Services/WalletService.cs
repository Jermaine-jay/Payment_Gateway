using Microsoft.EntityFrameworkCore;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.BLL.Interfaces.IServices;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;

namespace Payment_Gateway.BLL.Implementation.Services
{
    public class WalletService : IWalletService
    {
        private readonly IRepository<ApplicationUser> _UserRepo;
        private readonly IRepository<Wallet> _WalletRepo;
        public readonly IUnitOfWork _unitOfWork;
        public readonly IPayoutServiceExtension _payoutServiceExtension;


        public WalletService(IUnitOfWork unitOfWork, IPayoutServiceExtension payoutServiceExtension)
        {
            _unitOfWork = unitOfWork;
            _UserRepo = _unitOfWork.GetRepository<ApplicationUser>();
            _WalletRepo = _unitOfWork.GetRepository<Wallet>();
            _payoutServiceExtension = payoutServiceExtension;
        }


        public async Task<bool> UpdateBlance(string userId, long amount)
        {
            var tranFee = await _payoutServiceExtension.TranscationFees(amount);
            var user = await _UserRepo.GetSingleByAsync(x => x.Id.ToString() == userId, include: u => u.Include(x => x.Wallet), tracking: true);
            var balance = user.Wallet.Balance;
            var wallet = user.Wallet;

            if (wallet != null)
            {
                return false;
            }

            var newBalance = balance + (amount + tranFee);
            user.Wallet.Balance = newBalance;
            await _WalletRepo.UpdateAsync(wallet);
            return true;
        }



        public async Task<DebitResponseDto> CheckBalance(string userId, long amount)
        {
            var tranFee = await _payoutServiceExtension.TranscationFees(amount);
            var user = await _UserRepo.GetSingleByAsync(x => x.Id.ToString() == userId, include: u => u.Include(x => x.Wallet), tracking: true);
            var balance = user.Wallet.Balance;
            var wallet = user.Wallet;

            if (string.IsNullOrEmpty(user.Id.ToString()))
            {
                return new DebitResponseDto
                {
                    Status = false,
                    Message = "User Does Not Exist"
                };
            }

            if (balance < amount)
            {
                return new DebitResponseDto
                {
                    Status = false,
                    Message = "Insufficient Funds"
                };
            }

            return new DebitResponseDto
            {
                Status = true,
                Message = "User Found",
            };

        }


        public class DebitResponseDto
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public long? Balance { get; set; }

        }


    }
}
