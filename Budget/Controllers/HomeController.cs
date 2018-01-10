using Budget.Models;
using Budget.Models.Categories;
using Budget.Models.Helpers;
using Budget.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Budget.Controllers {
    [Authorize]
    [RequireHttps]
    public class HomeController : Universal {



        public ActionResult Index() {
            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null) {
                return RedirectToAction("Index", "Manage");
            }
            HomeViewModel hvm = new HomeViewModel();

            // Must create a list for each ViewModel Array.  Pass info into the Lists, then send the Lists .ToArray()
            List<string> chartBudgetPlanCategories = new List<string>();
            List<decimal> chartBudgetPlanCategoryAmounts = new List<decimal>();
            List<string> chartBudgetPlanCategoryColors = new List<string>();
            List<string> chartTransactionCategories = new List<string>();
            List<decimal> chartTransactionAmounts = new List<decimal>();
            List<string> chartTransactionColors = new List<string>();
            List<string> chartMembers = new List<string>();
            List<decimal> chartMemberAmounts = new List<decimal>();
            List<string> chartMemberColors = new List<string>();
            IDictionary<string, decimal> chartBudgetPlanCategoryAmountDictionary = new Dictionary<string, decimal>();

            int n = 0;
            decimal tempAmount = 0;

            //
            // LOAD THE CHART ARRAYS FOR THE BUDGETPLAN CATEGORIES
            List<BudgetPlan> budgetPlans = db.BudgetPlans.Where(b => user.HouseholdId == b.HouseholdId).ToList();
            budgetPlans.Sort((y, x) => decimal.Compare(x.AmountActual, y.AmountActual));
            //= budgetPlans.OrderByDescending(x => x.AmountActual).ToList();
            foreach (var item in budgetPlans.Where(b => b.Category.Category != "Voided")) {
                // Add the category to the array
                chartBudgetPlanCategories.Add(item.Category.Category);
                foreach (var tx in item.Category.Transactions.Where(t => t.HouseholdId == user.HouseholdId)) {
                    if (tx.IsCredit) {
                        tempAmount -= tx.Amount;
                    }
                    else {
                        tempAmount += tx.Amount;
                    }
                }
                //Add the amount to the array
                chartBudgetPlanCategoryAmounts.Add(item.AmountActual);
                //chartBudgetPlanCategoryAmountDictionary.Add(item.Category.Category, tempAmount);
            }
            for (n = 0; n < 5; n++) {
                chartBudgetPlanCategoryColors.Add(hvm.Colors[n]);
            }
            hvm.ChartBudgetPlanCategories = chartBudgetPlanCategories.Take(5).ToArray();
            hvm.ChartBudgetPlanCategoryAmounts = chartBudgetPlanCategoryAmounts.Take(5).ToArray();
            hvm.ChartBudgetPlanCategoryColors = chartBudgetPlanCategoryColors.Take(5).ToArray();

            //
            // LOAD THE CHART ARRAYS FOR THE TRANSACTION CATEGORIES
            List<Transaction> transactions = db.Transactions.Where(t => t.HouseholdId == user.HouseholdId).ToList();
            List<TransactionCategory> txCategories = db.TransactionCategories.Where(t => (t.HouseholdId == user.HouseholdId || t.HouseholdId == null) && t.Category != "Voided" && t.Transactions.Any(tx => tx.HouseholdId == user.HouseholdId)).ToList();
            txCategories.Sort((y, x) => decimal.Compare(x.Amount, y.Amount));
            // Cycle through the transactions and get a list of each Category used
            foreach (var item in txCategories) {
                // Add the category to the array
                chartTransactionCategories.Add(item.Category);
                //Add the amount to the array
                chartTransactionAmounts.Add(item.Amount);
            }

            for (n = 0; n < 5; n++)  {
                chartTransactionColors.Add(hvm.Colors[n]);
            }
            hvm.ChartTransactionCategories = chartTransactionCategories.Take(5).ToArray();
            hvm.ChartTransactionAmounts = chartTransactionAmounts.Take(5).ToArray();
            hvm.ChartTransactionColors = chartTransactionColors.Take(5).ToArray();


            // LOAD THE CHART ARRAYS FOR THE MEMBER TRANSACTIONS
            List<ApplicationUser> members = db.Users.Where(t => t.HouseholdId == user.HouseholdId).ToList();
            members.Sort((y, x) => decimal.Compare(x.Amount, y.Amount));
            // Cycle through members and get a list of each Member's Full Name with Transactions
            foreach (var item in members) {
                // Add the category to the array
                chartMembers.Add(item.FullName);
                //Add the amount to the array
                chartMemberAmounts.Add(tempAmount);
            }
            for (n = 0; n < 5; n++) {
                chartMemberColors.Add(hvm.Colors[n]);
            }
            hvm.ChartMembers = chartMembers.Take(5).ToArray();
            hvm.ChartMemberAmounts = chartMemberAmounts.Take(5).ToArray();
            hvm.ChartMemberColors = chartMemberColors.Take(5).ToArray();

            // LOAD THE CHART CATEGORY DICTIONARY
            //for (n = 0; n < chartBudgetPlanCategories.Count; n++) {
            //    chartBudgetPlanCategoryAmountDictionary.Add(hvm.ChartBudgetPlanCategories[n], hvm.ChartBudgetPlanCategoryAmounts[n]);
            //}
            //hvm.ChartBudgetPlanCategoryAmountDictionary = chartBudgetPlanCategoryAmountDictionary;
            //hvm.ChartBudgetPlanCategoryAmountArray = chartBudgetPlanCategoryAmountDictionary.Values.ToArray();

            // LOAD THE LIST FOR ACCOUNTS
            hvm.AccountsList = db.Accounts.Where(a => a.HouseholdId == user.HouseholdId && a.Category.Category != "Closed").ToList();
            hvm.AccountsList.Sort((x, y) => decimal.Compare(x.Balance, y.Balance));

            // LOAD THE LIST FOR OVERRUN BUDGETS
            hvm.BudgetPlans = budgetPlans.Where(b => b.AmountBudgeted < b.AmountActual).ToList();

            // LOAD THE LIST FOR TRANSACTIONS > $100
            hvm.LargeTransactions = transactions.Where(t => t.Amount > 100 && t.IsCredit == false).ToList();
            hvm.LargeTransactions.Sort((x, y) => string.Compare(x.Category.Category, y.Category.Category));

            return View(hvm);

        }

        public ActionResult Index4() {
            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            HomeViewModel hvm = new HomeViewModel();

            //
            // LOAD THE CHART ARRAYS FOR THE BUDGETPLAN CATEGORIES
            List<BudgetPlan> budgetPlans = db.BudgetPlans.Where(b => user.HouseholdId == b.HouseholdId && b.Category.Category != "Voided").ToList();
            IDictionary<string, decimal> chartBudgetPlanCategoryAmountDictionary = new Dictionary<string, decimal>();
            // Cycle through budgetPlans and get a list of each Category used
            // We want the list sorted by Amount, and are using a dictionary because Amount is 
            // dynamically calculated in the Model and cannot be used to sort the list sense the value is transient/ethereal/virtual/temporary..
            foreach (var item in budgetPlans) {
                chartBudgetPlanCategoryAmountDictionary.Add(item.Category.Category, item.AmountActual);
            }
            // Sort the dictionary by the value
            chartBudgetPlanCategoryAmountDictionary.OrderByDescending(bc => bc.Value);
            // Create two temporary arrays to split the keys and values from the dictionary
            string[] chartBudgetPlanCategories = new string[chartBudgetPlanCategoryAmountDictionary.Count];
            decimal[] chartBudgetPlanCategoryAmounts = new decimal[chartBudgetPlanCategoryAmountDictionary.Count];
            // Copy the dictionary keys and values to the corresponding arrays
            chartBudgetPlanCategoryAmountDictionary.Keys.CopyTo(chartBudgetPlanCategories, 0);
            chartBudgetPlanCategoryAmountDictionary.Values.CopyTo(chartBudgetPlanCategoryAmounts, 0);
            // Assign the values for the corresponding arrays in the View Model
            hvm.ChartBudgetPlanCategories = chartBudgetPlanCategories;
            hvm.ChartBudgetPlanCategoryAmounts = chartBudgetPlanCategoryAmounts;

            
            // LOAD THE CHART ARRAYS FOR THE TRANSACTION CATEGORIES
            List<TransactionCategory> txCategories = db.TransactionCategories.Where(t => (t.HouseholdId == user.HouseholdId || t.HouseholdId == null) && t.Category != "Voided").ToList();
            IDictionary<string, decimal> chartTransactionCategoryAmountDictionary = new Dictionary<string, decimal>();
            // Cycle through txCategories and get a list of each Category used
            // We want the list sorted by Amount, and are using a dictionary because Amount is 
            // dynamically calculated in the Model and cannot be used to sort the list sense the value is transient/ethereal/virtual/temporary..
            foreach (var item in txCategories) {
                chartTransactionCategoryAmountDictionary.Add(item.Category, item.Amount);
            }
            // Sort the dictionary by the value
            chartTransactionCategoryAmountDictionary.OrderByDescending(tc => tc.Value);
            // Create two temporary arrays to split the keys and values from the dictionary
            string[] chartTransactionCategories = new string[chartTransactionCategoryAmountDictionary.Count];
            decimal[] chartTransactionAmounts = new decimal[chartTransactionCategoryAmountDictionary.Count];
            // Copy the dictionary keys and values to the corresponding arrays
            chartTransactionCategoryAmountDictionary.Keys.CopyTo(chartTransactionCategories, 0);
            chartTransactionCategoryAmountDictionary.Values.CopyTo(chartTransactionAmounts, 0);
            // Assign the values for the corresponding arrays in the View Model
            hvm.ChartTransactionCategories = chartTransactionCategories;
            hvm.ChartTransactionAmounts = chartTransactionAmounts;


            // LOAD THE CHART ARRAYS FOR THE MEMBER TRANSACTIONS
            List<ApplicationUser> members = db.Users.Where(t => (t.HouseholdId == user.HouseholdId || t.HouseholdId == null) && t.FirstName != null).ToList();
            Dictionary<string, decimal> chartMemberAmountDictionary = new Dictionary<string, decimal>();
            // Cycle through members and get a list of each Member's Full Name with Transactions
            // We want the list sorted by Amount, and are using a dictionary because Amount is 
            // dynamically calculated in the Model and cannot be used to sort the list sense the value is transient/ethereal/virtual/temporary..
            foreach (var item in members) {
                chartMemberAmountDictionary.Add(item.FullName, item.Amount);
            }
            // Sort the dictionary by the value
            chartMemberAmountDictionary.OrderByDescending(m => m.Value);
            // Create two temporary arrays to split the keys and values from the dictionary
            string[] chartMembers = new string[chartMemberAmountDictionary.Count];
            decimal[] chartMemberAmounts = new decimal[chartMemberAmountDictionary.Count];
            // Copy the dictionary keys and values to the corresponding arrays
            chartMemberAmountDictionary.Keys.CopyTo(chartMembers, 0);
            chartMemberAmountDictionary.Values.CopyTo(chartMemberAmounts, 0);
            // Assign the values for the corresponding arrays in the View Model
            hvm.ChartMembers = chartMembers;
            hvm.ChartMemberAmounts = chartMemberAmounts;


            // LOAD THE CHART CATEGORY DICTIONARY
            //for (n = 0; n < chartBudgetPlanCategories.Count; n++) {
            //    chartBudgetPlanCategoryAmountDictionary.Add(hvm.ChartBudgetPlanCategories[n], hvm.ChartBudgetPlanCategoryAmounts[n]);
            //}
            //hvm.ChartBudgetPlanCategoryAmountDictionary = chartBudgetPlanCategoryAmountDictionary;
            //hvm.ChartBudgetPlanCategoryAmountArray = chartBudgetPlanCategoryAmountDictionary.Values.ToArray();

            // LOAD THE LIST FOR ACCOUNTS
            hvm.AccountsList = db.Accounts.Where(a => a.HouseholdId == user.HouseholdId).ToList();

            // LOAD THE LIST FOR OVERRUN BUDGETS
            hvm.BudgetPlans = db.BudgetPlans.Where(b => b.HouseholdId == user.HouseholdId).ToList();

            // LOAD THE LIST FOR TRANSACTIONS > $100
            List<Transaction> transactions = db.Transactions.Where(t => t.HouseholdId == user.HouseholdId).ToList();
            hvm.LargeTransactions = transactions.Where(t => t.Amount > 100).OrderByDescending(x => x.Amount).ToList();

            return View(hvm);

        }


        public ActionResult Index2() {
            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            HomeViewModel hvm = new HomeViewModel();

            // Must create a list for each ViewModel Array.  Pass info into the Lists, then send the Lists .ToArray()
            List<string> chartBudgetPlanCategories = new List<string>();
            List<decimal> chartBudgetPlanCategoryAmounts = new List<decimal>();
            List<string> chartBudgetPlanCategoryColors = new List<string>();
            List<string> chartTransactionCategories = new List<string>();
            List<decimal> chartTransactionAmounts = new List<decimal>();
            List<string> chartTransactionColors = new List<string>();
            List<string> chartMembers = new List<string>();
            List<decimal> chartMemberAmounts = new List<decimal>();
            List<string> chartMemberColors = new List<string>();
            IDictionary<string, decimal> chartBudgetPlanCategoryAmountDictionary = new Dictionary<string, decimal>();


            int n = 0;
            decimal tempAmount = 0;
            // LOAD THE CHART ARRAYS FOR THE BUDGETPLAN CATEGORIES
            List<BudgetPlan> budgetPlans = db.BudgetPlans.Where(b => user.HouseholdId == b.HouseholdId).ToList();
            foreach (var item in budgetPlans) {
                // Add the category to the array
                chartBudgetPlanCategories.Add(item.Category.Category);
                foreach (var tx in item.Category.Transactions) {
                    if (tx.IsCredit) {
                        tempAmount -= tx.Amount;
                    }
                    else {
                        tempAmount += tx.Amount;
                    }
                }
                //Add the amount to the array
                chartBudgetPlanCategoryAmounts.Add(tempAmount);
                tempAmount = 0;
            }
            for (n = 0; n < chartBudgetPlanCategories.Count(); n++) {
                chartBudgetPlanCategoryColors.Add(hvm.Colors[n]);
            }
            hvm.ChartBudgetPlanCategories = chartBudgetPlanCategories.ToArray();
            hvm.ChartBudgetPlanCategoryAmounts = chartBudgetPlanCategoryAmounts.ToArray();
            hvm.ChartBudgetPlanCategoryColors = chartBudgetPlanCategoryColors.ToArray();

            // LOAD THE CHART ARRAYS FOR THE TRANSACTION CATEGORIES
            List<Transaction> transactions = db.Transactions.Where(t => t.HouseholdId == user.HouseholdId).ToList();
            List<string> txCats = new List<string>();
            // Cycle through the transactions and get a list of each Category used
            foreach (var tx in transactions) {
                if (!txCats.Any(c => c == tx.Category.Category)) {
                    txCats.Add(tx.Category.Category);
                }
            }
            tempAmount = 0;
            foreach (var cat in txCats) { // Cycle through the list of used Categories
                // Calculate the Tx Amount for all Tx's with that category
                foreach (var tx in transactions.Where(t => t.Category.Category == cat)) {
                    if (tx.IsCredit) {
                        tempAmount -= tx.Amount;
                    }
                    else {
                        tempAmount += tx.Amount;
                    }
                }
                // Add the corresponding Category and Amount to the same index location in the Array
                chartTransactionCategories.Add(cat);
                chartTransactionAmounts.Add(tempAmount);
            }
            for (n = 0; n < chartTransactionCategories.Count(); n++) {
                chartTransactionColors.Add(hvm.Colors[n]);
            }
            hvm.ChartTransactionCategories = chartTransactionCategories.ToArray();
            hvm.ChartTransactionAmounts = chartTransactionAmounts.ToArray();
            hvm.ChartTransactionColors = chartTransactionColors.ToArray();

            // LOAD THE CHART ARRAYS FOR THE MEMBER TRANSACTIONS
            List<string> members = new List<string>();
            // Cycle through transactions and get a list of each Member with Transactions
            foreach (var tx in transactions) {
                if (!members.Any(m => m == tx.Owner.FullName)) {
                    members.Add(tx.Owner.FullName);
                }
            }
            tempAmount = 0;
            foreach (var mem in members) { // Cycle through the list of Members
                // Calculate the Tx Amount for all Tx's by that Member
                foreach (var tx in transactions.Where(t => t.Owner.FullName == mem)) {
                    if (tx.IsCredit) {
                        tempAmount -= tx.Amount;
                    }
                    else {
                        tempAmount += tx.Amount;
                    }
                }
                // Add teh corresponding Member and Amount to the same index location in the Array
                chartMembers.Add(mem);
                chartMemberAmounts.Add(tempAmount);
            }
            for (n = 0; n < chartMembers.Count(); n++) {
                chartMemberColors.Add(hvm.Colors[n]);
            }
            hvm.ChartMembers = chartMembers.ToArray();
            hvm.ChartMemberAmounts = chartMemberAmounts.ToArray();
            hvm.ChartMemberColors = chartMemberColors.ToArray();

            // LOAD THE CHART CATEGORY DICTIONARY
            for (n = 0; n < chartBudgetPlanCategories.Count; n++)
            {
                chartBudgetPlanCategoryAmountDictionary.Add(hvm.ChartBudgetPlanCategories[n], hvm.ChartBudgetPlanCategoryAmounts[n]);
            }
            hvm.ChartBudgetPlanCategoryAmountDictionary = chartBudgetPlanCategoryAmountDictionary;
            //hvm.ChartBudgetPlanCategoryAmountArray = chartBudgetPlanCategoryAmountDictionary.Values.ToArray();

            // LOAD THE LIST FOR ACCOUNTS
            hvm.AccountsList = db.Accounts.Where(a => a.HouseholdId == user.HouseholdId).ToList();

            // LOAD THE LIST FOR OVERRUN BUDGETS
            hvm.BudgetPlans = db.BudgetPlans.Where(b => b.HouseholdId == user.HouseholdId).ToList();

            // LOAD THE LIST FOR TRANSACTIONS > $100
            hvm.LargeTransactions = transactions.Where(t => t.Amount > 100).ToList();

            return View(hvm);

        }

        //public ActionResult About() {

        //    return View();
        //}

        //public ActionResult Contact() {
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Contact(EmailModel model) {
        //    if (ModelState.IsValid) {
        //        try {
        //            var body = "<p>Email From: <bold>{0}</bold> ({1})</p><p>Message:</p><p>{2}</p>";
        //            var from = "MyPortfolio<antonio@raynor.com>";
        //            model.Body = "This is a message from your portfolio site.  The name and the email of the contacting person is above.";

        //            var email = new MailMessage(from,
        //                        ConfigurationManager.AppSettings["emailto"]) {
        //                Subject = "Portfolio Contact Email",
        //                Body = string.Format(body, model.FromName, model.FromEmail,
        //                                     model.Body),
        //                IsBodyHtml = true
        //            };

        //            var svc = new PersonalEmail();
        //            await svc.SendAsync(email);

        //            return RedirectToAction("Sent");
        //        }
        //        catch (Exception ex) {
        //            Console.WriteLine(ex.Message);
        //            await Task.FromResult(0);
        //        }
        //    }
        //    return View(model);
        //}
    }
}