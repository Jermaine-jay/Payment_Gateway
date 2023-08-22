using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment_Gateway.Models.Entities
{
    public class AdminProfile : BaseUserProfile
    {
        public string Address { get; set; }
        public string Role { get; set; } = "Admin";

        [ForeignKey(nameof(Admin))]
        public int AdminIdentity { get; set; }
        public Admin Admin { get; set; }
    }
}
