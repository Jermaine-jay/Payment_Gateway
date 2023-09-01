using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment_Gateway.BLL.Infrastructure;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.Shared.DataTransferObjects.Requests;
using Payment_Gateway.Shared.DataTransferObjects.Response;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Payment_Gateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Authorization")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;



        public AuthController(IAuthenticationService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;

        }


        [AllowAnonymous]
        [HttpPost("CreateUser", Name = "Create-New-User")]
        [SwaggerOperation(Summary = "Creates user")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "UserId of created user", Type = typeof(AuthorizationResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User with provided email already exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to create user", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateUser([FromBody] UserRegistrationRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _authService.CreateUser(request);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response);
            }
            return BadRequest();
        }



        [AllowAnonymous]
        [HttpPost("login", Name = "Login")]
        [SwaggerOperation(Summary = "Authenticates user")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "returns user Id", Type = typeof(AuthenticationResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid username or password", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.UserLogin(request);
            if (ModelState.IsValid)
            {
                if (response.Success)
                    return Ok(response);

                return BadRequest(response);
            }
            return BadRequest();
        }



        [AllowAnonymous]
        [HttpPost("ConfirmEmail", Name = "Confirm Email Address")]
        [SwaggerOperation(Summary = "Authenticates user")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "returns user Id", Type = typeof(AuthorizationResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid username or password", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<ActionResult<AuthorizationResponse>> ConfirmEmail(string validToken)
        {
            var response = await _authService.ConfirmEmail(validToken);
            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }



        [HttpPost("ChangeUserPassword", Name = "ChangeUserPassword")]
        [SwaggerOperation(Summary = "Change User password")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "returns email confirmation", Type = typeof(ChangePasswordResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid User", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<ActionResult<object>> ChangeUserPassword()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _authService.ChangeUserPassword(userId);
            if (response.Data.Success)
                return Ok(response);

            return BadRequest(response);
        }



        [HttpPut("ChangePassword", Name = "ChangePassword")]
        [SwaggerOperation(Summary = "Change User password")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "returns email confirmation", Type = typeof(ChangePasswordResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid User", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                request.UserId = userId;
                var response = await _authService.ChangePassword(request);
                if (response.Data.Success)
                    return Ok(response);

                return BadRequest(response);
            }
            return BadRequest("Some properties are not valid");
        }




        [HttpPost("ForgotPassword", Name = "ForgotPassword")]
        [SwaggerOperation(Summary = "Reset User password")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Sends a Reset password Mail", Type = typeof(ChangePasswordResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid User", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<ActionResult<object>> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            if (ModelState.IsValid)
            {
                var response = await _authService.ForgotPassword(request);
                if (response.Data.Success)
                    return Ok(response);

                return BadRequest(response);
            }
            return BadRequest("Some properties are not valid");

        }



        [HttpPut("ResetPassword", Name = "ResetPassword")]
        [SwaggerOperation(Summary = "Reset User password")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Resets a user password", Type = typeof(ChangePasswordResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid User", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<ActionResult<object>> ResetUserPassword([FromBody] ResetPasswordRequest request)
        {
            if (ModelState.IsValid)
            {

                var response = await _authService.ResetPassword(request);
                if (response.Data.Success)
                    return Ok(response);

                return BadRequest(response);
            }
            return BadRequest("Some properties are not valid");
        }
    }
}
