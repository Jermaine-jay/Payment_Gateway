﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
            

            modelBuilder.Entity<Payout>()
                .HasKey(a => a.Id);


            modelBuilder.Entity<Payin>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Wallet>(p =>
            {
                p.Property(p => p.Balance)
                    .HasDefaultValue(0);

            });


            modelBuilder.Entity<Payout>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);


            modelBuilder.Entity<ApiKey>(p =>
            {
                p.HasKey(p => p.ApiSecretKey);
                
            });


            modelBuilder.Entity<ApiKey>()
                .HasOne(a => a.ApplicationUser)
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.ApiSecretKey)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Wallet)
                .WithOne(u=>u.ApplicationUser)
                .HasForeignKey<ApplicationUser>(u => u.WalletId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<TransactionHistory>()
                 .HasOne(t => t.Wallet)   
                 .WithOne(w => w.TransactionHistory)      
                 .HasForeignKey<TransactionHistory>(t => t.WalletId)
                 .OnDelete(DeleteBehavior.Restrict);

            
        
            /* modelBuilder.Entity<Payin>()
                 .HasOne(a => a.TransactionHistory)
                 .WithMany(u => u.CreditTransactionList)
                 .HasForeignKey(u => u.Id)
                 .OnDelete(DeleteBehavior.Restrict);


             modelBuilder.Entity<Payout>()
                 .HasOne(a => a.TransactionHistory)
                 .WithMany(u => u.DebitTransactionList)
                 .HasForeignKey(u => u.Id)
                 .OnDelete(DeleteBehavior.Restrict);*/


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
