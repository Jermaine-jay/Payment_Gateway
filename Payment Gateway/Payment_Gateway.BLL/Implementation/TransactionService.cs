using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Payment_Gateway.API.Extensions;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Request;
using System.Linq.Dynamic.Core;
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
        private readonly UserManager<ApplicationUser> _userManager;


        public TransactionService(UserManager<ApplicationUser> userManager, IServiceFactory serviceFactory, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
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
            var wallet = await _walletRepo.GetSingleByAsync(b => b.WalletId.Equals(walletId));
            if (wallet == null)
            {
                return new ServiceResponse<IEnumerable<TransactionHistory>>
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false,
                };
            }

            var transac = await _transRepo.GetAllAsync();
            var trans = transac.Where(u => u.WalletId == wallet.WalletId);

            if (transac == null)
            {
                return new ServiceResponse<IEnumerable<TransactionHistory>>
                {
                    Message = "Transaction History Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false,
                };
            }

            /* var result = transac.OrderByDescending(u => u.CreatedAt).Select(e => new TransactionHistory
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
             });*/

            return new ServiceResponse<IEnumerable<TransactionHistory>>
            {
                Message = "User Not Found",
                StatusCode = HttpStatusCode.NotFound,
                Success = true,
                Data = trans
            };
        }



        public async Task<ServiceResponse<TransactionHistory>> GetTransaction(GetTransactionRequest request)
        {
            var wallet = await _walletRepo.GetSingleByAsync(u=> u.WalletId == request.WalletId);
            if (wallet == null)
            {
                return new ServiceResponse<TransactionHistory>
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            var transac = await _transRepo.GetAllAsync();

            var result = transac.Where(u => u.WalletId == request.WalletId && (u.CreditTransactionList.Any(u => u.Transactionid == request.TransactionId)
             || u.DebitTransactionList.Any(u => u.payoutId == request.TransactionId))).FirstOrDefault();

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
            var wall = _userManager.Users.AsQueryable();
            var tran = _userManager.Users.AsQueryable();
            var b = _userManager.Users;

      /*      var c = b.Join(wall.ToList(),
                u => u.WalletId,
                e => e.WalletId,
                (u, e) => new
                {
                    e.WalletId,
                    u.UserName,
                }).Join(tran.ToList(),
                    u => u.WalletId,
                    e => e.WalletId,
                    (u, e) => new
                    {
                        u.WalletId,
                        e.UserName,
                        DebitTransactionList = e.Wallet.DebitTransactionList.Select(d => new Payout
                        {
                            Id = d.Id,
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

                        CreditTransactionList = e.Wallet.TransactionHistory.CreditTransactionList.Select(c => new Payin
                        {
                            Id = c.Id,
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
                    });*/

            return b;
        }


        public async Task<object> GetTransactionsByDate(string startdate, string enddate, string userId)
        {
            var user = await _userRepo.GetSingleByAsync(p => p.Id.ToString() == userId, include: e => e.Include(e => e.Wallet));
            var transaction = await _transRepo.GetByAsync(u => u.WalletId == user.WalletId);

            return transaction.Where(t => t.CreditTransactionList.Any(u => u.CreatedAt == startdate));

            //var transactions = await _transRepo.GetAllAsync();
            //return transactions.Where(t => t.TransactionDate.Date == date.Date);
        }


        public async Task<ServiceResponse<IEnumerable<PayOutDto>>> GetDebitTransactions(string walletId)
        {

            var trans = await _transRepo.GetSingleByAsync(u => u.WalletId == walletId, include: e => e.Include(u => u.DebitTransactionList));
            var result = trans.DebitTransactionList.Select(u => new PayOutDto
            {
                payoutId = u.payoutId,
                Amount = u.Amount,
                Reason = u.Reason,
                Recipient = u.Recipient,
                Reference = u.Reference,
                CreatedAt = u.CreatedAt,
                Status = u.Status,
            });

            return new ServiceResponse<IEnumerable<PayOutDto>>
            {
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Data = result,
            };
        }


        public async Task<ServiceResponse<IEnumerable<Payin>>> GetCreditTransactions(string walletId)
        {
            var user = await _transRepo.GetSingleByAsync(p => p.WalletId == walletId, include: u => u.Include(u => u.CreditTransactionList), tracking: true);
            if (user == null)
            {
                return new ServiceResponse<IEnumerable<Payin>>
                {
                    Message = "User Not Founf",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false,
                };
            }

            var result = user.CreditTransactionList.Select(c => new Payin
            {
                Id = c.Id,
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
            });

            return new ServiceResponse<IEnumerable<Payin>>
            {
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Data = result
            };
        }


        public class PayInDto
        {
            public string? Id { get; set; }
            public string? Transactionid { get; set; }
            public long Amount { get; set; }
            public string? UserId { get; set; }
            public string? Reference { get; set; }
            public string? Email { get; set; }
            public string? AccountName { get; set; }
            public string Bank { get; set; }
            public string? Status { get; set; }
            public string? GatewayResponse { get; set; }
            public string? CreatedAt { get; set; }
            public string? PaidAt { get; set; }
            public string? AuthorizationCode { get; set; }
            public string? WalletId { get; set; }
            public string? IpAddress { get; set; }
            public string? Channel { get; set; }
            public string? CardType { get; set; }
        }

        public class PayOutDto
        {
            public string payoutId { get; set; }
            public long Amount { get; set; }
            public string? Reason { get; set; }
            public string? Recipient { get; set; }
            public string? Reference { get; set; }
            public string Currency { get; set; }
            public string? Source { get; set; }
            public bool? Responsestatus { get; set; }
            public string? Status { get; set; }
            public string? WalletId { get; set; }
            public string? CreatedAt { get; set; }
        }
    }
}
