using Payment_Gateway.Models.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Payment_Gateway.Models.Entities
{
    public class ApiKey
    {
        [Key]
        public string ApiSecretKey { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
