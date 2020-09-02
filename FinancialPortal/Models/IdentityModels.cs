﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FinancialPortal.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        #region Actual Properties
        [Display(Name = "First Name")]
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 50 characters")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 50 characters")]
        public string LastName { get; set; }
        public int? HouseholdId { get; set; }
        public virtual Household Household { get; set; }
        public string AvatarPath { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<BankAccount> Accounts { get; set; }

        #endregion
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<BudgetItem> BudgetItems { get; set; }

        public System.Data.Entity.DbSet<FinancialPortal.Models.BankAccount> BankAccounts { get; set; }

        public System.Data.Entity.DbSet<FinancialPortal.Models.Household> Households { get; set; }

        public System.Data.Entity.DbSet<FinancialPortal.Models.ApplicationUser> ApplicationUsers { get; set; }

        public System.Data.Entity.DbSet<FinancialPortal.Models.Budget> Budgets { get; set; }

        public System.Data.Entity.DbSet<FinancialPortal.Models.Invitation> Invitations { get; set; }

        public System.Data.Entity.DbSet<FinancialPortal.Models.Notification> Notifications { get; set; }

        public System.Data.Entity.DbSet<FinancialPortal.Models.Transaction> Transactions { get; set; }
    }
}