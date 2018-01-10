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
    public class CommentsController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        [Authorize]
        public ActionResult Index()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            var comments = db.Comments.Where(c => c.HouseholdId == user.HouseholdId).ToList();
            return View(comments);
        }

        // GET: Comments/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && comment.HouseholdId == user.HouseholdId) {
                return View(comment);
            }
            return RedirectToAction("NotAuthorized", "Account");
        }

        // GET: Comments/Create
        [Authorize]
        public ActionResult Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.BudgetId = new SelectList(db.BudgetPlans.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.TransactionId = new SelectList(db.Transactions.Where(x => x.HouseholdId == user.HouseholdId && x.IsVoided == false), "Id", "Title");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Body,Created,Modified,OwnerId,HouseholdId,AccountId,BudgetId,TransactionId,IncomeId,ExpenseId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.BudgetId = new SelectList(db.BudgetPlans.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.TransactionId = new SelectList(db.Transactions.Where(x => x.HouseholdId == user.HouseholdId && x.IsVoided == false), "Id", "Title");
            return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && comment.HouseholdId == user.HouseholdId) {
                ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
                ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
                ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
                ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
                ViewBag.BudgetId = new SelectList(db.BudgetPlans.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
                ViewBag.TransactionId = new SelectList(db.Transactions.Where(x => x.HouseholdId == user.HouseholdId && x.IsVoided == false), "Id", "Title");
                return View(comment);
            }
            return RedirectToAction("NotAuthorized", "Account");
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Body,Created,Modified,OwnerId,HouseholdId,AccountId,BudgetId,TransactionId,IncomeId,ExpenseId")] Comment comment)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && comment.HouseholdId == user.HouseholdId) {
                if (ModelState.IsValid) {
                    db.Entry(comment).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
                ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
                ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
                ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
                ViewBag.BudgetId = new SelectList(db.BudgetPlans.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
                ViewBag.TransactionId = new SelectList(db.Transactions.Where(x => x.HouseholdId == user.HouseholdId && x.IsVoided == false), "Id", "Title");
                return View(comment);
            }
            return RedirectToAction("NotAuthorized", "Account");
        }

        // GET: Comments/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && comment.HouseholdId == user.HouseholdId) {
                return View(comment);
            }
            return RedirectToAction("NotAuthorized", "Account");
        }

        // POST: Comments/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null && comment.HouseholdId == user.HouseholdId) {
                db.Comments.Remove(comment);
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
