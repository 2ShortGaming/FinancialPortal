using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using FinancialPortal.Enums;
using FinancialPortal.Models;

namespace FinancialPortal.Extensions
{
    public static class TransactionExtensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static void UpdateBalances(this Transaction transaction)
        {
            UpdateBankBalance(transaction);
            if (transaction.TransactionType == Enums.TransactionType.Withdrawal)
            {

                UpdateBudgetAmount(transaction);
                UpdateBudgetItemAmount(transaction);
            }

        }
        private static void UpdateBankBalance(Transaction transaction)
        {
            var bankAccount = db.BankAccounts.Find(transaction.AccountId);
            if (transaction.TransactionType == Enums.TransactionType.Deposit)
            {
                bankAccount.CurrentBalance += transaction.Amount;
            }
            else if (transaction.TransactionType == Enums.TransactionType.Withdrawal)
            {
                bankAccount.CurrentBalance -= transaction.Amount;
            }
            db.SaveChanges();
            switch (transaction.TransactionType)
            {
                case Enums.TransactionType.Deposit:
                    bankAccount.CurrentBalance += transaction.Amount;
                    break;
                case Enums.TransactionType.Withdrawal:
                    bankAccount.CurrentBalance -= transaction.Amount;
                    break;
                default:
                    return;
            }

        }
        
        public static void EditTransaction(this Transaction newTransaction, Transaction oldTransaction)
        {
            //What happens when I Edit a transaction? - could I use a momento object:  .AsNoTracking()
            //What happens when I Delete or Void a transaction? - Part of these methods involve tracking the old amount and the new amount

            if (oldTransaction.TransactionType == Enums.TransactionType.Deposit)
            {
                ReverseUpdateBankBalance(oldTransaction);
                UpdateBankBalance(newTransaction);
            }
            if (oldTransaction.TransactionType == Enums.TransactionType.Withdrawal)
            {
                ReverseUpdateBankBalance(oldTransaction);
                UpdateBankBalance(newTransaction);
                ReverseUpdateBudgetAmount(oldTransaction);
                UpdateBudgetAmount(newTransaction);
                ReverseUpdateBudgetItemAmount(oldTransaction);
                UpdateBudgetItemAmount(newTransaction);
            }
        }


        private static void ReverseUpdateBankBalance(Transaction transaction)
        {
            var bankAccount = db.BankAccounts.Find(transaction.AccountId);
            switch (transaction.TransactionType)
            {
                case TransactionType.Deposit:
                    bankAccount.CurrentBalance -= transaction.Amount;
                    break;
                case TransactionType.Withdrawal:
                    bankAccount.CurrentBalance += transaction.Amount;
                    break;
                default:
                    return;
            }
            db.SaveChanges();
        }

        private static void ReverseUpdateBudgetAmount(Transaction transaction)
        {
            var budgetItem = db.BudgetItems.Find(transaction.BudgetItemId);
            var budget = db.Budgets.Find(budgetItem.BudgetId);
            budget.CurrentAmount -= transaction.Amount;
            db.SaveChanges();
        }

        private static void ReverseUpdateBudgetItemAmount(Transaction transaction)
        {
            var budgetItem = db.BudgetItems.Find(transaction.BudgetItemId);
            budgetItem.CurrentAmount -= transaction.Amount;
            db.SaveChanges();
        }

        private static void UpdateBudgetAmount(Transaction transaction)
        {
            if (transaction.TransactionType == Enums.TransactionType.Deposit || transaction.BudgetItemId == null)
            {
                return;

            }
            var budgetId = db.BudgetItems.Find(transaction.BudgetItemId).BudgetId;
            var budget = db.Budgets.Find(budgetId);
            budget.CurrentAmount += transaction.Amount;
            db.SaveChanges();

        }
        private static void UpdateBudgetItemAmount(Transaction transaction)
        {
            if (transaction.TransactionType == Enums.TransactionType.Deposit || transaction.BudgetItemId == null)
            {
                return;

            }
            var budgetItem = db.BudgetItems.Find(transaction.BudgetItemId);
            budgetItem.CurrentAmount += transaction.Amount;
            db.SaveChanges();
        }

    }
}
