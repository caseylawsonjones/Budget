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
    public class IncomesController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Incomes
        [Authorize]
        public ActionResult Index()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null) {
                return RedirectToAction("Index", "Manage");
            }
            var incomes = db.Incomes.Where(i => i.HouseholdId == user.HouseholdId);
            return View(incomes.ToList());
        }

        // GET: Incomes/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null) {
                ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
                return PartialView("NotAuthorized", "Account");
            }
            Income income = db.Incomes.Find(id);
            if (income == null) {
                ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
                return PartialView("NotAuthorized", "Account");
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || income.HouseholdId != user.HouseholdId) {
                ViewBag.ErrorMessage = "Either you belong to no Household, or the resource you are trying to view belongs to another Household.  Either condition prevents you from viewing this resource.";
                return PartialView("NotAuthorized", "Account");
            }
            return View(income);
        }

        // GET: Incomes/Create
        [Authorize]
        public ActionResult Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.CategoryId = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            ViewBag.BudgetPlanCategoryId = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            return View();
        }

        // POST: Incomes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,EstimatedAmount,CategoryId,AccountId,BudgetPlanCategoryId")] Income income)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                income.OwnerId = user.Id;
                income.HouseholdId = user.HouseholdId;
                income.IsActive = true;
                db.Incomes.Add(income);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.CategoryId = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            ViewBag.BudgetPlanCategoryId = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            return View(income);
        }

        // GET: Incomes/Edit/5
        [Authorize]
        public PartialViewResult _Edit(int? id)
        {
            if (id == null) {
                ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
                return PartialView("_NotAuthorized", "Account");
            }
            Income income = db.Incomes.Find(id);
            if (income == null) {
                ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
                return PartialView("_NotAuthorized", "Account");
            }
            if (income.IsActive != true) {
                ViewBag.ErrorMessage = "The Income resource that you are attempting to access has been deactivated.  You cannot edit this Income item.";
                return PartialView("_NotAuthorized", "Account");
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || income.HouseholdId != user.HouseholdId) {
                ViewBag.ErrorMessage = "Either you belong to no Household, or the resource you are trying to view belongs to another Household.  Either condition prevents you from viewing this resource.";
                return PartialView("_NotAuthorized", "Account");
            }
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.CategoryId = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            ViewBag.BudgetPlanCategoryId = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            return PartialView(income);
        }

        // POST: Incomes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,EstimatedAmount,CategoryId,AccountId,UserId,HouseholdId,IsActive,BudgetPlanCategoryId")] Income income)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || income.HouseholdId != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }
            if (ModelState.IsValid){
                db.Entry(income).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.CategoryId = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            ViewBag.BudgetPlanCategoryId = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            return View(income);
        }

        // DEACTIVATE INCOME METHOD
        public ActionResult DeactivateIncome(int id) {
            Income income = db.Incomes.Find(id);
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            // Double-check that the User and Income are in the same Household
            if (user.HouseholdId != null && income.HouseholdId == user.HouseholdId) {
                // Set properties accordingly
                income.IsActive = false;
                income.EndDate = DateTime.UtcNow;
                income.AccountId = db.Accounts.First(a => a.Name == "Trash Can Account").Id;
                income.CategoryId = db.IncomeCategories.First(i => i.Category == "Closed").Id;
                db.Entry(income).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        //// GET: Incomes/Delete/5
        //[Authorize]
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Income income = db.Incomes.Find(id);
        //    if (income == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
        //    if (user.HouseholdId == null || income.HouseholdId != user.HouseholdId) {
        //        return RedirectToAction("NotAuthorized", "Account");
        //    }
        //    return View(income);
        //}

        //// POST: Incomes/Delete/5
        //[Authorize]
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
        //    Income income = db.Incomes.Find(id);
        //    if (user.HouseholdId == null || income.HouseholdId != user.HouseholdId) {
        //        return RedirectToAction("NotAuthorized", "Account");
        //    }
        //    db.Incomes.Remove(income);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
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
