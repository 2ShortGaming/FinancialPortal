using FinancialPortal.Extensions;
using FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Helpers
{
    public class BankAccountHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int HouseHBankAccount()
        {
            var hhId = HttpContext.Current.User.Identity.GetHouseholdId();
            return db.BankAccounts.Where(h => h.HouseholdId == hhId).ToList().Count;
        }

        public List<BankAccount> ListHouseHBankAccounts()
        {
            List<BankAccount> accounts = new List<BankAccount>();
            var hhId = HttpContext.Current.User.Identity.GetHouseholdId();

            accounts.AddRange(db.BankAccounts.Where(h => h.HouseholdId == hhId).ToList());

            return accounts;
        }

        public decimal TotalBal(BankAccount account)
        {
            return account.CurrentBalance;
        }
    }
}