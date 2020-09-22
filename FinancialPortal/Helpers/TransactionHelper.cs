using FinancialPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinancialPortal.Helpers;

namespace FinancialPortal.Helpers
{
    public class TransactionHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RolesHelper roleHelper = new RolesHelper();
        public List<Transaction> ListHouseholdTransactions()
        {
            var houseTrans = new List<Transaction>();
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            houseTrans.AddRange(db.BankAccounts.Where(b => b.HouseholdId == user.HouseholdId).SelectMany(b => b.Transactions));
            return houseTrans;
        }

        public bool CanCreateTransaction()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();

            switch (myRole)
            {
                case "Head":
                case "Member":
                    return true;
                default:
                    return false;
            }
        }
        public bool CanEditTransaction()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();

            switch (myRole)
            {
                case "Head":
                case "Member":
                    return true;
                default:
                    return false;
            }
        }
        public bool CanDeletTransaction()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();

            switch (myRole)
            {
                case "Head":
                case "Member":
                    return true;
                default:
                    return false;
            }
        } 
    }
}