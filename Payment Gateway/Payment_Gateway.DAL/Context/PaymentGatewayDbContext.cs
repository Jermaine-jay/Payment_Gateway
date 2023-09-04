using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Payment_Gateway.Models.Entities;


namespace Payment_Gateway.DAL.Context
{
    public class PaymentGatewayDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public PaymentGatewayDbContext(DbContextOptions<PaymentGatewayDbContext> options)
            : base(options)
        {

        }


        public DbSet<Admin> Admins { get; set; }
        public DbSet<AdminProfile> AdminProfiles { get; set; }
        public DbSet<Payin> Payins { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<Payout> Payouts { get; set; }
        public DbSet<TransactionHistory> TransactionHistory { get; set; }
        public DbSet<ApplicationRoleClaim> ApplicationRoleClaims { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Wallet>()
               .HasKey(a => a.WalletId);


            modelBuilder.Entity<TransactionHistory>()
               .HasKey(a => a.Id);


            modelBuilder.Entity<ApiKey>()          
                .HasKey(p => p.ApiSecretKey);
            

            modelBuilder.Entity<Payout>()
                .HasKey(a => a.Id);


            modelBuilder.Entity<Payin>()
                .HasKey(a => a.Id);


            modelBuilder.Entity<Payout>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);


                
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.ApiKey)
                .WithOne(u=>u.ApplicationUser)
                .HasForeignKey<ApplicationUser>(u => u.ApiSecretKey)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Wallet)
                .WithOne(u=>u.ApplicationUser)
                .HasForeignKey<ApplicationUser>(u => u.WalletId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<TransactionHistory>()
                .HasMany(a => a.CreditTransactionList)
                .WithOne(u => u.TransactionHistory)
                .HasForeignKey(u => u.TransactionHistoryId)
                .OnDelete(DeleteBehavior.Restrict); 


            modelBuilder.Entity<TransactionHistory>()
                .HasMany(a => a.DebitTransactionList)
                .WithOne(u => u.TransactionHistory)
                .HasForeignKey(u => u.TransactionHistoryId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ApplicationRole>(b =>
                {
                    b.HasMany<ApplicationUserRole>()
                    .WithOne()
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.NoAction);
                });


            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.HasMany<ApplicationUserRole>()
               .WithOne()
               .HasForeignKey(ur => ur.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
