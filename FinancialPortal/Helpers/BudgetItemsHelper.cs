using FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Helpers
{
    public class BudgetItemsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<BudgetItem> ListBudgetItemsForBudget(int budgetId)
        {
            List<BudgetItem> budgetItems = new List<BudgetItem>();

            budgetItems.AddRange(db.BudgetItems.Where(b => b.BudgetId == budgetId));

            return budgetItems;
        }

        public int BudgetItemsCount(int budgetId)
        {
            var count = db.BudgetItems.Where(b => b.BudgetId == budgetId && !b.IsDeleted).ToList().Count();
            return count;
        }

        public int GetBudgetItemByName(string name, int budgetId)
        {
            return db.BudgetItems.Where(b => b.BudgetId == budgetId && b.ItemName == name).FirstOrDefault().Id;
        }
    }
}