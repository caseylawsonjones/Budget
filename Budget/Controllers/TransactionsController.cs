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
using System.IO;

namespace Budget.Controllers
{
    [Authorize]
    public class TransactionsController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        [Authorize]
        public ActionResult Index()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null) {
                return RedirectToAction("Index", "Manage");
            }
            List<Transaction> transactions = db.Transactions.Where(t => t.HouseholdId == user.HouseholdId).ToList();
            return View(transactions);
        }

        // GET: Transactions/Details/5
        [Authorize]
        public PartialViewResult _Details(int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
                return PartialView("_NotAuthorized", "Account");
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
                return PartialView("_NotAuthorized", "Account");
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || transaction.HouseholdId != user.HouseholdId) {
                ViewBag.ErrorMessage = "Either you belong to no Household, or the resource you are trying to view belongs to another Household.  Either condition prevents you from viewing this resource.";
                return PartialView("_NotAuthorized", "Account");
            }

            return PartialView(transaction);
        }

        // GET: Transactions/Create
        [Authorize]
        public ActionResult Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");

            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            ViewBag.BudgetPlanCategoryId = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            //ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            //ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");            //ViewBag.FrequencyId = new SelectList(db.FrequencyCategories, "Id", "Frequency", transaction.FrequencyId);
            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", transaction.HouseholdId);
            //ViewBag.ReconcilerId = new SelectList(db.Users, "Id", "FirstName", transaction.ReconcilerId);
            //ViewBag.ReconciliationId = new SelectList(db.Reconciliations, "Id", "Explanation", transaction.ReconciliationId);
            //ViewBag.UpdaterId = new SelectList(db.Users, "Id", "FirstName", transaction.UpdaterId);
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Institution,Date,Amount,IsCredit,IsHidden,Payee,ConfirmationNumber,OwnerId,CategoryId,ExpenseId,IncomeId,AccountId,BudgetPlanCategoryId")] Transaction transaction)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                decimal amount = transaction.Amount;
                transaction.Amount = Math.Abs(amount); //Tx entries are marked as Credit or Debit, so we have to avoid receiving negative numerical values
                transaction.UpdaterId = user.Id;
                transaction.HouseholdId = user.HouseholdId.Value;
                transaction.IsVoided = false;

                db.Transactions.Add(transaction);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            ViewBag.BudgetPlanCategoryId = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            //ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            //ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            //ViewBag.FrequencyId = new SelectList(db.FrequencyCategories, "Id", "Frequency", transaction.FrequencyId);
            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", transaction.HouseholdId);
            //ViewBag.ReconcilerId = new SelectList(db.Users, "Id", "FirstName", transaction.ReconcilerId);
            //ViewBag.ReconciliationId = new SelectList(db.Reconciliations, "Id", "Explanation", transaction.ReconciliationId);
            //ViewBag.UpdaterId = new SelectList(db.Users, "Id", "FirstName", transaction.UpdaterId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        [Authorize]
        public PartialViewResult _Edit(int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
                return PartialView("_NotAuthorized", "Account");
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
                return PartialView("_NotAuthorized", "Account");
            }
            if (transaction.IsVoided == true || transaction.IsReconciled == true) {
                ViewBag.ErrorMessage = "Either the Transaction is Voided or has already been Reconciled.  Either condition prevents you from editing this Transaction.";
                return PartialView("_NotAuthorized", "Account");
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || transaction.HouseholdId != user.HouseholdId) {
                ViewBag.ErrorMessage = "Either you belong to no Household, or the resource you are trying to view belongs to another Household.  Either condition prevents you from viewing this resource.";
                return PartialView("_NotAuthorized", "Account");
            }

            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            ViewBag.BudgetPlanCategoryId = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            //ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            //ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", transaction.HouseholdId);
            //ViewBag.ReconcilerId = new SelectList(db.Users, "Id", "FirstName", transaction.ReconcilerId);
            //ViewBag.ReconciliationId = new SelectList(db.Reconciliations, "Id", "Explanation", transaction.ReconciliationId);
            //ViewBag.UpdaterId = new SelectList(db.Users, "Id", "FirstName", transaction.UpdaterId);
            return PartialView(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Institution,Date,Amount,IsCredit,IsVoided,ConfirmationNumber,OwnerId,CategoryId,ExpenseId,IncomeId,AccountId,HouseholdId,BudgetPlanCategoryId")] Transaction transaction)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            Transaction oldTransaction = db.Transactions.AsNoTracking().First(t => t.Id == transaction.Id);
            if (user.HouseholdId == null || transaction.HouseholdId != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }

            if (ModelState.IsValid) {
                decimal amount = transaction.Amount;
                transaction.Amount = Math.Abs(amount); //Tx entries are marked as Credit or Debit, so we have to avoid receiving negative numerical values
                transaction.UpdaterId = user.Id;
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.TransactionCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            ViewBag.BudgetPlanCategoryId = new SelectList(db.BudgetPlanCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == null), "Id", "Category");
            //ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            //ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            return RedirectToAction("Index");
        }

        [Authorize]
        //RECONCILE TRANSACTION
        public PartialViewResult _ReconcileTransaction(int id) {
            Transaction transaction = db.Transactions.Find(id);
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null || transaction.HouseholdId == user.HouseholdId) {
                return PartialView(transaction);
            }
            ViewBag.ErrorMessage = "Either you belong to no Household, or the resource you are trying to view belongs to another Household.  Either condition prevents you from viewing this resource.";
            return PartialView("_NotAuthorized", "Account");
        }

        // CREATE INCOME TRANSACTION RECEIVER
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ReconcileTransactionReceiver([Bind(Include = "Id,Amount,ReconciledIncorrectAmount,ReconciliationExplanation")] Transaction transaction, int id) {
            if (ModelState.IsValid) {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                Transaction transaction2 = db.Transactions.Find(id);
                transaction2.IsDiscrepancy = false;
                if (transaction.Amount != transaction.ReconciledIncorrectAmount) {
                    transaction2.IsDiscrepancy = true;
                }
                transaction2.Amount = transaction.ReconciledIncorrectAmount.Value;
                transaction2.ReconciledIncorrectAmount = transaction.Amount;
                transaction2.IsReconciled = true;
                transaction2.ReconcilerId = user.Id;
                transaction2.ReconciliationExplanation = transaction.ReconciliationExplanation;
                transaction2.ReconciledDate = DateTime.UtcNow;
                db.Entry(transaction2).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return RedirectToAction("Index");
        }

        // VOID TRANSACTION
        [Authorize]
        public ActionResult VoidTransaction(int id) {
            Transaction transaction = db.Transactions.Find(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null || transaction.HouseholdId == user.HouseholdId) {
                transaction.IsVoided = true;
                transaction.AccountId = db.Accounts.First(a => a.Name == "Trash Can Account").Id;
                transaction.Category = db.TransactionCategories.First(b => b.Category == "Voided");
                transaction.BudgetPlanCategory = db.BudgetPlanCategories.First(b => b.Category == "Voided");
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // CREATE INCOME TRANSACTION
        [Authorize]
        public PartialViewResult _CreateIncomeTransaction (int id) {
            Income income = db.Incomes.Find(id);
            return PartialView(income);
        }

        // CREATE INCOME TRANSACTION RECEIVER
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateIncomeTransactionReceiver([Bind(Include = "Id,EstimatedAmount,EndDate")] Income income, int id) {
            // I am borrowing the EndDate property to piggyback the selected date to the Tx creation process.
            TransactionProcessor processor = new TransactionProcessor();
            processor.CreateIncomeTransaction(income.Id, income.EstimatedAmount, income.EndDate.Value);
            return RedirectToAction("Details", "Incomes", new { id = income.Id });
        }

        // CREATE EXPENSE TRANSACTION
        [Authorize]
        public PartialViewResult _CreateExpenseTransaction(int id) {
            Expense expense = db.Expenses.Find(id);
            return PartialView(expense);
        }

        // CREATE EXPENSE TRANSACTION RECEIVER
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateExpenseTransactionReceiver([Bind(Include = "Id,EstimatedAmount,EndDate")] Income expense, int id) {
            // I am borrowing the EndDate property to piggyback the selected date to the Tx creation process.
            TransactionProcessor processor = new TransactionProcessor();
            processor.CreateExpenseTransaction(expense.Id, expense.EstimatedAmount, expense.EndDate.Value);
            return RedirectToAction("Details", "Expenses", new { id = expense.Id });
        }

        //// GET: Transactions/Delete/5
        //[Authorize]
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Transaction transaction = db.Transactions.Find(id);
        //    if (transaction == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    var user = db.Users.Find(User.Identity.GetUserId());
        //    if (user.HouseholdId == null || transaction.HouseholdId != user.HouseholdId) {
        //        return RedirectToAction("NotAuthorized", "Account");
        //    }
        //    return View(transaction);
        //}

        //// POST: Transactions/Delete/5
        //[Authorize]
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Transaction transaction = db.Transactions.Find(id);
        //    var user = db.Users.Find(User.Identity.GetUserId());
        //    if (user.HouseholdId == null || transaction.HouseholdId != user.HouseholdId) {
        //        return RedirectToAction("NotAuthorized", "Account");
        //    }
        //    db.Transactions.Remove(transaction);
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
