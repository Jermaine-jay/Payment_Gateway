using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Payment_Gateway.API.Extensions;
using Payment_Gateway.BLL.Infrastructure.Paystack;
using Payment_Gateway.BLL.Interfaces.IServices;
using Payment_Gateway.BLL.Paystack.Interfaces;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects;
using Payment_Gateway.Shared.DataTransferObjects.Response;
using System.Net;

namespace Payment_Gateway.BLL.Implementation.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Wallet> _walletRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaystackPostRequest _paystackPostRequest;
        private readonly PaystackConfig _paystackConfig;



        public AdminServices(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IPaystackPostRequest paystackPostRequest, PaystackConfig paystackConfig)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
            _walletRepo = _unitOfWork.GetRepository<Wallet>();
            _paystackPostRequest = paystackPostRequest;
            _paystackConfig = paystackConfig;
        }


        public async Task<ServiceResponse<CheckBalanceResponse>> CheckBalance()
        {
            var result = await _paystackPostRequest.GetRequest(_paystackConfig.CheckBalanceUrl);

            if (result.IsSuccessStatusCode)
            {
                var listResponse = await result.Content.ReadAsStringAsync();
                var getResponse = JsonConvert.DeserializeObject<CheckBalanceResponse>(listResponse);

                return new ServiceResponse<CheckBalanceResponse>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = getResponse,
                    Success = true
                };

            }
            return new ServiceResponse<CheckBalanceResponse>
            {
                Message = "Could not complete action",
                StatusCode = HttpStatusCode.NotFound,
                Success = false
            };
        }


        public async Task<ServiceResponse<FetchLedgerResponse>> FetchLedger()
        {

            var result = await _paystackPostRequest.GetRequest(_paystackConfig.FetchLedgerUrl);
            if (result != null)
            {
                var ledgerResponse = await result.Content.ReadAsStringAsync();
                var getResponse = JsonConvert.DeserializeObject<FetchLedgerResponse>(ledgerResponse);

                return new ServiceResponse<FetchLedgerResponse>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = getResponse,
                    Success = true

                };
            }
            return new ServiceResponse<FetchLedgerResponse>
            {
                Message = "Could not retrieve data",
                StatusCode = HttpStatusCode.BadRequest,
                Success = false
            };
        }


        public async Task<ServiceResponse<IEnumerable<ApplicationUserDto>>> GetAllUsers()
        {
            var users = await _userRepo.GetAllAsync(include: u => u.Include(u => u.Wallet));
            if (users == null)
            {
                return new ServiceResponse<IEnumerable<ApplicationUserDto>>
                {
                    Message = "Users Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false
                };
            }

            var result = users.Select(u => new ApplicationUserDto
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                MiddleName = u.MiddleName,
                UserName = u.UserName,
                Birthday = u.Birthday.ToString(),
                ApiSecretKey = u.ApiSecretKey,
                Wallet = u.Wallet.WalletId,
            });

            return new ServiceResponse<IEnumerable<ApplicationUserDto>>
            {
                StatusCode = HttpStatusCode.NotFound,
                Success = true,
                Data = result
            };

        }


        public async Task<ServiceResponse<IList<UserDto>>> GetAllUsersWithBalance()
        {
            var user = await _userRepo.GetAllAsync(include: u => u.Include(t => t.Wallet));
            if (user == null)
            {
                return new ServiceResponse<IList<UserDto>>
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }


            var result = user.Join(await _walletRepo.GetAllAsync(),
                user => user.WalletId,
                wallet => wallet.WalletId,
                (user, wallet) => new UserDto
                {
                    WalletId = wallet.WalletId,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Birthday = user.Birthday.ToString(),
                    Balance = wallet.Balance,
                    ApiSecretKey = user.ApiSecretKey,
                }).OrderByDescending(u => u.LastName).ToList();


            return new ServiceResponse<IList<UserDto>>
            {
                StatusCode = HttpStatusCode.OK,
                Data = result,
            };
        }


        public async Task<ServiceResponse<UserDto>> GetUserDetails(string walletId)
        {
            var user = await _userRepo.GetSingleByAsync(e => e.WalletId == walletId, include: u => u.Include(u => u.Wallet));
            if (user == null)
            {
                return new ServiceResponse<UserDto>
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.NotFound,
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
            };
        }


        public async Task<ServiceResponse<UserDto>> GetUser(string walletId)
        {
            var wallet = await _userRepo.GetAllAsync(include: u => u.Include(u => u.Wallet));
               var user = wallet.Where( u=>u.WalletId == walletId).FirstOrDefault();
            if (user == null)
            {
                return new ServiceResponse<UserDto>
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.NotFound,
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


        public async Task<object> UpdateUser(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(e => e.Id.ToString() == userId, include: u => u.Include(u => u.Wallet));
            return user;
        }


        public async Task<ServiceResponse> DeleteUser(string walletId)
        {
            var user = await _userRepo.GetSingleByAsync(e => e.WalletId.ToString() == walletId);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false,
                };
            }

            await _userRepo.DeleteAsync(user);
            return new ServiceResponse
            {
                StatusCode = HttpStatusCode.OK,
                Success = true
            };
        }

    }
}
