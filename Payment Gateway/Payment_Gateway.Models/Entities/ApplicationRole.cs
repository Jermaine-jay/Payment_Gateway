using Microsoft.AspNetCore.Identity;
using Payment_Gateway.Models.Enums;

namespace Payment_Gateway.Models.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; } = true;
        public UserType Type { get; set; }
        public virtual IEnumerable<ApplicationRoleClaim> RoleClaims { get; set; }
    }
}