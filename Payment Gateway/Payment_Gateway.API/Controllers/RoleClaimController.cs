using Microsoft.AspNetCore.Mvc;
using Payment_Gateway.BLL.Infrastructure;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.Shared.DataTransferObjects.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace Payment_Gateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleClaimController : ControllerBase
    {
        private readonly IRoleClaimService _userClaimsService;

        public RoleClaimController(IRoleClaimService userClaimsService)
        {
            _userClaimsService = userClaimsService;
        }



        [HttpGet("get-claims", Name = "get-claims")]
        [SwaggerOperation(Summary = "returns claims of selected role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns claim types and values", Type = typeof(RoleClaimResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = " ", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetClaims(string role)
        {
            var result = await _userClaimsService.GetUserClaims(role);
            return Ok(result);
        }



        [HttpPost("add-claim", Name = "add-claim")]
        [SwaggerOperation(Summary = "adds claim to role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns claim type and value", Type = typeof(RoleClaimResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to add claim", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> AddClaim([FromBody] RoleClaimRequest request)
        {
            var result = await _userClaimsService.AddClaim(request);
            return Ok(result);
        }



        [HttpPost("delete-claim", Name = "delete-claim")]
        [SwaggerOperation(Summary = "deletes claims")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Success")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to delete claim", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteClaim(string claimValue, string role)
        {
            await _userClaimsService.RemoveUserClaims(claimValue, role);
            return Ok();
        }
    }
}
