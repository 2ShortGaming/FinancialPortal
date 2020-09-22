using FinancialPortal.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPortal.ViewModels
{
    public class BankAccountWizardVM
    {
        public decimal StartingBalance { get; set; }
        public decimal WarningBalance { get; set; }

        [Display(Name = "Name")]
        public string AccountName { get; set; }
        public AccountType AccountType { get; set; }

    }
}