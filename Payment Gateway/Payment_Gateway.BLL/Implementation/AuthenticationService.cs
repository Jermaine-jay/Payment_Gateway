using Microsoft.AspNetCore.Identity;
using Payment_Gateway.API.Extensions;
using Payment_Gateway.BLL.Infrastructure.Otp;
using Payment_Gateway.BLL.Infrastructure.Security;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.BLL.Interfaces.IServices;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Models.Enums;
using Payment_Gateway.Models.Extensions;
using Payment_Gateway.Shared.DataTransferObjects.Requests;
using Payment_Gateway.Shared.DataTransferObjects.Response;
using System.Net;
using System.Text;

namespace Payment_Gateway.BLL.Implementation
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceFactory _serviceFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationRoleClaim> _roleClaimsRepo;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly IRepository<ApiKey> _apiKeyRepo;
        private readonly IRoleService _roleService;
        private readonly RoleManager<ApplicationRole> _roleManager;



        public AuthenticationService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _serviceFactory = serviceFactory;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _roleClaimsRepo = _unitOfWork.GetRepository<ApplicationRoleClaim>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
            _apiKeyRepo = _unitOfWork.GetRepository<ApiKey>();
        }


        public async Task<ServiceResponse<AuthorizationResponse>> CreateUser(UserRegistrationRequest request)
        {
            ApplicationUser existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new ServiceResponse<AuthorizationResponse>
                {
                    Message = $"User with {request.Email} already exist",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }


            existingUser = await _userManager.FindByNameAsync(request.UserName);
            if (existingUser != null)
            {
                return new ServiceResponse<AuthorizationResponse>
                {
                    Message = $"User with UserName: {request.UserName} already exist",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            var verifyEmail = await _serviceFactory.GetService<IEmailServices>().VerifyEmail(request.Email);
            if (!verifyEmail)
            {
                return new ServiceResponse<AuthorizationResponse>
                {
                    Message = $"Email Address does not Exist",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            ApplicationUser user = new()
            {
                Email = request.Email,
                UserName = request.UserName,
                FirstName = request.Firstname,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Active = true,
                Pin = SHA256Hasher.Hash("0000"),
                Wallet = new Wallet()
                {
                    WalletId = AccountNumberGenerator.GenerateRandomNumber(),
                    Balance = 0,
                },
                ApiKey = new ApiKey(),
                UserType = UserType.User,
            };


            IdentityResult result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new ServiceResponse<AuthorizationResponse>
                {
                    Message = $"Could not create user",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }


            var registerMail = await _serviceFactory.GetService<IEmailServices>().RegistrationMail(user);
            if (!registerMail)
            {
                return new ServiceResponse<AuthorizationResponse>
                {
                    Message = $"Could not send Confirmation mail",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            //await _userManager.SetTwoFactorEnabledAsync(user, true);

            var roleName = UserType.User.GetStringValue();
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                ApplicationRole newRole = new ApplicationRole { Name = roleName };
                await _roleManager.CreateAsync(newRole);
            }

            await _userManager.AddToRoleAsync(user, roleName);
            var response = new ServiceResponse<AuthorizationResponse>
            {
                Message = "User created Sucessfully",
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Data = new AuthorizationResponse
                {
                    UserId = user.Id.ToString(),
                    ApiSecretKey = user.ApiSecretKey,
                    Succeeded = true,
                }
            };
            return response;
        }


        public async Task<ServiceResponse<AuthorizationResponse>> ConfirmEmail(string validToken)
        {
            var opslen = (validToken.Count() - 84);
            var getstring = validToken.Substring(opslen, 48);
            var getoperation = validToken.Substring(0, opslen);

            var getUserId = Convert.FromBase64String(getstring);
            var operation = Convert.FromBase64String(getoperation);

            string decodedStr = Encoding.ASCII.GetString(getUserId);
            string ops = Encoding.UTF8.GetString(operation);

            var user = await _userManager.FindByIdAsync(decodedStr);
            if (user == null)
            {
                return new ServiceResponse<AuthorizationResponse>
                {
                    Message = "User not Found",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            if (ops != OtpOperation.EmailConfirmation.ToString())
            {
                return new ServiceResponse<AuthorizationResponse>
                {
                    Message = "Wrong Operation",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }


            var verifyToken = await _serviceFactory.GetService<IOtpService>().VerifyUniqueOtpAsync(user.Id.ToString(), validToken, OtpOperation.EmailConfirmation);
            if (verifyToken)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                return new ServiceResponse<AuthorizationResponse>
                {
                    Message = "Email Address Comfirmed",
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                };
            }


            return new ServiceResponse<AuthorizationResponse>
            {
                Message = "Unable to confirm Email Address",
                StatusCode = HttpStatusCode.BadRequest,
                Success = false
            };
        }



        public async Task<ServiceResponse<AuthenticationResponse>> UserLogin(LoginRequest request)
        {
            ApplicationUser user;
            if (request.UserNameOrEmail.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(request.UserNameOrEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(request.UserNameOrEmail);
            }

            if (user == null)
            {
                return new ServiceResponse<AuthenticationResponse>
                {
                    Message = "Invalid username or password",
                    StatusCode = HttpStatusCode.BadRequest,
                };
            }

            var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (!emailConfirmed)
            {
                return new ServiceResponse<AuthenticationResponse>
                {
                    Message = "Cannot login User",
                    StatusCode = HttpStatusCode.Unauthorized,
                };
            }

            if (!user.Active)
            {
                return new ServiceResponse<AuthenticationResponse>
                {
                    Message = "User Inactive",
                    StatusCode = HttpStatusCode.BadRequest,
                };
            }


            (bool isAccountLocked, int? minutesLeft) = await _serviceFactory.GetService<IAccountLockoutService>().IsAccountLocked(user.Id.ToString());
            if (isAccountLocked)
            {
                return new ServiceResponse<AuthenticationResponse>
                {
                    Message = $"User account already locked, hence sign in was skipped, retry in {minutesLeft} mins",
                    StatusCode = HttpStatusCode.Forbidden,
                };
            }


            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordCorrect)
            {
                await _serviceFactory.GetService<IAccountLockoutService>().RecordFailedLoginAttempt(user.Id.ToString());
                return new ServiceResponse<AuthenticationResponse>
                {
                    Message = "Invalid username or password",
                    StatusCode = HttpStatusCode.NotFound,
                };
            };


            await _serviceFactory.GetService<IAccountLockoutService>().RecordSuccessfulLoginAttempt(user.Id.ToString());
            JwtToken userToken = await _serviceFactory.GetService<IJWTAuthenticator>().GenerateJwtToken(user);

            string? userType = user.UserType.GetStringValue();
            string fullName = string.IsNullOrWhiteSpace(user.MiddleName)
                ? $"{user.LastName} {user.FirstName}"
                : $"{user.LastName} {user.FirstName} {user.MiddleName}";

            return new ServiceResponse<AuthenticationResponse>
            {
                Message = "Login Successfull",
                StatusCode = HttpStatusCode.OK,
                Data = new AuthenticationResponse
                {
                    JwtToken = userToken,
                    FullName = fullName,
                    UserId = user.Id.ToString(),
                }
            };


            /* List<Claim> userClaims = (await _userManager.GetClaimsAsync(user)).ToList();
             List<string> userRoles = (await _userManager.GetRolesAsync(user)).ToList();

             IEnumerable<Claim> roleClaims = await _roleClaimsRepo.GetQueryable().Include(x => x.AppRole)
                 .Where(r => userRoles.Any(u => u == r.AppRole.Name)).Select(s => s.ToClaim()).ToListAsync();
             userClaims.AddRange(roleClaims);*/

        }



        public async Task<ServiceResponse<ChangePasswordResponse>> ChangeUserPassword(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (string.IsNullOrEmpty(user.Id.ToString()))
            {
                return new ServiceResponse<ChangePasswordResponse>
                {
                    Message = "User does not exist",
                    StatusCode = HttpStatusCode.BadGateway,
                    Data = new ChangePasswordResponse
                    {
                        Success = false,
                    }
                };
            }


            await _serviceFactory.GetService<IEmailServices>().ChangePasswordMail(user);
            return new ServiceResponse<ChangePasswordResponse>
            {
                Message = "Sent token to your Email Address",
                StatusCode = HttpStatusCode.OK,
                Data = new ChangePasswordResponse
                {
                    Success = true,
                }
            };
        }



        public async Task<ServiceResponse<ChangePasswordResponse>> ChangePassword(ChangePasswordRequest request)
        {

            ApplicationUser user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return new ServiceResponse<ChangePasswordResponse>
                {
                    Message = "User does not exist",
                    StatusCode = HttpStatusCode.NotFound,
                    Data = new ChangePasswordResponse
                    {
                        Success = false,
                    }
                };
            }


            bool isOtpValid = await _serviceFactory.GetService<IOtpService>().VerifyOtpAsync(user.Id.ToString(), request.Token, OtpOperation.ChangePassword);
            if (!isOtpValid)
            {
                return new ServiceResponse<ChangePasswordResponse>
                {
                    Message = "Invalid Token",
                    StatusCode = HttpStatusCode.BadRequest,
                    Data = new ChangePasswordResponse
                    {
                        Success = false,
                    }
                };
            }

            await _userManager.ChangePasswordAsync(user, request.NewPassword, request.CurrentPassword);
            return new ServiceResponse<ChangePasswordResponse>
            {
                Message = "Password changed Sucessfully",
                StatusCode = HttpStatusCode.OK,
                Data = new ChangePasswordResponse
                {
                    Success = true,
                }
            };
        }



        public async Task<ServiceResponse<ChangePasswordResponse>> ForgotPassword(ForgotPasswordRequestDto model)
        {

            var verify = await _serviceFactory.GetService<IEmailServices>().VerifyEmail(model.Email);
            if (verify == false)
            {
                return new ServiceResponse<ChangePasswordResponse>
                {
                    Message = "Invalid Email Address",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }


            var user = await _userManager.FindByEmailAsync(model.Email);
            var isConfrimed = await _userManager.IsEmailConfirmedAsync(user);
            if (user == null || !isConfrimed)
            {
                return new ServiceResponse<ChangePasswordResponse>
                {
                    Message = "User does not exist",
                    StatusCode = HttpStatusCode.BadRequest,
                };
            }


            var result = await _serviceFactory.GetService<IEmailServices>().ResetPasswordMail(user);
            return new ServiceResponse<ChangePasswordResponse>
            {
                Message = "Reset Password Email Sent",
                StatusCode = HttpStatusCode.OK,
                Data = new ChangePasswordResponse
                {
                    Message = "Token sent",
                    Code = result,
                }
            };
        }



        public async Task<ServiceResponse<ChangePasswordResponse>> ResetPassword(ResetPasswordRequest request)
        {
            var opslen = (request.Token.Count() - 84);
            var getstring = request.Token.Substring(opslen, 48);
            var getoperation = request.Token.Substring(0, opslen);

            var getUserId = Convert.FromBase64String(getstring);
            var operation = Convert.FromBase64String(getoperation);

            string decodedStr = Encoding.ASCII.GetString(getUserId);
            string ops = Encoding.UTF8.GetString(operation);

            ApplicationUser user = await _userManager.FindByIdAsync(decodedStr);
            if (user == null || !user.EmailConfirmed)
            {
                return new ServiceResponse<ChangePasswordResponse>
                {
                    Message = "User does not exist",
                    StatusCode = HttpStatusCode.BadRequest,

                };
            }

            if (ops != OtpOperation.PasswordReset.ToString())
            {
                return new ServiceResponse<ChangePasswordResponse>
                {
                    Message = "Wrong Operation",
                    StatusCode = HttpStatusCode.BadRequest,

                };
            }

            bool isOtpValid = await _serviceFactory.GetService<IOtpService>().VerifyUniqueOtpAsync(user.Id.ToString(), request.Token, OtpOperation.PasswordReset);
            if (!isOtpValid)
            {
                return new ServiceResponse<ChangePasswordResponse>
                {
                    Message = "Invalid Token",
                    StatusCode = HttpStatusCode.BadRequest,

                };
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, request.NewPassword, request.ConfirmPassword);
            if (!result.Succeeded)
            {
                return new ServiceResponse<ChangePasswordResponse>
                {
                    Message = "Could not complete Operation",
                    StatusCode = HttpStatusCode.BadRequest,
                };
            }

            return new ServiceResponse<ChangePasswordResponse>
            {
                Message = "Password Reset Completed",
                StatusCode = HttpStatusCode.OK,
            };
        }



        public async Task<AuthenticationResponse> ConfirmTwoFactorToken(TwoFactorLoginRequest request)
        {
            /* ApplicationUser user = await _userManager.FindByIdAsync(request.ApplicationUser.Id.ToString("d"));

             if (user == null)
                 throw new InvalidOperationException("Invalid user");

             bool result = await _userManager.VerifyTwoFactorTokenAsync(user,
                 _userManager.Options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultEmailProvider, request.Token);

             if (!result)
                 throw new InvalidOperationException("Invalid token");

             JwtToken userToken = await GetTokenAsync(user);

             List<Claim> userClaims = (await _userManager.GetClaimsAsync(user)).ToList();
             List<string> userRoles = (await _userManager.GetRolesAsync(user)).ToList();

             IEnumerable<Claim> roleClaims = await _roleClaimsRepo.GetQueryable().Include(x => x.RoleId)
                 .Where(r => userRoles.Contains(r.)).Select(s => s.ToClaim()).ToListAsync();

             userClaims.AddRange(roleClaims);

             List<string> claims = userClaims.Select(x => x.Value).ToList();

             string? userType = user.UserType.GetStringValue();

             string fullName = string.IsNullOrWhiteSpace(user.MiddleName)
                 ? $"{user.LastName} {user.FirstName}"
                 : $"{user.LastName} {user.FirstName} {user.MiddleName}";

             bool? birthday = null;

             if (user.UserType == UserType.User && user.Birthday?.Date == DateTime.Now)
             {
                 birthday = true;
             }

             //IEnumerable<string> menuItems = await _serviceFactory.GetService<IMenuService>().GetMenuItems(claims);

             //Log.ForContext(new PropertyBagEnricher().Add("LoginResponse",
             //    new LoggedInUserResponse
             //    { FullName = fullName, UserType = userType, UserId = user.Id },
             //    destructureObject: true)).Information("Login Successful");

             return new AuthenticationResponse
             {
                 JwtToken = userToken,
                 UserType = userType,
                 FullName = fullName,
                 //MenuItems = menuItems,
                 TwoFactor = true
             };*/

            throw new InvalidOperationException();



        }


        public async Task<string> ChangeEmail(ChangeEmailRequest request)
        {
            //string? _userId = _contextAccessor.HttpContext?.User.GetUserId();

            /*if (_userId != null)
                return await SaveChangedEmail(_userId, decodedNewEmail, decodedToken);*/

            throw new InvalidOperationException("Recovery email not found.");
        }


        public async Task UpdateRecoveryEmail(string userId, string email)
        {

            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found!");
            }

            user.RecoveryMail = email;
            await _userManager.UpdateAsync(user);

            //Log.ForContext(new PropertyBagEnricher().Add("RecoveryEmail", email))
            //    .Information("Recovery Mail Updated Successfully");
        }



        public async Task ToggleUserActivation(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException($"The user with {nameof(userId)}: {userId} doesn't exist in the database.");
            }
            user.Active = !user.Active;

            await _userManager.UpdateAsync(user);

            //Log.ForContext(new PropertyBagEnricher().Add("ToggleState", user.Active)
            //).Information("User activation toggle successful");
        }



        public async Task<ApplicationUser> GetApiKey(string apiKey)
        {
            var getUser = await _userRepo.GetAllAsync();
            var user = getUser.Where(u => u.ApiSecretKey == apiKey).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            return user;
        }

    }
}
