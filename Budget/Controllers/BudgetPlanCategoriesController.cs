using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budget.Models;
using Budget.Models.Categories;
using Microsoft.AspNet.Identity;

namespace Budget.Controllers
{
    [Authorize]
    public class BudgetPlanCategoriesController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public PartialViewResult _Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null) {
                ViewBag.ErrorMessage = "You do not appear to belong to a Household and cannot create new Budget Categories";
                return PartialView("_NotAuthorized", "Account");
            }

            return PartialView("_Create");
        }

        // POST: BudgetPlanCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Category")] BudgetPlanCategory budgetPlanCategory)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || (budgetPlanCategory.HouseholdId != user.HouseholdId && budgetPlanCategory.HouseholdId != null)) {
                ViewBag.ErrorMessage = "Either you belong to no Household, or the resource you are trying to view belongs to another Household.  Either condition prevents you from viewing this resource.";
                return View("NotAuthorized", "Account");
            }

            if (ModelState.IsValid)
            {
                budgetPlanCategory.HouseholdId = user.HouseholdId;
                db.BudgetPlanCategories.Add(budgetPlanCategory);
                db.SaveChanges();
                return RedirectToAction("Index", "BudgetPlans");
            }

            return View(budgetPlanCategory);
        }

        // GET: BudgetPlanCategories/Edit/5
        [Authorize]
        public PartialViewResult _Edit(int? id)
        {
            if (id == null) {
                ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
                return PartialView("_NotAuthorized", "Account");
            }
            BudgetPlanCategory budgetPlanCategory = db.BudgetPlanCategories.Find(id);
            if (budgetPlanCategory.HouseholdId == null) {
                ViewBag.ErrorMessage = "The Budget Category that you are attempting to Edit is a built-in Category and cannot be modified or deleted.";
                return PartialView("_NotAuthorized", "Account");
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || (budgetPlanCategory.HouseholdId != user.HouseholdId && budgetPlanCategory.HouseholdId != null)) {
                ViewBag.ErrorMessage = "Either you belong to no Household, or the resource you are trying to view belongs to another Household.  Either condition prevents you from viewing this resource.";
                return PartialView("_NotAuthorized", "Account");
            }
            if (budgetPlanCategory == null)
            {
                ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
                return PartialView("_NotAuthorized", "Account");
            }

            return PartialView(budgetPlanCategory);
        }

        // POST: BudgetPlanCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Category,HouseholdId")] BudgetPlanCategory budgetPlanCategory)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || (budgetPlanCategory.HouseholdId != user.HouseholdId && budgetPlanCategory.HouseholdId != null)) {
                ViewBag.ErrorMessage = "Either you belong to no Household, or the resource you are trying to view belongs to another Household.  Either condition prevents you from viewing this resource.";
                return View("NotAuthorized", "Account");
            }
            if (ModelState.IsValid)
            {
                db.Entry(budgetPlanCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "BudgetPlans");
            }
            return View("Index", "BudgetPlans");
        }

        [Authorize]
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetPlanCategory budgetPlanCategory = db.BudgetPlanCategories.Find(id);
            if (budgetPlanCategory.HouseholdId == null) {
                ViewBag.ErrorMessage = "The Budget Category that you are attempting to Delete is a built-in Category and cannot be modified or deleted.";
                return View("NotAuthorized", "Account");
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || (budgetPlanCategory.HouseholdId != user.HouseholdId && budgetPlanCategory.HouseholdId != null)) {
                ViewBag.ErrorMessage = "Either you belong to no Household, or the resource you are trying to view belongs to another Household.  Either condition prevents you from viewing this resource.";
                return View("NotAuthorized", "Account");
            }

            db.BudgetPlanCategories.Remove(budgetPlanCategory);
            db.SaveChanges();
            return RedirectToAction("Index", "BudgetPlans");
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
        // GET: BudgetPlanCategories
        //public ActionResult Index()
        //{
        //    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
        //    if (user.HouseholdId == null) {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    return View(db.BudgetPlanCategories.ToList());
        //}

        // GET: BudgetPlanCategories/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BudgetPlanCategory budgetPlanCategory = db.BudgetPlanCategories.Find(id);
        //    if (budgetPlanCategory == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(budgetPlanCategory);
        //}

        // GET: BudgetPlanCategories/Create
        // GET: BudgetPlanCategories/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null) {
        //        ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
        //        return View("NotAuthorized", "Account");
        //    }
        //    BudgetPlanCategory budgetPlanCategory = db.BudgetPlanCategories.Find(id);
        //    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
        //    if (user.HouseholdId == null || (budgetPlanCategory.HouseholdId != user.HouseholdId && budgetPlanCategory.HouseholdId != null)) {
        //        ViewBag.ErrorMessage = "Either you belong to no Household, or the resource you are trying to view belongs to another Household.  Either condition prevents you from viewing this resource.";
        //        return View("NotAuthorized", "Account");
        //    }
        //    if (budgetPlanCategory == null)
        //    {
        //        ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
        //        return View("NotAuthorized", "Account");
        //    }

        //    return View(budgetPlanCategory);
        //}

        // POST: BudgetPlanCategories/Delete/5
}
