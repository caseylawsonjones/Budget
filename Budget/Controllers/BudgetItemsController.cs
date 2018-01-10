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
    ////[Authorize]
    //public class BudgetItemsController : Universal
    //{
    //    private ApplicationDbContext db = new ApplicationDbContext();

    //    // GET: BudgetItems
    //    [Authorize]
    //    public ActionResult Index()
    //    {
    //        ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
    //        var budgetItems = db.BudgetItems.Where(b => b.HouseholdId == null || b.HouseholdId == user.HouseholdId).ToList();
    //        return View(budgetItems);
    //    }

    //    // GET: BudgetItems/Details/5
    //    [Authorize]
    //    public ActionResult Details(int? id)
    //    {
    //        if (id == null) {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        var user = db.Users.Find(User.Identity.GetUserId());
    //        BudgetItem budgetItem = db.BudgetItems.Find(id);
    //        if (user.HouseholdId != null && budgetItem.HouseholdId == user.HouseholdId) {
    //            if (budgetItem == null) {
    //                return HttpNotFound();
    //            }
    //            return View(budgetItem);
    //        }
    //        return RedirectToAction("NotAuthorized", "Account");
    //    }

    //    // GET: BudgetItems/Create
    //    [Authorize]
    //    public ActionResult Create()
    //    {
    //        ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
    //        ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //        ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //        ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //        ViewBag.BudgetPlans = new SelectList(db.BudgetPlans.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //        ViewBag.FrequencyId = new SelectList(db.FrequencyCategories.Where(x => x.HouseholdId == 0 || x.HouseholdId == user.HouseholdId), "Id", "Frequency");
    //        ViewBag.BudgetCategories = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == null || x.HouseholdId == user.HouseholdId), "Id", "Name");
    //        ViewBag.CategoryID = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == null || x.HouseholdId == user.HouseholdId), "Id", "Category");
    //        return View();
    //    }

    //    // POST: BudgetItems/Create
    //    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    //    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    //    [HttpPost]
    //    [Authorize]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult Create([Bind(Include = "Id,Name,Description,AmountActual,AmountBudgeted,CategoryID,FrequencyId,IncomeId,ExpenseId,AccountId,BudgetPlanId")] BudgetItem budgetItem)
    //    {
    //        ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
    //        if (ModelState.IsValid)
    //        {
    //            budgetItem.OwnerId = user.Id;
    //            budgetItem.HouseholdId = user.HouseholdId;
    //            db.BudgetItems.Add(budgetItem);
    //            db.SaveChanges();
    //            return RedirectToAction("Index");
    //        }

    //        ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //        ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //        ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //        ViewBag.BudgetPlans = new SelectList(db.BudgetPlans.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //        ViewBag.FrequencyId = new SelectList(db.FrequencyCategories.Where(x => x.HouseholdId == 0 || x.HouseholdId == user.HouseholdId), "Id", "Frequency");
    //        ViewBag.BudgetCategories = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == null || x.HouseholdId == user.HouseholdId), "Id", "Name");
    //        ViewBag.CategoryID = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == null || x.HouseholdId == user.HouseholdId), "Id", "Category");
    //        return View(budgetItem);
    //    }

    //    // GET: BudgetItems/Edit/5
    //    [Authorize]
    //    public ActionResult Edit(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
    //        BudgetItem budgetItem = db.BudgetItems.Find(id);
    //        if (user.HouseholdId != null && budgetItem.HouseholdId == user.HouseholdId) {
    //            if (budgetItem == null) {
    //                return HttpNotFound();
    //            }
    //            ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //            ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //            ViewBag.BudgetPlans = new SelectList(db.BudgetPlans.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //            ViewBag.FrequencyId = new SelectList(db.FrequencyCategories.Where(x => x.HouseholdId == 0 || x.HouseholdId == user.HouseholdId), "Id", "Frequency");
    //            ViewBag.BudgetCategories = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == null || x.HouseholdId == user.HouseholdId), "Id", "Name");
    //            ViewBag.CategoryID = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == null || x.HouseholdId == user.HouseholdId), "Id", "Category");
    //            return View(budgetItem);
    //        }
    //        return RedirectToAction("NotAuthorized", "Account");
    //    }

    //    // POST: BudgetItems/Edit/5
    //    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    //    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    //    [HttpPost]
    //    [Authorize]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult Edit([Bind(Include = "Id,Name,Description,FrequencyIndex,AmountActual,AmountBudgeted,OwnerId,CategoryID,FrequencyId,HouseholdId,IncomeId,ExpenseId,AccountId,BudgetId")] BudgetItem budgetItem)
    //    {
    //        ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
    //        if (user.HouseholdId != null && budgetItem.HouseholdId == user.HouseholdId) {
    //            if (ModelState.IsValid) {
    //                db.Entry(budgetItem).State = EntityState.Modified;
    //                db.SaveChanges();
    //                return RedirectToAction("Index");
    //            }
    //            ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //            ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //            ViewBag.BudgetPlans = new SelectList(db.BudgetPlans.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
    //            ViewBag.FrequencyId = new SelectList(db.FrequencyCategories.Where(x => x.HouseholdId == 0 || x.HouseholdId == user.HouseholdId), "Id", "Frequency");
    //            ViewBag.BudgetCategories = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == null || x.HouseholdId == user.HouseholdId), "Id", "Name");
    //            ViewBag.CategoryID = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == null || x.HouseholdId == user.HouseholdId), "Id", "Category");
    //            return View(budgetItem);
    //        }
    //        return RedirectToAction("NotAuthorized", "Account");
    //    }

    //    // GET: BudgetItems/Delete/5
    //    [Authorize]
    //    public ActionResult Delete(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        var user = db.Users.Find(User.Identity.GetUserId());
    //        BudgetItem budgetItem = db.BudgetItems.Find(id);
    //        if (user.HouseholdId != null && budgetItem.HouseholdId == user.HouseholdId) {
    //            if (budgetItem == null) {
    //                return HttpNotFound();
    //            }
    //            return View(budgetItem);
    //        }
    //        return RedirectToAction("NotAuthorized", "Account");
    //    }

    //    // POST: BudgetItems/Delete/5
    //    [Authorize]
    //    [HttpPost, ActionName("Delete")]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult DeleteConfirmed(int id)
    //    {
    //        BudgetItem budgetItem = db.BudgetItems.Find(id);
    //        var user = db.Users.Find(User.Identity.GetUserId());
    //        if (user.HouseholdId != null && budgetItem.HouseholdId == user.HouseholdId) {
    //            db.BudgetItems.Remove(budgetItem);
    //            db.SaveChanges();
    //            return RedirectToAction("Index");
    //        }
    //        return RedirectToAction("NotAuthorized", "Account");
    //    }

    //    protected override void Dispose(bool disposing)
    //    {
    //        if (disposing)
    //        {
    //            db.Dispose();
    //        }
    //        base.Dispose(disposing);
    //    }
    //}
}
