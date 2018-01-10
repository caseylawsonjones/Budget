using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budget.Models;
using Microsoft.AspNet.Identity;
using Budget.Models.Helpers;
using Budget.Models.ViewModels;

namespace Budget.Controllers
{
    [Authorize]
    public class AccountsController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accounts
        [Authorize]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null) { // Verify that User is in a Household, then send only Accounts related to that Household
                var accounts = db.Accounts.Where(a => a.HouseholdId == user.HouseholdId).ToList();
                return View(accounts);
            }
            return RedirectToAction("Index", "Manage");
        }

        // GET: Accounts/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && account.HouseholdId == user.HouseholdId) {
                return View(account);
            }
            return RedirectToAction("Index", "Manage");
        }

        // GET: Accounts/Create
        [Authorize]
        public ActionResult Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.CategoryId = new SelectList(db.AccountCategories.Where(x => x.HouseholdId == 0 || x.HouseholdId == user.HouseholdId), "Id", "Category");
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,InitiationBalance,AccountNumber,CategoryId")] Account account)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                if (user.HouseholdId == null) {
                    return RedirectToAction("Index", "Manage");
                }
                account.Opened = DateTime.UtcNow;
                account.OwnerId = user.Id;
                account.HouseholdId = user.HouseholdId.Value;
                account.IsActive = true;
                db.Accounts.Add(account);
                db.SaveChanges();
                TransactionProcessor txProc = new TransactionProcessor();
                txProc.AccountInitiatorTransaction(account.Id, account.InitiationBalance, user);
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.AccountCategories.Where(x => x.HouseholdId == 0 || x.HouseholdId == user.HouseholdId), "Id", "Category", account.CategoryId);
            return View(account);
        }

        // GET: Accounts/Edit/5
        [Authorize]
        public PartialViewResult _Edit(int? id)
        {
            if (id == null)
            {
                return PartialView("_NonExistentRecord", "Account");
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return PartialView("_NotAuthorized", "Account");
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && account.HouseholdId == user.HouseholdId) {
                ViewBag.CategoryId = new SelectList(db.AccountCategories.Where(x => x.HouseholdId == 0 || x.HouseholdId == user.HouseholdId), "Id", "Category", account.CategoryId);
                return PartialView(account);
            }
            return PartialView("_NotAuthorized", "Account");
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Opened,Closed,Balance,AccountNumber,OwnerId,CategoryId,HouseholdId,IsActive")] Account account) {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && account.HouseholdId == user.HouseholdId) {
                if (ModelState.IsValid) {
                    db.Entry(account).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.CategoryId = new SelectList(db.AccountCategories.Where(x => x.HouseholdId == 0 || x.HouseholdId == user.HouseholdId), "Id", "Category", account.CategoryId);
                return View(account);
            }
            return RedirectToAction("NotAuthorized", "Account");
        }

        public ActionResult CloseAccount(int? id) {
            Account account = db.Accounts.Find(id);

            // Incomes and Expenses Associated with this account no longer have an associated account.
            // The returned View will list these so they can be reassigned to a new Account
            CloseAccountViewModel closeAccountVM = new CloseAccountViewModel();
            closeAccountVM.IncomesWithNoAssociatedAccount = account.Incomes.ToList();
            closeAccountVM.ExpensesWithNoAssociatedAccount = account.Expenses.ToList();

            // The VM needs to know what Account was closed as well as a list of all Active Accounts to choose from
            // in the next view when reassigning the Incomes and Expenses.
            closeAccountVM.ClosedAccount = account;
            closeAccountVM.AllActiveAccounts = db.Accounts.Where(a => a.IsActive == true).ToList();

            // Set the associated Expenses' and Incomes' AccountId's to 0
            foreach (var inc in account.Incomes) {
                inc.AccountId = 0;
            }
            foreach(var exp in account.Expenses) {
                exp.AccountId = 0;
            }

            // Get the Account's closing Balance
            decimal balance = 0;
            foreach (var tx in db.Transactions.Where(t => t.AccountId == id)) {
                if (tx.IsCredit) {
                    balance += tx.Amount;
                }
                else {
                    balance -= tx.Amount;
                }
            }
            account.ClosingBalance = balance;
            account.IsActive = false;
            account.Closed = DateTime.UtcNow;

            // Set the Account's 
            account.CategoryId = db.AccountCategories.First(ac => ac.Category == "Closed").Id;

            db.Entry(account).State = EntityState.Modified;
            db.SaveChanges();

            return View(closeAccountVM);
        }

        //// GET: Accounts/Delete/5
        //[Authorize]
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Account account = db.Accounts.Find(id);
        //    if (account == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    var user = db.Users.Find(User.Identity.GetUserId());
        //    if (user.HouseholdId != null && account.HouseholdId == user.HouseholdId) {
        //        return View(account);
        //    }
        //    return RedirectToAction("NotAuthorized", "Account");
        //}

        //// POST: Accounts/Delete/5
        //[Authorize]
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    var user = db.Users.Find(User.Identity.GetUserId());
        //        Account account = db.Accounts.Find(id);
        //    if (user.HouseholdId != null && account.HouseholdId == user.HouseholdId) {
        //        db.Accounts.Remove(account);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return RedirectToAction("NotAuthorized", "Account");
        //}

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
