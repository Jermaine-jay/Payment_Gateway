using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Models.Enums;
using Payment_Gateway.Models.Extensions;



namespace Payment_Gateway.DAL.Seeds
{
    public static class DatabaseUserSeeder
    {
        public static async Task SeededUserAsync(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                RoleManager<ApplicationRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                if (!await userManager.Users.AnyAsync())
                {
                    foreach (ApplicationUser user in GetUsers())
                    {
                        await userManager.CreateAsync(user, user.PasswordHash);
                    }
                }

                ApplicationUser user1 = await userManager.FindByNameAsync("Jota10");
                ApplicationUser user2 = await userManager.FindByNameAsync("Jermaine10");
                ApplicationUser user3 = await userManager.FindByNameAsync("Salah10");
                ApplicationUser user4 = await userManager.FindByNameAsync("Konate10");

                var User = UserType.User.GetStringValue();
                var Admin = UserType.Admin.GetStringValue();
                var SuperAdmin = UserType.SuperAdmin.GetStringValue();
                var ThirdParty = UserType.ThirdParty.GetStringValue();

                if (user1 != null)
                {
                    await userManager.AddToRoleAsync(user1, User);
                }

                if (user2 != null)
                {
                    await userManager.AddToRolesAsync(user2, new[] {Admin});
                }

                if (user3 != null)
                {
                    await userManager.AddToRolesAsync(user3, new[] { SuperAdmin });
                } 

                if (user4 != null)
                {
                    await userManager.AddToRolesAsync(user4, new[] {ThirdParty});
                }
            }
        }



        private static ICollection<ApplicationUser> GetUsers()
        {
            return new List<ApplicationUser>()
            {
                new ApplicationUser
                {
                    UserName = "Jota10",
                    FirstName = "Jota",
                    LastName = "Diogo",
                    Email = "jermaine.jay00@gmail.com",
                    PhoneNumber = "1234567890",
                    PasswordHash = "12345qwert",
                    Active = true,
                    Pin = SHA256Hasher.Hash("0000"),
                    Wallet = new Wallet()
                    {
                        WalletId = AccountNumberGenerator.GenerateRandomNumber(),
                        Balance = 0,
                    },
                    ApiKey = new ApiKey(),
                    UserType = UserType.User,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                },

                 new ApplicationUser
                {
                    UserName = "Salah10",
                    FirstName = "Mo",
                    LastName = "Salah",
                    Email = "jsonosita@outlook.com",
                    PhoneNumber = "1334447880",
                    PasswordHash = "12345qwert",
                    Active = true,
                    Pin = SHA256Hasher.Hash("0000"),
                    Wallet = new Wallet()
                    {
                        WalletId = AccountNumberGenerator.GenerateRandomNumber(),
                        Balance = 0,
                    },
                    ApiKey = new ApiKey(),
                    UserType = UserType.SuperAdmin,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                },

                new ApplicationUser
                {
                    UserName = "Jermaine10",
                    FirstName = "Roberto",
                    LastName = "Firmino",
                    Email = "jsonosii097@gmail.com",
                    PhoneNumber = "1234447890",
                    PasswordHash = "12345qwert",
                    Active = true,
                    Pin = SHA256Hasher.Hash("0000"),
                    Wallet = new Wallet()
                    {
                        WalletId = AccountNumberGenerator.GenerateRandomNumber(),
                        Balance = 0,
                    },
                    ApiKey = new ApiKey(),
                    UserType = UserType.Admin,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                },

                new ApplicationUser
                {
                    UserName = "Konate10",
                    FirstName = "Ibou",
                    LastName = "Konate",
                    Email = "IbouKonate@gmail.com",
                    PhoneNumber = "1223344789",
                    PasswordHash = "12345qwert",
                    Active = true,
                    Pin = SHA256Hasher.Hash("0000"),
                    Wallet = new Wallet()
                    {
                        WalletId = AccountNumberGenerator.GenerateRandomNumber(),
                        Balance = 0,
                    },
                    ApiKey = new ApiKey(),
                    UserType = UserType.ThirdParty,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                }
            };
        }
    }
}

