//using FinancialPortal.Models;
//using Microsoft.AspNet.Identity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace FinancialPortal.Helpers
//{
//    public class TransactionHistoryHelper
//    {
//        private ApplicationDbContext db = new ApplicationDbContext();
//        public void TransactionHistories(Transaction oldTransaction, Transaction newTransaction)
//        {
//            OwnerUpdate(oldTransaction, newTransaction);
//            BudgetItemUpdate(oldTransaction, newTransaction);
//            AccountUpdate(oldTransaction, newTransaction);

//            db.SaveChanges();
//        }
       
//}