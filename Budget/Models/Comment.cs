using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class Comment {

        public int Id { get; set; }

        [Display(Name = "Comment")]
        public string Body { get; set; }

        [Display(Name = "Created on")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime Created { get; set; }

        [Display(Name = "Last Modified on")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime? Modified { get; set; }

        public string OwnerId { get; set; }
        public int HouseholdId { get; set; }
        public int? AccountId { get; set; }
        public int? BudgetId { get; set; }
        public int? TransactionId { get; set; }
        public int? IncomeId { get; set; }
        public int? ExpenseId { get; set; }



        [Display(Name = "Created by")]
        public virtual ApplicationUser Owner { get; set; }
        public virtual Household Household { get; set; }
        public virtual Account Account { get; set; }
        public virtual BudgetPlan BudgetPlan { get; set; }
        public virtual Transaction Transaction { get; set; }
        public virtual Income Income { get; set; }
        public virtual Expense Expense { get; set; }
    }
}