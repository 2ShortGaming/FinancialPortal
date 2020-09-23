using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.EnterpriseServices;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FinancialPortal.Extensions;
using FinancialPortal.Helpers;
using FinancialPortal.Models;
using FinancialPortal.ViewModels;
using Microsoft.AspNet.Identity;

namespace FinancialPortal.Controllers
{
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RolesHelper roleHelper = new RolesHelper();
        private HouseHelper houseHelper = new HouseHelper();
        // GET: Households
        public ActionResult Index()
        {
            return View(db.Households.ToList());
        }

        //GET: Members
        public ActionResult Members()
        {
            return View(houseHelper.ListHouseholdMembers());
        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "New User")]
        public async Task<ActionResult> Create([Bind(Include = "Id,HouseholdName,Greeting")] Household household)
        {
            if (ModelState.IsValid)
            {
                household.Created = DateTime.Now;
                db.Households.Add(household);
                db.SaveChanges();

                var user = db.Users.Find(User.Identity.GetUserId());
                user.HouseholdId = household.Id;
                foreach (var account in user.Accounts)
                {
                    account.HouseholdId = null;
                }
                roleHelper.AddUserToRole(user.Id, "Head");
                db.SaveChanges();

                await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);
                return RedirectToAction("ConfigureHouse");
            }

            return View(household);
        }

        [HttpGet]
        //[Authorize(Roles = "Head")]
        public ActionResult ConfigureHouse()
        {
            var model = new ConfigureHouseVM();
            model.HouseholdId = User.Identity.GetHouseholdId();
            if (model.HouseholdId == 0)
            {
                return RedirectToAction("Create");
            }
            return View(model);
        }

        //POST: ConfigureHouse

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfigureHouse(ConfigureHouseVM model)
        {
            var bankAccount = new BankAccount(model.StartingBalance, model.BankAccount.WarningBalance, model.BankAccount.AccountName);
            bankAccount.AccountType = model.BankAccount.AccountType;
            bankAccount.HouseholdId = (int)model.HouseholdId;
            db.BankAccounts.Add(bankAccount);

            var budget = new Budget();
            budget.HouseholdId = (int)model.HouseholdId;
            budget.BudgetName = model.Budget.BudgetName;
            db.Budgets.Add(budget);
            db.SaveChanges();

            var budgetItem = new BudgetItem();
            budgetItem.BudgetId = budget.Id;
            budgetItem.TargetAmount = model.BudgetItem.TargetAmount;
            budgetItem.ItemName = model.BudgetItem.ItemName;
            db.BudgetItems.Add(budgetItem);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdName,Greeting,Created,IsDeleted")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        //user leaving the hose the HHid needs to be null
        //if the user is in the head role someone must be take over
        //if user is a member they can just leave
        //anyone leaving needs role reset to new user role
        //if user is last person in household, household can be deleted.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeHouseholdRequired]
        public async Task<ActionResult> LeaveAsync()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var role = roleHelper.ListUserRoles(userId).FirstOrDefault();
            switch (role)
            {
                case "Head":
                    var memberCount = db.Users.Where(u => u.HouseholdId == user.HouseholdId).Count() - 1;
                    if (memberCount >= 1)
                    {
                        TempData["Message"] = $"You are unable to leave the household! There are still <b>{memberCount}</b> other members in the household. You must select one of them to assume your role!";
                        return RedirectToAction("ExitDenied");
                    }
                    //this is a soft delete, record stays in DB, you can limit access on the front end
                    user.Household.IsDeleted = true;
                    user.HouseholdId = null;
                    // this is a hard delete, record will be removed from DB, and anything with the HH FK is deleted.
                    //var household = db.Households.Find(user.HouseholdId);
                    //db.Households.Remove(household);
                    foreach (var account in user.Accounts)
                    {
                        account.HouseholdId = null;
                    }
                    db.SaveChanges();

                    roleHelper.UpdateUserRole(userId, "New User");
                    await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);
                    return RedirectToAction("Index", "Home");
                case "Member":
                    user.HouseholdId = null;
                    foreach (var account in user.Accounts)
                    {
                        account.HouseholdId = null;
                    }
                    db.SaveChanges();

                    roleHelper.UpdateUserRole(userId, "New User");
                    await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);
                    return RedirectToAction("Index", "Home");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "Head")]
        public ActionResult ExitDenied()
        {
            return View();
        }

        [Authorize(Roles = "Head")]

        public ActionResult ChangeHead()
        {
            var myHouseId = User.Identity.GetHouseholdId();
            if(myHouseId == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var members = db.Users.Where(u => u.HouseholdId == myHouseId).ToList();
            ViewBag.NewHoH = new SelectList(members, "Id", "FullName");
            return View();

        }

        [HttpPost, ActionName("ChangeHead")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeHeadAsync(string newHoH, bool leave)
        {
            if(string.IsNullOrEmpty(newHoH))
            {
                return RedirectToAction("Index", "Home");
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            roleHelper.UpdateUserRole(newHoH, "Head");
            if(leave)
            {
                //user.Household.IsDeleted = true;
                user.HouseholdId = null;
            foreach (var account in user.Accounts)
                {
                    account.HouseholdId = null;
                }
                db.SaveChanges();
                roleHelper.UpdateUserRole(user.Id, "New User");
                await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);
                
            }
            else
            {
                roleHelper.UpdateUserRole(user.Id, "Member");
                await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);
            }
            return RedirectToAction("Index", "Home");
        }


        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
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
