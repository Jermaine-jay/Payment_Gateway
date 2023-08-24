using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Payment_Gateway.API.Extensions;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Request;
using System.Net;

namespace Payment_Gateway.Shared.DataTransferObjects
{
    public class TransactionService : ITransactionService
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IRepository<TransactionHistory> _transRepo;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly IRepository<Wallet> _walletRepo;
        private readonly IUnitOfWork _unitOfWork;


        public TransactionService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _serviceFactory = serviceFactory;
            _unitOfWork = _serviceFactory.GetService<IUnitOfWork>();
            _transRepo = _unitOfWork.GetRepository<TransactionHistory>();
            _walletRepo = _unitOfWork.GetRepository<Wallet>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
        }


        public async Task<object> GetAllTransactions()
        {
            var transactionsList = await _transRepo.GetAllAsync();
            if (!transactionsList.Any())
                throw new InvalidOperationException("Transaction list is empty");

            return transactionsList;

        }


        public async Task<ServiceResponse<IEnumerable<TransactionHistory>>> GetTransactionsDetails(string walletId)
        {
            var user = await _userRepo.GetSingleByAsync(b => b.WalletId.ToString().Equals(walletId));
            if (user == null)
            {
                return new ServiceResponse<IEnumerable<TransactionHistory>>
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false,
                };
            }

            var transac = await _transRepo.GetByAsync(u => u.WalletId == user.WalletId);
            if (transac == null)
            {
                return new ServiceResponse<IEnumerable<TransactionHistory>>
                {
                    Message = "Transaction History Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false,
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
                    WalletId = c.WalletId,
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

            return new ServiceResponse<IEnumerable<TransactionHistory>>
            {
                Message = "User Not Found",
                StatusCode = HttpStatusCode.NotFound,
                Success = true,
                Data = result
            };
        }



        public async Task<ServiceResponse<TransactionHistory>> GetTransaction(GetTransactionRequest request)
        {
            var wallet = await _walletRepo.GetAllAsync(include: u => u.Include(t => t.TransactionHistory));
            if (wallet == null)
            {
                return new ServiceResponse<TransactionHistory>
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            var transac = await _transRepo.GetAllAsync(include: u => u.Include(t => t.WalletId));
            var result = transac.Where(u => u.WalletId == request.WalletId).Where(u => u.CreditTransactionList.Any(u => u.Transactionid == request.IransactionId)
             || u.DebitTransactionList.Any(u => u.payoutId == request.IransactionId)).FirstOrDefault();

            if (result == null)
            {
                return new ServiceResponse<TransactionHistory>
                {
                    Message = "Transaction Not Found",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            return new ServiceResponse<TransactionHistory>
            {
                Message = "User Not Found",
                StatusCode = HttpStatusCode.BadRequest,
                Success = true,
                Data = result
            };
        }



        public async Task<object> GetUsersTransactionHistory()
        {
            var b = await _userRepo.GetAllAsync(include: u => u.Include(t => t.Wallet));
            var c = b.Join(await _walletRepo.GetAllAsync(),
                u => u.WalletId,
                e => e.WalletId,
                (u, e) => new
                {
                    e.WalletId,
                    u.UserName,
                    e.Balance
                }).Join(await _transRepo.GetAllAsync(),
                    u => u.WalletId,
                    e => e.WalletId,
                    (u, e) => new
                    {
                        u.WalletId,
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

                        CreditTransactionList = e.CreditTransactionList.Select(c => new Payin
                        {
                            Transactionid = c.Transactionid,
                            Amount = c.Amount,
                            UserId = c.UserId,
                            WalletId = c.WalletId,
                            Reference = c.Reference,
                            Email = c.Email,
                            AccountName = c.AccountName,
                            Bank = c.Bank,
                            Status = c.Status,
                            GatewayResponse = c.GatewayResponse,
                            CreatedAt = c.CreatedAt,
                            PaidAt = c.PaidAt,
                        }),

                    });
            return c;
        }



        public async Task<object> GetTransactionsByDate(string startdate, string enddate, string userId)
        {
            var user = await _userRepo.GetSingleByAsync(p => p.Id.ToString() == userId, include: e => e.Include(e => e.Wallet));
            var transaction = await _transRepo.GetByAsync(u => u.WalletId == user.WalletId);

            return transaction.Where(t => t.CreditTransactionList.Any(u => u.CreatedAt == startdate));

            //var transactions = await _transRepo.GetAllAsync();
            //return transactions.Where(t => t.TransactionDate.Date == date.Date);
        }


        public async Task<ServiceResponse<IEnumerable<Payout>>> GetDebitTransactions(string walletId)
        {
            var user = await _userRepo.GetByAsync(p => p.WalletId.ToString() == walletId);
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
                    u.Id,
                    e.WalletId,
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


            var k = await _userRepo.GetSingleByAsync(u => u.WalletId.ToString() == walletId, include: e => e.Include(u => u.Wallet), tracking: true);
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


        public async Task<ServiceResponse<IEnumerable<Payin>>> GetCreditTransactions(string walletId)
        {
            var user = await _userRepo.GetSingleByAsync(p => p.WalletId.ToString() == walletId, include: e => e.Include(e => e.Wallet), tracking: true);
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
                WalletId = u.WalletId,
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


    }
}
