﻿using FinancialPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Helpers
{
    public class UserHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string GetFullName(string userId)
        {
            var user = db.Users.Find(userId);
            var firstName = user.FirstName;
            var lastName = user.LastName;

            return firstName + " " + lastName;
        }
        public string LastNameFirst(string userId)
        {
            var user = db.Users.Find(userId);
            return user.FullName;
        }
     
        public string GetAvatarPath()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            return user.AvatarPath;
        }

        public string GetUserRole()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var roleId = user.Roles.Where(u => u.UserId == userId);
            return null;
        }
        public string GetUserRole(string userId)
        {
            return null;
        }

        public List<ApplicationUser> ListUsers()
        {
            return db.Users.ToList();
        }
        //public string GetUserAccounts()
        //{
        //    var userId = HttpContext.Current.User.Identity.GetUserId();
        //    var user = db.Users.Find(userId);
        //    var bankAccount = db.BankAccounts.ToList();
        //}
    }
}