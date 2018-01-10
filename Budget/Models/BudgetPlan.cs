using Budget.Models.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace Budget.Models {

    public class BudgetPlan {

        public int Id { get; set; }

        [Display(Name = "Budget Name")]
        public string Name { get; set; }

        [Display(Name = "Budget Description")]
        public string Description { get; set; }

        [Display(Name = "Currently Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Deactivated on")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? Deactivated { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Amount Budgeted")]
        public decimal AmountBudgeted { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Amount Spent")]
        public decimal AmountActual {
            get {
                ApplicationDbContext db = new ApplicationDbContext();
                ApplicationUser user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                DateTime tempTime = DateTime.UtcNow;
                decimal amountActual = 0;
                if (Category != null) {
                    foreach (var tx in Category.Transactions.Where(t => t.Date.Year == tempTime.Year && t.Date.Month == tempTime.Month && t.HouseholdId == user.HouseholdId)) {
                        if (tx.IsCredit) {
                            amountActual -= tx.Amount;
                        }
                        else {
                            amountActual += tx.Amount;
                        }
                    }
                }
                return amountActual;
            }
        }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Budget Balance")]
        public decimal Variance {
            get {
                decimal variance = 0;
                variance = AmountBudgeted - AmountActual;
                return variance;
            }
        }


        public string OwnerId { get; set; }
        public int CategoryId { get; set; }
        public int HouseholdId { get; set; }

        [Display(Name = "Budget Category")]
        public virtual BudgetPlanCategory Category { get; set; }

        [Display(Name = "Budget Creator")]
        public virtual ApplicationUser Owner { get; set; }

        public virtual Household Household { get; set; }
    }
}