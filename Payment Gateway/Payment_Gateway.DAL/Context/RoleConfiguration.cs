using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Payment_Gateway.DAL.Context;
using Payment_Gateway.Models.Entities;

namespace Payment_Gateway.DAL
{
    public static class RoleSeeder
    {
        public static async Task SeedRole(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                PaymentGatewayDbContext context = scope.ServiceProvider
                    .GetRequiredService<PaymentGatewayDbContext>();

                context.Database.EnsureCreated();
                var roleExist = context.Roles.Any();

                if (!roleExist)
                {
                    context.Roles.AddRange(SeededRoles());
                    context.SaveChanges();
                }
            }
        }

        private static IEnumerable<ApplicationRole> SeededRoles()
        {
            return new List<ApplicationRole>()
            {

                 new ApplicationRole
                 {
                     Id = Guid.NewGuid(),
                     Name = "Admin",
                     NormalizedName = "ADMIN"
                 },
                 new ApplicationRole
                 {
                     Id = Guid.NewGuid(),
                     Name = "User",
                     NormalizedName = "USER"
                 },
                 new ApplicationRole
                 {

                     Id = Guid.NewGuid(),
                     Name = "ThirdParty",
                     NormalizedName = "THIRDPARTY"
                 },
                 new ApplicationRole
                 {

                     Id = Guid.NewGuid(),
                     Name = "SuperAdmin",
                     NormalizedName = "SUPERADMIN"
                 }
            };
        }
    }
}