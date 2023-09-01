using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Payment_Gateway.BLL.Infrastructure;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Requests;
using Payment_Gateway.Shared.DataTransferObjects.Response;
using Swashbuckle.AspNetCore.Annotations;

namespace Payment_Gateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "Authorization")]

    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleservice;


        public RoleController(IRoleService roleservice)
        {
            _roleservice = roleservice;
        }



        [AllowAnonymous]
        [HttpPost("CreateRole", Name = "Create-Role")]
        [SwaggerOperation(Summary = "Creates role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Role", Type = typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Role already exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to create role", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateRoleAync([FromBody] RoleDto request)
        {
            var response = await _roleservice.CreateRoleAync(request);
            return Ok(response);
        }



        [AllowAnonymous]
        [HttpPost("Add-user-role", Name = "Add-user-role")]
        [SwaggerOperation(Summary = "add user to role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Role", Type = typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Role already exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to create role", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateUserRoleAync([FromBody] AddUserToRoleRequest request)
        {
            var response = await _roleservice.AddUserToRole(request);
            return Ok(response);
        }



        [AllowAnonymous]
        [HttpPut("Remove-user-Role", Name = "Remove-user-role")]
        [SwaggerOperation(Summary = "remove user from role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Role", Type = typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Role already exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to create role", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> RemoveUserRoleAync([FromBody] AddUserToRoleRequest request)
        {
            var response = await _roleservice.RemoveUserFromRole(request);
            return Ok(response);
        }



        [AllowAnonymous]
        [HttpPut("EditRole", Name = "EditRole")]
        [SwaggerOperation(Summary = "edit role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Role", Type = typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Role does not exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to edit role", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> EditRoleAsync(string id, string Name)
        {
            var response = await _roleservice.EditRole(id, Name);
            return Ok(response);
        }



        [AllowAnonymous]
        [HttpDelete("DeleteRole", Name = "DeleteRole")]
        [SwaggerOperation(Summary = "Delete role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Role", Type = typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Role does not exists", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to delete role", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteRoleAync(string Name)
        {
            var response = await _roleservice.DeleteRole(Name);
            return Ok(response);
        }



        [AllowAnonymous]
        [HttpGet("GetRoles", Name = "GetRoles")]
        [SwaggerOperation(Summary = "All roles")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Role", Type = typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = " ", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Empty", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> AllRolesAync()
        {
            var response = await _roleservice.GetAllRoles();
            return Ok(response);
        }



        [AllowAnonymous]
        [HttpGet("GetUserRoles", Name = "GetUserRoles")]
        [SwaggerOperation(Summary = "Get user Rles")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Roles")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = " ", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Empty", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetUserRolesAync(string Username)
        {
            var response = await _roleservice.GetUserRoles(Username);
            if (response.Success)
                return Ok(response.Data);

            return BadRequest(response.Data);
        }


    }
}
