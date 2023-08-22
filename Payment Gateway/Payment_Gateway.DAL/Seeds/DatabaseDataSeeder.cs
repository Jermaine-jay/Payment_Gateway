using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Payment_Gateway.DAL.Context;
using Payment_Gateway.Models.Entities;

namespace Payment_Gateway.DAL.Seeds
{
    public class DatabaseDataSeeder
    {
        public static async Task EnsurePopulatedAsync(IApplicationBuilder app)
        {
            PaymentGatewayDbContext context = app.ApplicationServices.CreateScope().ServiceProvider
                .GetRequiredService<PaymentGatewayDbContext>();

            if (!await context.TransactionHistory.AnyAsync())
            {
                await context.TransactionHistory.AddRangeAsync(TransactionHistory());
                await context.SaveChangesAsync();
            }
        }


        private static IEnumerable<TransactionHistory> TransactionHistory()
        {
            return new List<TransactionHistory>()
            {
                new TransactionHistory()
                {
                    WalletId = "2698818340",
                    DebitTransactionList = new List<Payout>
                    {
                        new Payout()
                        {
                            Id = Guid.NewGuid(),
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

                        },

                         new Payout()
                        {
                            Id = Guid.NewGuid(),
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

                        },

                        new Payout()
                        {
                            Id = Guid.NewGuid(),
                            payoutId = "00003",
                            Amount = 15000,
                            Reason = "Bought a private jet",
                            Recipient = "Ezwh Livinus",
                            Reference = Guid.NewGuid().ToString(),
                            Currency = "NGN",
                            Source = "Account Balance",
                            Status = "Failed",
                            Responsestatus = false,
                            CreatedAt = DateTime.Now.ToString(""),
                        },

                        new Payout()
                        {
                            Id = Guid.NewGuid(),
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
                        },

                    },

                   CreditTransactionList = new List<Transaction>
                   {
                       new Transaction 
                       { 
                           Id = Guid.NewGuid(),
                           Transactionid = "00001",
                           Amount = 10000,
                           UserId = "John Creator",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "johnCreator@gmail.com",
                           AccountName = "Ezeh Livinus",
                           Bank = "Zenith Bank",
                           Status = "Successful",
                           GatewayResponse = "Resp123098",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "246810",
                           WalletId = "2698818340",
                           CardType = "Master Card",
                           Channel = "Card",
                       },


                       new Transaction
                       {
                           Id = Guid.NewGuid(),
                           Transactionid = "00002",
                           Amount = 16000,
                           UserId = "Ezeh Livinus",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "EzehLivi@gmail.com",
                           AccountName = "Chef Dave",
                           Bank = "Wema Bank",
                           Status = "Successful",
                           GatewayResponse = "Respo12309",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "334455",
                           WalletId = "2698818340",
                           CardType = "Master Card",
                           Channel = "Card",
                       },

                       new Transaction
                       {
                           Id = Guid.NewGuid(),
                           Transactionid = "00003",
                           Amount = 10000,
                           UserId = "Bello Saliu",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "BelloSaliugmail.com",
                           AccountName = "Ben Ezeh",
                           Bank = "GTB Bank",
                           Status = "Failed",
                           GatewayResponse = "Resp09876",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "246810",
                           WalletId = "2698818340",
                           CardType = "Master Card",
                           Channel = "Card",
                       },

                       new Transaction
                       {
                           Id = Guid.NewGuid(),
                           Transactionid = "00001",
                           Amount = 10000,
                           UserId = "John Creator",
                           Reference = Guid.NewGuid().ToString(),
                           Email = "johnCreator@gmail.com",
                           AccountName = "Ezeh Livinus",
                           Bank = "Zenith Bank",
                           Status = "Successful",
                           GatewayResponse = "Resp123098",
                           CreatedAt = DateTime.UtcNow.ToString(),
                           PaidAt = DateTime.UtcNow.ToString(),
                           AuthorizationCode = "246810",
                           WalletId = "2698818340",
                           CardType = "Master Card",
                           Channel = "Card",
                       },
                   }

                }
            };

            throw new NotImplementedException();
        }
    }
}
