using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Payment_Gateway.DAL.Context;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;

namespace Payment_Gateway.DAL.Seeds
{
    public static class DatabaseDataSeeder
    {

        public static async Task EnsurePopulatedAsync(this IApplicationBuilder app)
        {
            PaymentGatewayDbContext context = app.ApplicationServices.CreateScope().ServiceProvider
                .GetRequiredService<PaymentGatewayDbContext>();


            using (var scope = app.ApplicationServices.CreateScope())
            {
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                ApplicationUser jo = await userManager.FindByNameAsync("Jota10");
                ApplicationUser jer = await userManager.FindByNameAsync("Jermaine10");
                ApplicationUser sah = await userManager.FindByNameAsync("Salah10");
                ApplicationUser ibou = await userManager.FindByNameAsync("Konate10");


                IUnitOfWork work = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                if (!await context.TransactionHistory.AnyAsync())
                {

                    await context.TransactionHistory.AddRangeAsync(TransactionHistory(jo));
                    await context.TransactionHistory.AddRangeAsync(TransactionHistory2(jer));
                    await context.TransactionHistory.AddRangeAsync(TransactionHistory3(sah));
                    await context.TransactionHistory.AddRangeAsync(TransactionHistory3(ibou));
                    await context.SaveChangesAsync();
                }
            }
        }


        private static ICollection<TransactionHistory> TransactionHistory(ApplicationUser wallet)
        {
            return new List<TransactionHistory>()
            {
                new TransactionHistory()
                {
                    WalletId = wallet.WalletId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    DebitTransactionList = new List<Payout>
                    {
                        new Payout()
                        {

                            payoutId = "00001",
                            Amount = 5000,
                            Reason = "Bought Ps5",
                            Recipient = "John Creator",
                            Reference = Guid.NewGuid().ToString(),
                            Currency = "NGN",
                            Source = "Account Balance",
                            Status = "Successful",
                            Responsestatus = true,
                            CreatedAt = DateTime.Now.ToString(""),
                            WalletId = wallet.WalletId,
                        },

                        new Payout()
                        {

                            payoutId = "00002",
                            Amount = 10000,
                            Reason = "Bought a ferrari",
                            Recipient = "Chef Dave",
                            Reference = Guid.NewGuid().ToString(),
                            Currency = "NGN",
                            Source = "Account Balance",
                            Status = "Successful",
                            Responsestatus = true,
                            CreatedAt = DateTime.Now.ToString(""),
                            WalletId = wallet.WalletId,
                        },

                        new Payout()
                        {

                            payoutId = "00003",
                            Amount = 15000,
                            Reason = "Bought a private jet",
                            Recipient = "Ezeh Livinus",
                            Reference = Guid.NewGuid().ToString(),
                            Currency = "NGN",
                            Source = "Account Balance",
                            Status = "Failed",
                            Responsestatus = false,
                            CreatedAt = DateTime.Now.ToString(""),
                            WalletId = wallet.WalletId,
                        },

                        new Payout()
                        {

                            payoutId = "00004",
                            Amount = 15000,
                            Reason = "Bought a chopper",
                            Recipient = "Ezeh Livinus",
                            Reference = Guid.NewGuid().ToString(),
                            Currency = "NGN",
                            Source = "Account Balance",
                            Status = "Failed",
                            Responsestatus = false,
                            CreatedAt = DateTime.Now.ToString(""),
                            WalletId= wallet.WalletId,
                        },

                    },
                    CreditTransactionList = new List<Payin>
                   {
                       new Payin
                       {

                           Transactionid = "00551",
                           Amount = 10000,
                           UserId = "ezeh livinus",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "johnCreator@gmail.com",
                           AccountName = "Ezeh Livinus",
                           Bank = "Zenith Bank",
                           Status = "Successful",
                           GatewayResponse = "Resp123098",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "246810",
                           CardType = "Master Card",
                           Channel = "Card",
                           WalletId = wallet.WalletId,
                       },

                       new Payin
                       {

                           Transactionid = "000432",
                           Amount = 16000,
                           UserId = "chef dave",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "EzehLivi@gmail.com",
                           AccountName = "Chef Dave",
                           Bank = "Wema Bank",
                           Status = "Successful",
                           GatewayResponse = "Respo12309",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "334455",
                           CardType = "Master Card",
                           Channel = "Card",
                           WalletId = wallet.WalletId
                       },

                       new Payin
                       {

                           Transactionid = "006603",
                           Amount = 10000,
                           UserId = "Ben Ezeh",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "BelloSaliugmail.com",
                           AccountName = "Ben Ezeh",
                           Bank = "GTB Bank",
                           Status = "Failed",
                           GatewayResponse = "Resp09876",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "246810",
                           CardType = "Visa Card",
                           Channel = "Card",
                           WalletId = wallet.WalletId
                       },

                       new Payin
                       {

                           Transactionid = "056701",
                           Amount = 10000,
                           UserId = "Emelogu Ikechukwu",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "EmeloguIkechukwu@gmail.com",
                           AccountName = "Emelogu Ikechukwu",
                           Bank = "Access Bank Plc",
                           Status = "Successful",
                           GatewayResponse = "Resp123098",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "246810",
                           CardType = "Verve Card",
                           Channel = "Card",
                           WalletId= wallet.WalletId
                       },
                   }
                },

            };
        }


        private static ICollection<TransactionHistory> TransactionHistory2(ApplicationUser applicationUser)
        {
            return new List<TransactionHistory>()
            {

                new TransactionHistory()
                {

                    WalletId = applicationUser.WalletId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    DebitTransactionList = new List<Payout>
                    {
                        new Payout()
                        {

                            payoutId = "00001",
                            Amount = 50000,
                            Reason = "Bought shoes",
                            Recipient = "Amaka Dev",
                            Reference = Guid.NewGuid().ToString(),
                            Currency = "NGN",
                            Source = "Account Balance",
                            Status = "Successful",
                            Responsestatus = true,
                            CreatedAt = DateTime.Now.ToString(),
                            WalletId = applicationUser.WalletId,
                        },

                        new Payout()
                        {

                            payoutId = "00002",
                            Amount = 10000,
                            Reason = "Bought a ferrari",
                            Recipient = "Chef Dave",
                            Reference = Guid.NewGuid().ToString(),
                            Currency = "NGN",
                            Source = "Account Balance",
                            Status = "Successful",
                            Responsestatus = true,
                            CreatedAt = DateTime.Now.ToString(),
                            WalletId = applicationUser.WalletId
                        },

                        new Payout()
                        {

                            payoutId = "00003",
                            Amount = 15000,
                            Reason = "School Fees",
                            Recipient = "Arthur john",
                            Reference = Guid.NewGuid().ToString(),
                            Currency = "NGN",
                            Source = "Account Balance",
                            Status = "Failed",
                            Responsestatus = false,
                            CreatedAt = DateTime.Now.ToString(),
                            WalletId = applicationUser.WalletId
                        },

                        new Payout()
                        {

                            payoutId = "00004",
                            Amount = 150000,
                            Reason = "Acceptance Fee",
                            Recipient = "Favour Azuide",
                            Reference = Guid.NewGuid().ToString(),
                            Currency = "NGN",
                            Source = "Account Balance",
                            Status = "Failed",
                            Responsestatus = false,
                            CreatedAt = DateTime.Now.ToString(),
                            WalletId= applicationUser.WalletId
                        },

                    },
                    CreditTransactionList = new List<Payin>
                   {
                       new Payin
                       {

                           Transactionid = "00031",
                           Amount = 10000,
                           UserId = "igwe",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "IgweOyedika@gmail.com",
                           AccountName = "Igwe Oyedika",
                           Bank = "Zenith Bank",
                           Status = "Successful",
                           GatewayResponse = "Resp303098",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "209810",
                           CardType = "Master Card",
                           Channel = "Card",
                           WalletId= applicationUser.WalletId
                       },

                       new Payin
                       {

                           Transactionid = "00072",
                           Amount = 16000,
                           UserId = "emmanuel Livinus",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "EzehLivi@gmail.com",
                           AccountName = "Emmanuel Chukwuemeka",
                           Bank = "Wema Bank",
                           Status = "Successful",
                           GatewayResponse = "Respo12309",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "334676",
                           CardType = "Master Card",
                           Channel = "Card",
                           WalletId= applicationUser.WalletId
                       },

                       new Payin
                       {

                           Transactionid = "04033",
                           Amount = 10000,
                           UserId = "Bello Saliu",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "BelloSaliugmail.com",
                           AccountName = "Echa obong",
                           Bank = "GTB Bank",
                           Status = "Failed",
                           GatewayResponse = "Resp09876",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "246335",
                           CardType = "Visa Card",
                           Channel = "Card",
                           WalletId= applicationUser.WalletId
                       },

                       new Payin
                       {

                           Transactionid = "04042",
                           Amount = 10000,
                           UserId = "solomon",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "solomonnwanga@gmail.com",
                           AccountName = "Solomon Nwanga",
                           Bank = "Access Bank Plc",
                           Status = "Successful",
                           GatewayResponse = "Resp123678",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "436810",
                           CardType = "Verve Card",
                           Channel = "Card",
                           WalletId= applicationUser.WalletId
                       },
                   }
                },
            };
        }


        private static ICollection<TransactionHistory> TransactionHistory3(ApplicationUser wallet)
        {
            return new List<TransactionHistory>()
            {
                new TransactionHistory()
                {

                    WalletId = wallet.WalletId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    DebitTransactionList = new List<Payout>
                    {
                        new Payout()
                        {
                            payoutId = "000111",
                            Amount = 790000,
                            Reason = "Microwave Payment",
                            Recipient = "Alexis McAllister",
                            Reference = Guid.NewGuid().ToString(),
                            Currency = "NGN",
                            Source = "Account Balance",
                            Status = "Successful",
                            Responsestatus = true,
                            CreatedAt = DateTime.Now.ToString(),
                            WalletId = wallet.WalletId,
                        },

                        new Payout()
                        {
                            payoutId = "00212",
                            Amount = 5000000,
                            Reason = "Bought a Mansion",
                            Recipient = "Dominik Sobozlai",
                            Reference = Guid.NewGuid().ToString(),
                            Currency = "NGN",
                            Source = "Account Balance",
                            Status = "Successful",
                            Responsestatus = true,
                            CreatedAt = DateTime.Now.ToString(),
                            WalletId= wallet.WalletId,
                        },

                        new Payout()
                        {
                            payoutId = "00603",
                            Amount = 90000000,
                            Reason = "Training Kit",
                            Recipient = "Joel Matip",
                            Reference = Guid.NewGuid().ToString(),
                            Currency = "NGN",
                            Source = "Account Balance",
                            Status = "Failed",
                            Responsestatus = false,
                            CreatedAt = DateTime.Now.ToString(),
                            WalletId= wallet.WalletId,
                        },

                        new Payout()
                        {
                            payoutId = "00404",
                            Amount = 1500000,
                            Reason = "Party in LaVegas",
                            Recipient = "Trent Arnold",
                            Reference = Guid.NewGuid().ToString(),
                            Currency = "NGN",
                            Source = "Account Balance",
                            Status = "Failed",
                            Responsestatus = false,
                            CreatedAt = DateTime.Now.ToString(),
                            WalletId= wallet.WalletId,
                        },

                    },
                    CreditTransactionList = new List<Payin>
                   {
                       new Payin
                       {
                           Transactionid = "50881",
                           Amount = 10008880,
                           UserId = "Allison",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "AllisonBecker@gmail.com",
                           AccountName = "Allison Becker",
                           Bank = "Zenith Bank",
                           Status = "Successful",
                           GatewayResponse = "Rhjp183198",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "298810",
                           CardType = "Master Card",
                           Channel = "Card",
                           WalletId = wallet.WalletId,
                       },

                       new Payin
                       {
                           Transactionid = "00992",
                           Amount = 16000000,
                           UserId = "Jason Ozil",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "JasonOzil@gmail.com",
                           AccountName = "Jason Ozil",
                           Bank = "Wema Bank",
                           Status = "Successful",
                           GatewayResponse = "Renmb18709",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "330055",
                           CardType = "Master Card",
                           Channel = "Card",
                           WalletId = wallet.WalletId
                       },

                       new Payin
                       {
                           Transactionid = "00553",
                           Amount = 100000000,
                           UserId = "2019647177",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "BenjaminEzeh@gmail.com",
                           AccountName = "Benjamin Ezeh",
                           Bank = "GTB Bank",
                           Status = "Failed",
                           GatewayResponse = "Rwel09956",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "246810",
                           CardType = "Visa Card",
                           Channel = "Card",
                           WalletId = wallet.WalletId
                       },

                       new Payin
                       {
                           Transactionid = "00861",
                           Amount = 105678000,
                           UserId = "burna",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "BurnaBoy@gmail.com",
                           AccountName = "Burna Boy",
                           Bank = "Access Bank Plc",
                           Status = "Successful",
                           GatewayResponse = "Regh129998",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "246810",
                           CardType = "Verve Card",
                           Channel = "Card",
                           WalletId = wallet.WalletId
                       },
                   }
                },
            };
        }
    }
}