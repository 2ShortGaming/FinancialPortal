using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPortal.Models;
using FinancialPortal.Extensions;
using FinancialPortal.ViewModels;
using FinancialPortal.Enums;
using Microsoft.AspNet.Identity;

namespace FinancialPortal.Controllers
{
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.Account).Include(t => t.BudgetItem).Include(t => t.Owner);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "AccountName");
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName");
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,BudgetItemId,Amount,Memo")] Transaction transaction)
        {
            transaction.Created = DateTime.Now;
            transaction.OwnerId = User.Identity.GetUserId();
            transaction.IsDeleted = false;
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                var thisTransaction = db.Transactions.Include(t => t.BudgetItem).FirstOrDefault(t => t.Id == transaction.Id);
                thisTransaction.UpdateBalances();
                var bankAccount = db.BankAccounts.Find(db.Transactions.Find(thisTransaction.Id).AccountId);
                if(bankAccount.CurrentBalance < 0)
                {
                    TempData["WarningBalance"] += "<p class\"text-danger\">Created Transaction has overdrawn your account!</p>";
                }  
                else if(bankAccount.CurrentBalance < bankAccount.WarningBalance)
                {
                    TempData["WarningBalance"] += "<p class\"text-danger\"> Created Transaction has brought your account under the warning balance!</p>";
                }
                //transaction.UpdateBalances();
                return RedirectToAction("Index", "Home");
            }

            //ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "OwnerId", transaction.AccountId);
            //ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName", transaction.BudgetItemId);
            //ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", transaction.OwnerId);
            return View(transaction);
        }

        //GET: Transactions/CreateDeposit
        public ActionResult CreateDeposit()
        {
            var model = new TransactionVM();
            model.TransactionType = TransactionType.Deposit;
            return View(model);
        }

        //POST: Transactions/CreateDeposit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDeposit(TransactionVM model)
        {
            if (ModelState.IsValid)
            {
                var transaction = new Transaction()
                {
                    AccountId = model.AccountId,
                    BudgetItemId = model.BudgetItemId,
                    OwnerId = User.Identity.GetUserId(),
                    TransactionType = model.TransactionType,
                    Created = DateTime.Now,
                    Amount = model.Amount,
                    Memo = model.Memo,
                    IsDeleted = false
                };
                db.Transactions.Add(transaction);
                db.SaveChanges();

                var thisTransaction = db.Transactions.Include(t => t.BudgetItem).FirstOrDefault(t => t.Id == transaction.Id);
                thisTransaction.UpdateBalances();

                return RedirectToAction("Index", "Home");
            }
            ViewBag.ErrorMessage = "Something went wrong.  Please try again.";
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "AccountName");
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName");

            var trans = new TransactionVM();
            trans.TransactionType = TransactionType.Deposit;

            return View(trans);
        }

        public PartialViewResult _TransactionModal(int accountId)
        {
            var model = new Transaction()
            {
                AccountId = accountId
            };
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName");
            return PartialView(model);
        }

        // GET: Transactions/CreateWithdrawal
        public ActionResult CreateWithdrawal()
        {
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "AccountName");
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName");

            var model = new TransactionVM();
            model.TransactionType = TransactionType.Withdrawal;
            return View(model);
        }

        // POST: Transactions/CreateWithdrawal
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWithdrawal(TransactionVM model)
        {
            if (ModelState.IsValid)
            {
                var transaction = new Transaction()
                {
                    AccountId = model.AccountId,
                    BudgetItemId = model.BudgetItemId,
                    OwnerId = User.Identity.GetUserId(),
                    TransactionType = model.TransactionType,
                    Created = DateTime.Now,
                    Amount = model.Amount,
                    Memo = model.Memo,
                    IsDeleted = false
                };

                db.Transactions.Add(transaction);
                db.SaveChanges();

                var thisTransaction = db.Transactions.Include(t => t.BudgetItem).FirstOrDefault(t => t.Id == transaction.Id);
                thisTransaction.UpdateBalances();

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Something went wrong.  Please try again.";
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "AccountName");
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName");

            var trans = new TransactionVM();
            trans.TransactionType = TransactionType.Withdrawal;

            return View(trans);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "OwnerId", transaction.AccountId);
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName", transaction.BudgetItemId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", transaction.OwnerId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,BudgetItemId,OwnerId,Created,Amount,Memo,IsDeleted")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var oldTransaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == transaction.Id);
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();

                var newTransaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == transaction.Id);
                //newTransaction.EditTransaction(oldTransaction, newTransaction);
                
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "OwnerId", transaction.AccountId);
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName", transaction.BudgetItemId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", transaction.OwnerId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
