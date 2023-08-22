using Payment_Gateway.API.Extensions;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Response;

namespace Payment_Gateway.BLL.Interfaces.IServices
{
    public interface IEmailServices
    {
        Task<bool> VerifyEmail(string emailAddress);
        Task<bool> Execute(string email, string subject, string htmlMessage);
        Task<ServiceResponse<AuthorizationResponse>> SendEmailAsync(string subject, string message, string email);
        Task<bool> RegistrationMail(ApplicationUser user);
        Task<string> ChangePasswordMail(ApplicationUser user);

        Task<string> ResetPasswordMail(ApplicationUser user);
    }
}
