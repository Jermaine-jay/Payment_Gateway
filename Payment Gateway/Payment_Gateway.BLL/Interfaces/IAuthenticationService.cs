using Payment_Gateway.API.Extensions;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Requests;
using Payment_Gateway.Shared.DataTransferObjects.Response;


namespace Payment_Gateway.BLL.Interfaces
{
    public interface IAuthenticationService
    {
		Task<ServiceResponse<AuthorizationResponse>> CreateUser(UserRegistrationRequest request);
		Task<ServiceResponse<AuthenticationResponse>> UserLogin(LoginRequest request);
        Task<ServiceResponse<AuthorizationResponse>> ConfirmEmail(string token);
        Task<ServiceResponse<ChangePasswordResponse>> ChangeUserPassword(string userId);
        Task<ServiceResponse<ChangePasswordResponse>> ChangePassword(ChangePasswordRequest request);
        Task<ServiceResponse<ChangePasswordResponse>> ForgotPassword(ForgotPasswordRequestDto model);
        Task<ServiceResponse<ChangePasswordResponse>> ResetPassword(ResetPasswordRequest request);
        Task<ApplicationUser> GetApiKey(string apiKey);
    }
}
