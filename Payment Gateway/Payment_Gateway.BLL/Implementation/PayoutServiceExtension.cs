using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Payment_Gateway.API.Extensions;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.BLL.Interfaces.IServices;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Models.Extensions;
using Payment_Gateway.Shared.DataTransferObjects.Response;
using System.Net;

namespace Payment_Gateway.BLL.Implementation
{
    public class PayoutServiceExtension : IPayoutServiceExtension
    {
        private readonly IRepository<ApplicationUser> _appuserRepo;
        private readonly IRepository<Payout> _payoutRepo;
        private readonly IRepository<TransactionHistory> _TrasHisRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceFactory _serviceFactory;


        public PayoutServiceExtension(IUnitOfWork unitOfWork, IMapper mapper, IServiceFactory serviceFactory)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _appuserRepo = _unitOfWork.GetRepository<ApplicationUser>();
            _TrasHisRepo = _unitOfWork.GetRepository<TransactionHistory>();
            _serviceFactory = serviceFactory;
        }



        public async Task<object> CreatePayout(string userId, TransferResponse response)
        {
            var user = await _appuserRepo.GetSingleByAsync(x => x.Id.ToString() == userId, include: u => u.Include(x => x.Wallet), tracking: true);
            if (user != null)
            {
                TransactionHistory history = new()
                {
                    WalletId = user.WalletId,
                    DebitTransactionList = new List<Payout>
                    {
                        new Payout()
                        {
                            Reason = response.data.reason,
                            Amount = response.data.amount,
                            Recipient = response.data.recipient,
                            CreatedAt = response.data.createdAt,
                            Responsestatus = response.status,
                            Status = response.data.status,
                            Currency = response.data.currency,
                            Reference = response.data.reference,
                            payoutId = response.data.Id,
                        }
                    }

                };

                var ops = await _TrasHisRepo.AddAsync(history);
                return ops;
            }
            return new InvalidOperationException("Can not create payout");
        }


        public async Task DebitTransfee(string userId, TransferResponse response)
        {
            var feeAmount = await TranscationFees(response.data.amount);
            var user = await _appuserRepo.GetSingleByAsync(x => x.Id.ToString() == userId, include: u => u.Include(x => x.Wallet), tracking: true);
            if (user != null)
            {
                TransactionHistory history = new()
                {
                    WalletId = user.WalletId,
                    DebitTransactionList = new List<Payout>
                    {
                        new Payout()
                        {

                            WalletId = user.WalletId.ToString(),
                            Reason = "Transaction Fee",
                            Amount = feeAmount,
                            CreatedAt = response.data.createdAt,
                            Responsestatus = response.status,
                            Status = response.data.status,
                            Currency = response.data.currency,
                            Reference = response.data.reference,
                            payoutId = response.data.Id,
                        }
                    }
                };
                var ops = await _TrasHisRepo.AddAsync(history);
            }
        }


        public async Task<bool> UpdatePayout(string walletId, FinalizeTransferResponse Response)
        {
            //var user = await _appuserRepo.GetSingleByAsync(x => x.Id.ToString() == userId, include: u => u.Include(x => x.Wallet), tracking: true);
            var trans = await _TrasHisRepo.GetSingleByAsync(u => u.WalletId == walletId);
            var payout = trans.DebitTransactionList.SingleOrDefault(x => x.payoutId == Response.data.Id);
            if (payout.Responsestatus == true)
            {
                payout.Status = Response.status;
                await _payoutRepo.UpdateAsync(payout);
                return true;
            }
            return false;
        }


        public async Task<long> TranscationFees(long Amount)
        {
            if (Amount < 1000)
                return 10;
            if (Amount < 10000)
                return 20;
            if (Amount < 50000)
                return 30;
            if (Amount < 100000)
                return 40;
            if (Amount < 200000)
                return 50;
            if (Amount < 300000)
                return 70;
            if (Amount < 400000)
                return 80;
            if (Amount < 500000)
                return 90;
            if (Amount < 600000)
                return 100;
            if (Amount < 700000)
                return 110;
            if (Amount < 800000)
                return 120;
            if (Amount < 900000)
                return 130;
            //if (Amount < 1000000)
            //    return 140;
            return 150;
        }


        public async Task<object> CompleteTransfer(string userId, string pin, FinalizeTransferResponse response)
        {
            if (userId == null)
            {
                return new InvalidOperationException("User Not Found ");
            }

            var user = await _appuserRepo.GetSingleByAsync(x => x.Id.ToString() == userId);
            var enteredpin = SHA256Hasher.Hash(pin);

            if (pin != user.Pin)
            {
                return new ServiceResponse<CompleteTransferResponse>
                {
                    Message = "Incorrect Pin",
                    StatusCode = HttpStatusCode.BadRequest,
                };
            }

            if (response.data.status != "success" && response == null)
            {
                return new ServiceResponse<CompleteTransferResponse>
                {
                    Message = "Transfer could not be Completed",
                    StatusCode = HttpStatusCode.BadRequest,
                };
            }

            int amount = int.Parse(response.data.amount) * (-1);
            await _serviceFactory.GetService<IWalletService>().UpdateBlance(userId, amount);
            await UpdatePayout(userId, response);
            return new ServiceResponse<CompleteTransferResponse>
            {
                Message = "Transfer Completed",
                StatusCode = HttpStatusCode.OK,
            };
        }


        public class CompleteTransferResponse
        {
            public int amount { get; set; }
            public string Message { get; set; }
        }
    }
}
