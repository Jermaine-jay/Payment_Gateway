using Payment_Gateway.API.Extensions;
using Payment_Gateway.Shared.DataTransferObjects.Requests;

namespace Payment_Gateway.BLL.Interfaces
{
    public interface IRoleClaimService
    {
        Task<ServiceResponse<IEnumerable<RoleClaimResponse>>> GetUserClaims(string role);
        Task<ServiceResponse<RoleClaimResponse>> AddClaim(RoleClaimRequest request);
        Task<ServiceResponse> RemoveUserClaims(string claimType, string role);
        Task<ServiceResponse<RoleClaimResponse>> UpdateRoleClaims(UpdateRoleClaimsDto request);
    }
}
