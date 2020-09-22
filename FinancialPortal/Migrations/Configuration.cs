namespace FinancialPortal.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Configuration;
    using FinancialPortal.Helpers;
    using FinancialPortal.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<FinancialPortal.Models.ApplicationDbContext>
    {
        private RolesHelper roleHelper = new RolesHelper();
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(FinancialPortal.Models.ApplicationDbContext context)
        {
            Random random = new Random();
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            #region Roles Creation
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            //Reps the Head of Household role
            if (!context.Roles.Any(r => r.Name == "Head"))
            {
                roleManager.Create(new IdentityRole { Name = "Head" });
            }
            //reps a user who is part of a household, but not head of house role
            //will assign this role to anyone who reg an account with an invitation code or who enters an invitation code to join a household
            if (!context.Roles.Any(r => r.Name == "Member"))
            {
                roleManager.Create(new IdentityRole { Name = "Member" });
            }
            //reps a new user who has not joined or created a household
            //will assign this role to anyone who reg an account without using an invitation code
            //will also be reassigned to any user that leaves a household
            if (!context.Roles.Any(r => r.Name == "New User"))
            {
                roleManager.Create(new IdentityRole { Name = "New User" });
            }
            #endregion


            #region User Creation
            var adminEmail = WebConfigurationManager.AppSettings["AdminEmail"];
            var adminPassword = WebConfigurationManager.AppSettings["AdminPassword"];
            var headEmail = WebConfigurationManager.AppSettings["HeadEmail"];
            var headPassword = WebConfigurationManager.AppSettings["HeadPassword"];
            var memberEmail = WebConfigurationManager.AppSettings["MemberEmail"];
            var memberPassword = WebConfigurationManager.AppSettings["MemberPassword"];
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == adminEmail))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    FirstName = "Brandon",
                    LastName = "Swaney",
                    AvatarPath = "/Avatars/default_avatar.png",
                }, adminPassword);

                //get the id that just created by adding the above user
                var userId = userManager.FindByEmail(adminEmail).Id;

                userManager.AddToRole(userId, "Admin");
            }

            if (!context.Users.Any(u => u.Email == headEmail))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = headEmail,
                    UserName = headEmail,
                    FirstName = "Andrew",
                    LastName = "Russell",
                    AvatarPath = "/Avatars/default_avatar.png",
                }, headPassword);

                var userId = userManager.FindByEmail(headEmail).Id;
                userManager.AddToRole(userId, "Head");

            }

            if (!context.Users.Any(u => u.Email == memberEmail))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = memberEmail,
                    UserName = memberEmail,
                    FirstName = "Harvey",
                    LastName = "Denton",
                    AvatarPath = "/Avatars/default_avatar.png",
                }, memberPassword);

                var userId = userManager.FindByEmail(memberEmail).Id;
                userManager.AddToRole(userId, "Member");
            }

            if (!context.Users.Any(u => u.Email == "selenak@mailinator.com"))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = "selenak@mailinator.com",
                    UserName = "selenak@mailinator.com",
                    FirstName = "Selena",
                    LastName = "Kylie",
                    AvatarPath = "/Avatars/default_avatar.png",
                }, "Abc&123!");

                var userId = userManager.FindByEmail("selenak@mailinator.com").Id;
                userManager.AddToRole(userId, "New User");
            }
            if (!context.Users.Any(u => u.Email == "jimmyg@mailinator.com"))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = "jimmyg@mailinator.com",
                    UserName = "jimmyg@mailinator.com",
                    FirstName = "Jimmy",
                    LastName = "Gordon",
                    AvatarPath = "/Avatars/default_avatar.png",
                }, "Abc&123!");

                var userId = userManager.FindByEmail("jimmyg@mailinator.com").Id;
                userManager.AddToRole(userId, "New User");
            }
            #endregion

            context.SaveChanges();

  

        



        }


    }
}
