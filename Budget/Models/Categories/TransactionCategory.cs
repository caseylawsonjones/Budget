using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace Budget.Models.Categories {
    public class TransactionCategory {

        public int Id { get; set; }
        public string Category { get; set; }
        public int? HouseholdId { get; set; }
        public decimal Amount {
            get {
                ApplicationDbContext db = new ApplicationDbContext();
                ApplicationUser user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                DateTime tempTime = DateTime.UtcNow;
                decimal amount = 0;
                if (Transactions != null)
                {
                    foreach (var tx in Transactions.Where(t => t.Date.Year == tempTime.Year && t.Date.Month == tempTime.Month && t.HouseholdId == user.HouseholdId))
                    {
                        if (tx.IsCredit)
                        {
                            amount -= tx.Amount;
                        }
                        else
                        {
                            amount += tx.Amount;
                        }
                    }
                }
                return amount;
            }
        }

        public virtual ICollection<Transaction> Transactions { get; set; }
        //public int BudgetPlanCategoryId { get; set; }
        //public virtual BudgetPlanCategory BudgetPlanCategory { get; set; }
    }
}