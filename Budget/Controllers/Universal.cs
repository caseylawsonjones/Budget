using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Budget.Models {
    public class Universal : Controller {
        public ApplicationDbContext db = new ApplicationDbContext();

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (User.Identity.IsAuthenticated) {
                var user = db.Users.Find(User.Identity.GetUserId());
                List<Account> hhAccounts = db.Accounts.Where(a => a.HouseholdId == user.HouseholdId).ToList();
                ViewBag.overdrawnAccounts = hhAccounts.Where(acc => acc.Balance < 0).ToList();
                List<BudgetPlan> hhBudgets = db.BudgetPlans.Where(b => b.HouseholdId == user.HouseholdId).ToList();
                ViewBag.overrunBudgets = hhBudgets.Where(b => b.AmountActual > b.AmountBudgeted).ToList();
            }

            base.OnActionExecuting(filterContext); //This needs to be here

        }
    }
}