using System.ComponentModel.DataAnnotations;

namespace Payment_Gateway.Shared.DataTransferObjects.Requests
{
    public class UpdateRoleClaimsDto
    {
        public string Role { get; set; }
        public string ClaimType { get; set; }
        public string NewClaim { get; set; }
    }

    public class RoleRequestDto : RequestParameters
    {
        public RoleRequestDto()
        {
            OrderBy = "Name";
        }

        public bool Active { get; set; } = false;
    }

    public class RoleDto
    {
        [Required(ErrorMessage = "Role Name cannot be empty"), MinLength(2), MaxLength(30)]
        public string Name { get; set; } = null!;
    }


    public class RoleClaimRequest
    {
        public string Role { get; set; }
        public string ClaimType { get; set; }
    }

    public class RoleClaimResponse
    {
        public string Role { get; set; }
        public string ClaimType { get; set; }
    }
}
