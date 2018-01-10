using Budget.Models.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class Income {

        public int Id { get; set; }

        [Display(Name = "Income Name")]
        public string Name { get; set; }

        [Display(Name = "Income Description")]
        public string Description { get; set; }

        [Display(Name = "Income End Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? EndDate { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Estimated Amount")]
        public decimal EstimatedAmount { get; set; }

        public string Institution { get; set; }
        public bool IsActive { get; set; }

        public string OwnerId { get; set; }
        public int CategoryId { get; set; }
        public int? HouseholdId { get; set; }
        public int AccountId { get; set; }


        [Display(Name = "Income Earner")]
        public virtual ApplicationUser Owner { get; set; }

        [Display(Name = "Transaction Category")]
        public virtual TransactionCategory Category { get; set; }

        //[Display(Name = "Income Category")]
        //public virtual IncomeCategory Category { get; set; }

        public virtual Household Household { get; set; }

        [Display(Name = "Account Used")]
        public virtual Account Account { get; set; }

        [Display(Name = "General Budget Category")]
        public int? BudgetPlanCategoryId { get; set; }
        [Display(Name = "General Budget Category")]
        public virtual BudgetPlanCategory BudgetPlanCategory { get; set; }


        [Display(Name = "Transactions")]
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}