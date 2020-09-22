using FinancialPortal.Controllers;
using FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.ViewModels
{
    public class ConfigureHouseVM
    {
        public int? HouseholdId { get; set; }
        public BankAccount BankAccount { get; set; }
        public decimal StartingBalance { get; set; }
        public Budget Budget { get; set; }
        public BudgetItem BudgetItem { get; set; }

        //public ICollection<BankAccount> BankAccounts { get; set; }
        //public ICollection<Budget> Budgets { get; set; }
        //public ICollection<BudgetItem> BudgetItems { get; set; }
        //public ICollection<BankAccountWizardVM> BankAccounts { get; set; }
        //public ICollection<BudgetWizardVM> Budgets { get; set; }
    }
}