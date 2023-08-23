using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Payment_Gateway.API.Extensions;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.BLL.Interfaces.IServices;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects;
using System.Net;

namespace Payment_Gateway.BLL.Implementation.Services
{
    public class UserService : IUserService
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IRepository<TransactionHistory> _transRepo;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly IRepository<Wallet> _walletRepo;
        private readonly IUnitOfWork _unitOfWork;


        public UserService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _serviceFactory = serviceFactory;
            _userManager = userManager;
            _mapper = _serviceFactory.GetService<IMapper>();
            _transRepo = _unitOfWork.GetRepository<TransactionHistory>();
            _walletRepo = _unitOfWork.GetRepository<Wallet>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
        }


        public async Task<ServiceResponse<UserDto>> GetUserBalance(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(u => u.Id.ToString() == userId, include: u => u.Include(t => t.Wallet));
            if (user == null)
            {
                return new ServiceResponse<UserDto>
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false
                };
            }
            return new ServiceResponse<UserDto>
            {
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Data = new UserDto
                {
                    Balance = user.Wallet.Balance,
                },
                
            };
        }



        public async Task<ServiceResponse<UserDto>> GetUserDetails(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(e => e.Id.ToString() == userId, include: u => u.Include(u => u.Wallet));
            if (user == null)
            {
                return new ServiceResponse<UserDto>
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false,
                };
            }


            return new ServiceResponse<UserDto>
            {
                StatusCode = HttpStatusCode.OK,
                Data = new UserDto
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Birthday = user.Birthday.ToString(),
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    ApiSecretKey = user.ApiSecretKey,
                    WalletId = user.WalletId,
                    Balance = user.Wallet.Balance,
                },
                Success = true
            };
        }



        public async Task<ServiceResponse<ICollection<TransactionHistory>>> GetTransactionsDetails(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(b => b.Id.ToString().Equals(userId));
            if(user == null)
            {
                return new ServiceResponse<ICollection<TransactionHistory>>
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success =false,
                };
            }

            var transac = await _transRepo.GetByAsync(u => u.WalletId == user.WalletId);
            if(transac == null)
            {
                return new ServiceResponse<ICollection<TransactionHistory>>
                {
                    Message = "No Transactions Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success =false,
                };
            }

            var result = transac.OrderByDescending(u => u.CreatedAt).Select(e => new TransactionHistory
            {
                Id = e.Id,
                WalletId = e.WalletId,
                DebitTransactionList = e.DebitTransactionList.Select(d => new Payout
                {
                    WalletId = d.WalletId,
                    payoutId = d.payoutId,
                    Amount = d.Amount,
                    Reason = d.Reason,
                    Recipient = d.Recipient,
                    Reference = d.Reference,
                    Currency = d.Currency,
                    Source = d.Source,
                    Responsestatus = d.Responsestatus,
                    Status = d.Status,
                    CreatedAt = d.CreatedAt,
                }).ToList(),

                CreditTransactionList = e.CreditTransactionList.Select(c => new Payin
                {
                    Transactionid = c.Transactionid,
                    Amount = c.Amount,
                    UserId = c.UserId,
                    Reference = c.Reference,
                    Email = c.Email,
                    AccountName = c.AccountName,
                    Bank = c.Bank,
                    Status = c.Status,
                    GatewayResponse = c.GatewayResponse,
                    CreatedAt = c.CreatedAt,
                    PaidAt = c.PaidAt,
                }).ToList(),
            });

            return new ServiceResponse<ICollection<TransactionHistory>>
            {
                Message = "User Not Found",
                StatusCode = HttpStatusCode.NotFound,
                Success = true,
                Data = (ICollection<TransactionHistory>)result
            };
        }



        public async Task<IEnumerable<TransactionResponse>> GetAllTransactions(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(b => b.Id.ToString().Equals(userId));
            if (user == null)
            {
                throw new ArgumentNullException("User Not Found");
            }
            var wallet = await _walletRepo.GetSingleByAsync(u => u.WalletId.Equals(user.WalletId), include: u => u.Include(t => t.TransactionHistory));
            if(wallet == null)
            {
                throw new ArgumentNullException("User Not Found");
            }

            var transac = wallet.TransactionHistory;
            return transac.DebitTransactionList.Join(transac.CreditTransactionList,
                debit => debit.Id,
                credit => credit.Id,
                (debit, credit) => new TransactionResponse
                {
                    Amount = debit.Amount,
                    Reference = debit.Reference,
                    Status = debit.Status,
                    Transactionid = debit.payoutId,
                    CreatedAt = debit.CreatedAt,
                    Bank = credit.Bank

                }).OrderByDescending(date => date.CreatedAt);
        }


        public async Task<ServiceResponse<IEnumerable<Payin>>> GetCreditTransactions(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(p => p.Id.ToString() == userId, include: e => e.Include(e => e.Wallet), tracking: true);
            if (user == null)
            {
                return new ServiceResponse<IEnumerable<Payin>>
                {
                    Message = "User Not Founf",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false,
                };
            }
            var result = user.Wallet.TransactionHistory.CreditTransactionList.Select(u => new Payin
            {
                Transactionid = u.Transactionid,
                Amount = u.Amount,
                AccountName = u.AccountName,
                Bank = u.Bank,
                Reference = u.Reference,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                PaidAt = u.PaidAt,
            });

            return new ServiceResponse<IEnumerable<Payin>>
            {
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Data = result
            };
        }


        public async Task<ServiceResponse<IEnumerable<Payout>>> GetDebitTransactions(string userId)
        {
            var user = await _userRepo.GetByAsync(p => p.Id.ToString() == userId);
            if (user == null)
            {
                return new ServiceResponse<IEnumerable<Payout>>
                {
                    Message = "User Not Founf",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false,
                };
            }

            var a = user.Join(await _walletRepo.GetAllAsync(),
                u => u.WalletId,
                e => e.WalletId, (u, e) => new
                {
                    Id = u.Id,
                    WalletId = e.WalletId,
                }).Join(await _transRepo.GetAllAsync(),
            u => u.WalletId,
            e => e.WalletId,
            (u, e) => new
            {
                DebitTransactionList = e.DebitTransactionList.Select(d => new Payout
                {
                    WalletId = d.WalletId,
                    payoutId = d.payoutId,
                    Amount = d.Amount,
                    Reason = d.Reason,
                    Recipient = d.Recipient,
                    Reference = d.Reference,
                    Currency = d.Currency,
                    Source = d.Source,
                    Responsestatus = d.Responsestatus,
                    Status = d.Status,
                    CreatedAt = d.CreatedAt,
                }),
            });


            var k = await _userRepo.GetSingleByAsync(u => u.Id.ToString() == userId, include: e => e.Include(u => u.Wallet), tracking: true);
            var result = k.Wallet.TransactionHistory.DebitTransactionList.Select(u => new Payout
            {
                payoutId = u.payoutId,
                Amount = u.Amount,
                Reason = u.Reason,
                Recipient = u.Recipient,
                Reference = u.Reference,
                CreatedAt = u.CreatedAt,
                Status = u.Status,
            });

            return new ServiceResponse<IEnumerable<Payout>>
            {
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Data = result
            };
        }

    }
}
