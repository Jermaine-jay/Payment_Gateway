using Payment_Gateway.API.Extensions;
using Payment_Gateway.Shared.DataTransferObjects.Requests;
using Payment_Gateway.Shared.DataTransferObjects.Response;

namespace Payment_Gateway.BLL.Interfaces
{
    public interface IRoleService
    {
        Task<ServiceResponse<AddUserToRoleResponse>> AddUserToRole(AddUserToRoleRequest request);
        Task<ServiceResponse<RoleDto>> CreateRoleAync(RoleDto request);
        Task<ServiceResponse> DeleteRole(string name);
        Task<ServiceResponse> EditRole(string id, string Name);
        Task<ServiceResponse> RemoveUserFromRole(AddUserToRoleRequest request);
        Task<ServiceResponse<IEnumerable<string>>> GetUserRoles(string userName);
        Task<IEnumerable<RoleResponse>> GetAllRoles();

        //Task<PagedResponse<RoleResponse>> GetAllRoles(RoleRequestDto request);
        //Task<IEnumerable<MenuClaimsResponse>> GetRoleClaims(string roleName);
        //Task UpdateRoleClaims(UpdateRoleClaimsDto request);
    }
}
