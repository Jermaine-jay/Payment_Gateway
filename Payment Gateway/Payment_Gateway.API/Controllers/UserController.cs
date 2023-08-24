
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Payment_Gateway.BLL.Infrastructure;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.BLL.Interfaces.IServices;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects;
using Payment_Gateway.Shared.DataTransferObjects.Response;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;


namespace Payment_Gateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;


        public UserController(UserManager<ApplicationUser> userManager, IAuthenticationService authService, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _userManager = userManager;
            _authService = authService;
            _userService = userService;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;

        }



        [AllowAnonymous]
        [HttpGet("user-balance", Name = "user-balance")]
        [SwaggerOperation(Summary = "Get user account balance")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "User balance", Type = typeof(ApplicationUserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User doesn't exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User not found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> UserBalance()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _userService.GetUserBalance(userId);
            return Ok(response);
        }




        [AllowAnonymous]
        [HttpPost("user-details", Name = "user-details")]
        [SwaggerOperation(Summary = "User transactions Details")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "UserId of created user", Type = typeof(AuthenticationResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User with provided email already exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to create user", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> UserDetails(string userId)
        {
            //var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _userService.GetUserDetails(userId);
            if(response.Success)
                return Ok(response);

            return BadRequest(response);
        }
        


        [AllowAnonymous]
        [HttpPost("user-transaction-details", Name = "user-transaction-details")]
        [SwaggerOperation(Summary = "User Details")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "UserId of created user", Type = typeof(AuthenticationResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User with provided email already exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to create user", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetTransactionsDetails(string userId)
        {
            //var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _userService.GetTransactionsDetails(userId);
            if(response.Success)
                return Ok(response);

            return BadRequest(response);
        }



        [AllowAnonymous]
        [HttpPost("all-transactions-details", Name = "all-transactions-details")]
        [SwaggerOperation(Summary = "User Transaction Details")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "UserId of created user", Type = typeof(AuthenticationResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User with provided email already exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to create user", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetTransactions(string userId)
        {
            //var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _userService.GetAllTransactions(userId);
            if(response!= null)
                return Ok(response);

            return BadRequest(response);
        }
    }
}
