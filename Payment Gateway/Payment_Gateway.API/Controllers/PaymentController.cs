using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment_Gateway.API.Extensions;
using Payment_Gateway.BLL.Infrastructure;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.BLL.Interfaces.IServices;
using Payment_Gateway.BLL.Paystack.Interfaces;
using Payment_Gateway.Shared.DataTransferObjects.Request;
using Swashbuckle.AspNetCore.Annotations;

namespace Payment_Gateway.API.Controllers
{
    [Route("PayGo/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IServiceFactory _serviceFactory;

        public PaymentController(IServiceFactory serviceFactory, IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
            _serviceFactory = serviceFactory;
        }



        [AllowAnonymous]
        [HttpPost("cardPayment", Name ="card-payment")]
        [SwaggerOperation(Summary = "Makes payment using your card")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Payment successful", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Payment failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<ActionResult> MakePayment([FromBody] PaymentRequest makePayment)
        {
            var response = await _serviceFactory.GetService<IMakePaymentService>().MakePayment(makePayment);
            if (response.data.status == "success" || response != null)
            {
                string? userId = _contextAccessor.HttpContext?.User.GetUserId();
                var amount = response.data.amount;

                await _serviceFactory.GetService<IWalletService>().UpdateBlance(userId, amount);
                await _serviceFactory.GetService<IPaymentServiceExtension>().CreatePayment(userId, response);
                return Ok(response);
            }
            return BadRequest();
        }
    }
}
