using FinancialPortal.Extensions;
using FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Helpers
{
    public class BudgetsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int BudgetsCount()
        {
            return db.Budgets.ToList().Count;
        }

        public List<Budget> ListBudgets()
        {
            List<Budget> budgets = new List<Budget>();

            budgets.AddRange(db.Budgets);

            return budgets;
        }

        public List<Budget> ListHHBudgets()
        {
            var hhId = HttpContext.Current.User.Identity.GetHouseholdId();
            List<Budget> hhBudgets = new List<Budget>();

            hhBudgets = db.Budgets.Where(b => b.HouseholdId == hhId).ToList();

            return hhBudgets;
        }
    }
}