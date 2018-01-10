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
using Budget.Models.ViewModels;

namespace Budget.Controllers
{
    [Authorize]
    public class BudgetPlansController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetPlans
        [Authorize]
        public ActionResult Index()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            BudgetPlanViewModel bpVM = new BudgetPlanViewModel();
            if (user.HouseholdId == null) {
                return RedirectToAction("Index", "Manage");
            }

            var budgetPlans = db.BudgetPlans.Where(b => b.HouseholdId == user.HouseholdId).ToList();
            var budgetPlanCategories = db.BudgetPlanCategories.Where(b => b.HouseholdId == user.HouseholdId).ToList();

            bpVM.BudgetPlans = budgetPlans;
            bpVM.BudgetPlanCategories = budgetPlanCategories;

            return View(bpVM);
        }

        // GET: BudgetPlans/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetPlan budgetPlan = db.BudgetPlans.Find(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && budgetPlan.HouseholdId == user.HouseholdId) {
                if (budgetPlan == null) {
                    return HttpNotFound();
                }
                return View(budgetPlan);
            }
            return RedirectToAction("NotAuthorized", "Account");
        }

        // GET: BudgetPlans/Create
        [Authorize]
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.CategoryId = new SelectList((db.BudgetPlanCategories.Where(bc => bc.HouseholdId == null || bc.HouseholdId == user.HouseholdId)), "Id", "Category");
            //ViewBag.BudgetItems = new SelectList((db.BudgetItems.Where(bc => (bc.HouseholdId == null || bc.HouseholdId == user.HouseholdId) && bc.IsActive == true)), "Id", "Name");
            return View();
        }

        // POST: BudgetPlans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,CategoryId,AmountBudgeted")]  BudgetPlan budgetPlan)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                budgetPlan.OwnerId = user.Id;
                budgetPlan.HouseholdId = user.HouseholdId.Value;
                budgetPlan.IsActive = true;
                db.BudgetPlans.Add(budgetPlan);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = budgetPlan.Id });
            }

            ViewBag.CategoryId = new SelectList((db.BudgetPlanCategories.Where(bc => bc.HouseholdId == null || bc.HouseholdId == user.HouseholdId)), "Id", "Category");
            //ViewBag.BudgetItems = new SelectList((db.BudgetItems.Where(bc => (bc.HouseholdId == null || bc.HouseholdId == user.HouseholdId) && bc.IsActive == true)), "Id", "Name");
            return View(budgetPlan);
        }

        // GET: BudgetPlans/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetPlan budgetPlan = db.BudgetPlans.Find(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && budgetPlan.HouseholdId == user.HouseholdId) {
                if (budgetPlan == null) {
                    return HttpNotFound();
                }
                ViewBag.CategoryId = new SelectList((db.BudgetPlanCategories.Where(bc => bc.HouseholdId == null || bc.HouseholdId == user.HouseholdId)), "Id", "Category");
                //ViewBag.BudgetItems = new SelectList((db.BudgetItems.Where(bc => bc.HouseholdId == null || bc.HouseholdId == user.HouseholdId)), "Id", "Name");
                return View(budgetPlan);
            }
            return RedirectToAction("NotAuthorized", "Account");
        }

        // POST: BudgetPlans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,IsActive,CategoryId,HouseholdId,OwnerId,AmountBudgeted")]  BudgetPlan budgetPlan)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || budgetPlan.HouseholdId != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }
            if (ModelState.IsValid) {
                if (budgetPlan.IsActive == false) { budgetPlan.Deactivated = DateTime.UtcNow; }
                db.Entry(budgetPlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = budgetPlan.Id });
            }
            ViewBag.CategoryId = new SelectList((db.BudgetPlanCategories.Where(bc => bc.HouseholdId == null || bc.HouseholdId == user.HouseholdId)), "Id", "Category");
            //ViewBag.BudgetItems = new SelectList((db.BudgetItems.Where(bc => (bc.HouseholdId == null || bc.HouseholdId == user.HouseholdId) && bc.IsActive == true)), "Id", "Name");
            return View(budgetPlan);
        }

        // GET: BudgetPlans/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetPlan budgetPlan = db.BudgetPlans.Find(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && budgetPlan.HouseholdId == user.HouseholdId) {
                if (budgetPlan == null) {
                    return HttpNotFound();
                }
                return View(budgetPlan);
            }
            return RedirectToAction("NotAuthorized", "Account");
        }

        // POST: BudgetPlans/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
             BudgetPlan  budgetPlan  = db.BudgetPlans.Find(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && budgetPlan.HouseholdId == user.HouseholdId) {
                db.BudgetPlans.Remove(budgetPlan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotAuthorized", "Account");

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
