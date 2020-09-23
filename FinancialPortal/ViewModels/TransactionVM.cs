using FinancialPortal.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.ViewModels
{
    public class TransactionVM
    {
       
            public TransactionType TransactionType { get; set; }
            public int AccountId { get; set; }
            public int BudgetItemId { get; set; }
            public decimal Amount { get; set; }
            public string Memo { get; set; }

    }
}