using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Budget.Models.Categories;
using Microsoft.AspNet.Identity;

namespace Budget.Models.Helpers {
    public class CategoryHelper {

        private ApplicationDbContext db = new ApplicationDbContext();

        public void CreateNewBudgetPlanCategory(string categoryName) {
            ApplicationUser user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            BudgetPlanCategory category = new BudgetPlanCategory();
            category.Category = categoryName;
            category.HouseholdId = user.HouseholdId.Value;
            db.BudgetPlanCategories.Add(category);
            db.SaveChanges();
        }

        public void CreateTransactionCategory(string categoryName, string bpCategoryName) {
            ApplicationUser user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            TransactionCategory category = new TransactionCategory();
            category.Category = categoryName;
            category.HouseholdId = user.HouseholdId.Value;
            db.TransactionCategories.Add(category);
            db.SaveChanges();

        }

        public void CreateNewExpenseCategory(string categoryName) {
            ApplicationUser user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            ExpenseCategory category = new ExpenseCategory();
            category.Category = categoryName;
            category.HouseholdId = user.HouseholdId.Value;
            db.ExpenseCategories.Add(category);
            db.SaveChanges();

        }

        public void CreateNewIncomeCategory(string categoryName) {
            ApplicationUser user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            IncomeCategory category = new IncomeCategory();
            category.Category = categoryName;
            category.HouseholdId = user.HouseholdId.Value;
            db.IncomeCategories.Add(category);
            db.SaveChanges();

        }

    }
}