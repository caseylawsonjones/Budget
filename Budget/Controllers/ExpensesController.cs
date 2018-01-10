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

namespace Budget.Controllers
{
    [Authorize]
    public class ExpensesController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Expenses
        [Authorize]
        public ActionResult Index() {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null) {
                return RedirectToAction("Index", "Manage");
            }
            var expenses = db.Expenses.Where(e => e.HouseholdId == user.HouseholdId);
            return View(expenses.ToList());
        }

        // GET: Expenses/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = db.Expenses.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && expense.HouseholdId == user.HouseholdId) {
                return View(expense);
            }
            return RedirectToAction("NotAuthorized", "Account");
        }

        // GET: Expenses/Create
        [Authorize]
        public ActionResult Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            ViewBag.BudgetPlanCategoryId = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            return View();
        }

        // POST: Expenses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,EstimatedAmount,CategoryId,AccountId,BudgetPlanCategoryId")] Expense expense) {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (ModelState.IsValid) {
                expense.OwnerId = user.Id;
                expense.HouseholdId = user.HouseholdId;
                expense.IsActive = true;
                db.Expenses.Add(expense);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.CategoryId = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            ViewBag.BudgetPlanCategoryId = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            return View(expense);
        }

        // GET: Expenses/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = db.Expenses.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && expense.HouseholdId == user.HouseholdId) {
                ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
                ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
                ViewBag.CategoryId = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
                ViewBag.BudgetPlanCategoryId = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
                return View(expense);
            }
            return RedirectToAction("NotAuthorized", "Account");
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,EstimatedAmount,CategoryId,AccountId,UserId,HouseholdId,IsActive,BudgetPlanCategoryId")] Expense expense)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || expense.HouseholdId != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }
            if (ModelState.IsValid) {
                    db.Entry(expense).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            ViewBag.BudgetPlanCategoryId = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            return View(expense);
        }

        // DEACTIVATE EXPENSE METHOD
        public ActionResult DeactivateExpense(int id) {
            Expense expense = db.Expenses.Find(id);
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            // Double-check that the User and Expense are in the same Household
            if (user.HouseholdId != null && expense.HouseholdId == user.HouseholdId) {
                // Set properties accordingly
                expense.IsActive = false;
                expense.EndDate = DateTime.UtcNow;
                expense.AccountId = db.Accounts.First(a => a.Name == "Trash Can Account").Id;
                expense.CategoryId = db.ExpenseCategories.First(i => i.Category == "Closed").Id;
                db.Entry(expense).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


        //// GET: Expenses/Delete/5
        //[Authorize]
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Expense expense = db.Expenses.Find(id);
        //    if (expense == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
        //    if (user.HouseholdId != null && expense.HouseholdId == user.HouseholdId) {
        //        return View(expense);
        //    }
        //    return RedirectToAction("NotAuthorized", "Account");
        //}

        //// POST: Expenses/Delete/5
        //[Authorize]
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
        //    Expense expense = db.Expenses.Find(id);
        //    if (user.HouseholdId != null && expense.HouseholdId == user.HouseholdId) {
        //        db.Expenses.Remove(expense);
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
