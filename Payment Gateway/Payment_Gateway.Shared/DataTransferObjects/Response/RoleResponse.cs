using Payment_Gateway.Models.Entities;

namespace Payment_Gateway.Shared.DataTransferObjects.Response
{
    public class RoleResponse
    {
        public string Name { get; set; }
        public IEnumerable<ApplicationRoleClaim> Claims { get; set; }
        public bool Active { get; set; }

    }

    public class AddUserToRoleResponse
    {
        public string UserName { get; set; }
        public string? Role { get; set; }
    }
}
