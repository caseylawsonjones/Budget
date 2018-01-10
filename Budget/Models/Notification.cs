using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Budget.Models.Categories;
using System.ComponentModel.DataAnnotations;

namespace Budget.Models {

    public class Notification {

        public Notification() {
            //Recipients = new HashSet<ApplicationUser>();
        }

        public int Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public string Body { get; set; }

        [Display(Name = "Created on")]
        public DateTime Created { get; set; }

        [Display(Name = "Originally Delivered on")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime OriginalDeliveryDate { get; set; }

        [Display(Name = "Delayed Until")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CurrentDeliveryDate { get; set; }

        [Display(Name = "Acknowledged on")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateAcknowledged { get; set; }

        [Display(Name = "Acknowledged?")]
        public bool IsAcknowledged { get; set; }

        public int HouseholdId { get; set; }
        public int CategoryId { get; set; }
        public string OwnerId { get; set; }
        public string RecipientId { get; set; } // For group messages, each user will receive a unique Notification
        public int? AccountId { get; set; }
        public int? BudgetPlanId { get; set; }
        public int? TransactionId { get; set; }
        public int? IncomeId { get; set; }
        public int? ExpenseId { get; set; }

        public virtual Household Household { get; set; }

        [Display(Name = "Notification Category")]
        public virtual NotificationCategory Category { get; set; }

        [Display(Name = "Recipient")]
        public virtual ApplicationUser Recipient { get; set; }

        [Display(Name = "Created by")]
        public virtual ApplicationUser Owner { get; set; }

        [Display(Name = "Accont")]
        public virtual Account Account { get; set; }

        [Display(Name = "Budget")]
        public virtual BudgetPlan BudgetPlan { get; set; }

        [Display(Name = "Transaction")]
        public virtual Transaction Transaction { get; set; }

        [Display(Name = "Income")]
        public virtual Income Income { get; set; }

        [Display(Name = "Expense")]
        public virtual Expense Expense { get; set; }

        //[Display(Name = "Recipients")]
        //public virtual ICollection<ApplicationUser> Recipients { get; set; }
    }
}