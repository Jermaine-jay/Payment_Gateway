using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json;
using Payment_Gateway.API.Extensions;
using Payment_Gateway.BLL.Infrastructure.EmailSender;
using Payment_Gateway.BLL.Infrastructure.GenerateEmailPage;
using Payment_Gateway.BLL.Infrastructure.Otp;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.BLL.Interfaces.IServices;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Response;
using System.Net;



namespace Payment_Gateway.BLL.Implementation.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly EmailSenderOptions _emailSenderOptions;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceFactory _serviceFactory;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private HttpClient _httpClient;
        private ZeroBounceConfig _zeroBounceConfig;



        public EmailServices(IServiceFactory serviceFactory, UserManager<ApplicationUser> userManager, EmailSenderOptions optionsAccessor, ZeroBounceConfig zeroBounceConfig)
        {
            _serviceFactory = serviceFactory;
            _userManager = userManager;
            _configuration = _serviceFactory.GetService<IConfiguration>();
            _unitOfWork = _serviceFactory.GetService<IUnitOfWork>();
            _userRepo = _unitOfWork?.GetRepository<ApplicationUser>();
            _emailSenderOptions = optionsAccessor;
            _zeroBounceConfig = zeroBounceConfig;
        }


        public async Task<ServiceResponse<AuthorizationResponse>> SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(_emailSenderOptions.Password))
            {
                return new ServiceResponse<AuthorizationResponse>
                {
                    Message = "could not send emali",
                    StatusCode = HttpStatusCode.BadRequest,
                };
            }

            await Execute(email, subject, message);

            return new ServiceResponse<AuthorizationResponse>
            {
                Message = "Email Sent",
                StatusCode = HttpStatusCode.OK,
            };
        }


        public async Task<bool> Execute(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("PayGo", _emailSenderOptions.Username));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlMessage;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {

                client.Connect(_emailSenderOptions.SmtpServer, _emailSenderOptions.Port, true);
                client.Authenticate(_emailSenderOptions.Email, _emailSenderOptions.Password);
                client.Send(message);
                client.Disconnect(true);
            }

            return true;
        }



        public async Task<bool> VerifyEmail(string emailAddress)
        {

            using (var httpClient = new HttpClient())
            {
                var parameters = $"api_key={_zeroBounceConfig.ApiKey}&email={emailAddress}";
                var response = await httpClient.GetAsync($"{_zeroBounceConfig.Url}?{parameters}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var getResponse = JsonConvert.DeserializeObject<dynamic>(responseContent).status;
                if (getResponse == "valid")
                {
                    return true;
                }
                return false;
            }
        }


        public async Task<bool> RegistrationMail(ApplicationUser user)
        {
            var page = _serviceFactory.GetService<IGenerateEmailPage>().EmailVerificationPage;
            var context = _serviceFactory.GetService<IHttpContextAccessor>().HttpContext;
            var link = _serviceFactory.GetService<LinkGenerator>();

            var validToken = await _serviceFactory.GetService<IOtpService>().GenerateUnoqueOtpAsync(user.Id.ToString(), OtpOperation.EmailConfirmation);
            //string apUrl = $"{_configuration["AppUrl"]}/api/Auth/ConfirmEmail?userId={user.Id}&token={validToken}";
            
            var callbackUrl = link.GetUriByAction(context, "ConfirmEmail", "Auth", new { userId = user.Id.ToString(), validToken });

            //string url = $"<p>Click <a href='{apUrl}'>here</a> to reset your password.</p>";
            await SendEmailAsync(user.Email, "Confirm your email", page(user.UserName, validToken));

            return true;
        }


        public async Task<string> ChangePasswordMail(ApplicationUser user)
        {
            var page = _serviceFactory.GetService<IGenerateEmailPage>().ChangePasswordPage;
            var validToken = await _serviceFactory.GetService<IOtpService>().GenerateUnoqueOtpAsync(user.Id.ToString(), OtpOperation.ChangePassword);

            await SendEmailAsync(user.Email, "Change Password", page(validToken));

            return validToken;
        }


        public async Task<string> ResetPasswordMail(ApplicationUser user)
        {
            var validToken = await _serviceFactory.GetService<IOtpService>().GenerateUnoqueOtpAsync(user.Id.ToString(), OtpOperation.PasswordReset);
            string apUrl = $"{_configuration["AppUrl"]}/api/Auth/ResetPassword?Token={validToken}";

            string url = $"<p>Click <a href='{apUrl}'>here</a> to reset your password.</p>";
            var page = _serviceFactory.GetService<IGenerateEmailPage>().PasswordResetPage(url);

            await _serviceFactory.GetService<IEmailServices>().SendEmailAsync(user.Email, "Reset Password", page);

            return validToken;
        }
    }
}
