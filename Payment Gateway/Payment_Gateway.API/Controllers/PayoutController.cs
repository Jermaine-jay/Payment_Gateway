using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment_Gateway.API.Extensions;
using Payment_Gateway.BLL.Infrastructure;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.BLL.Interfaces.IServices;
using Payment_Gateway.BLL.Paystack.Interfaces;
using Payment_Gateway.Shared.DataTransferObjects.Request;
using Payment_Gateway.Shared.DataTransferObjects.Response;
using Swashbuckle.AspNetCore.Annotations;

namespace Payment_Gateway.API.Controllers
{
    [Authorize(AuthenticationSchemes = "ApiKeyAuthorization")]
    [Route("api/[controller]")]
    [ApiController]
    public class PayoutController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IServiceFactory _serviceFactory;


        public PayoutController(IServiceFactory serviceFactory, IHttpContextAccessor httpContextAccessor)
        {

            _contextAccessor = httpContextAccessor;
            _serviceFactory = serviceFactory;
        }



        //[Route("list-bank")]
        [HttpGet("available-banks", Name ="available-banks")]
        [SwaggerOperation(Summary = "Select preferred bank")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Payment successful", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Payment failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<ActionResult<object>> ListBanks(string Currency)
        {

            ListBankResponse response = await _serviceFactory.GetService<IPayoutService>().ListBanks(Currency);
            return Ok(response);
        }



        //[Authorize]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Route("resolve-accountnumber")]
        [HttpGet("check-account", Name = "check-account")]
        [SwaggerOperation(Summary = "Resolve account using PayGo")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Operation successful", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Operation failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<ActionResult> ResolveAccountNumber([FromQuery] ResolveAccountNumberRequest request)
        {
            ResolveBankResponse response = await _serviceFactory.GetService<IPayoutService>().ResolveAccountNumber(request);
            return Ok(response);
        }




        //[Route("resolve-accountnumber")]
        [HttpPost("Create-recipient", Name = "create-recipient")]
        [SwaggerOperation(Summary = "Resolve account using PayGo")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Operation successful", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Operation failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<ActionResult> CreateTransferRecipient(CreateRecipientRequest request)
        {
            CreateRecipientResponse response = await _serviceFactory.GetService<IPayoutService>().CreateTransferRecipient(request);
            return Ok(response);
        }



        //[Route("initiate-transfer")]
        [HttpPost("make-transfer", Name = "make-transfer")]
        [SwaggerOperation(Summary = "Makes payment using paystack")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Payment successful", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Payment failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<ActionResult> InitiateTransfer([FromBody] InitiateTransferRequest initiateTransfer)
        {
            TransferResponse response = await _serviceFactory.GetService<IPayoutService>().InitiateTransfer(initiateTransfer);
            string? userId = _contextAccessor?.HttpContext?.User?.GetUserId();

            var balRes = await _serviceFactory.GetService<IWalletService>().CheckBalance(userId, initiateTransfer.AmountInKobo);
            if (response != null && balRes.Status)
            {
                await _serviceFactory.GetService<IPayoutServiceExtension>().CreatePayout(userId, response);
                await _serviceFactory.GetService<IPayoutServiceExtension>().DebitTransfee(userId, response);
                return Ok(response);
            }
            return BadRequest(balRes);
        }



       
        //[Route("finalize-transfer")]
        [HttpPost("finalize-payment", Name = "finalize-payment")]
        [SwaggerOperation(Summary = "Makes payment using paystack")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Payment successful", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Payment failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<ActionResult> FinilizeTransfer(string pin, string transferIdOrCode)
        {
            FinalizeTransferResponse response = await _serviceFactory.GetService<IPayoutService>().FinilizeTransfer(transferIdOrCode);
            string? userId = _contextAccessor.HttpContext?.User.GetUserId();

            var res = await _serviceFactory.GetService<IPayoutServiceExtension>().CompleteTransfer(userId, pin, response);
            return Ok(res);
        }

    }
}
