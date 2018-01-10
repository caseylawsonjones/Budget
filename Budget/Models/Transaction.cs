using Budget.Models.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class Transaction {

        public int Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Institution")]
        public string Institution { get; set; }
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Date { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Credit?")]
        public bool IsCredit { get; set; }

        [Display(Name = "Reconciled?")]
        public bool IsReconciled { get; set; }
        [Display(Name = "Discrepancy?")]
        public bool IsDiscrepancy { get; set; }
        [Display(Name = "Reconciled")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ReconciledDate { get; set; }
        [Display(Name = "Reconciliation Explanation")]
        public string ReconciliationExplanation { get; set; }
        [Display(Name = "Reconciled by")]
        public string ReconcilerId { get; set; }
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Reconciled Incorrect Amount")]
        public decimal? ReconciledIncorrectAmount { get; set; }
        // Upon Reconciliation, if there was a Discrepancy, the incorrect account is copied
        // here and the correct is copied to Amount so calculations can remain standardized
        // Reconciliation calls a method that processes the correct amount into the affected
        // Accounts and BudgetItems

        [Display(Name = "Void?")]
        public bool IsVoided { get; set; }

        [Display(Name = "Confirmation #")]
        public string ConfirmationNumber { get; set; }

        [Display(Name = "Updated by")]
        public string UpdaterId { get; set; }

        //FOREIGN KEYS
        [Display(Name = "Household")]
        public int HouseholdId { get; set; }
        [Display(Name = "Household")]
        public virtual Household Household { get; set; }

        [Display(Name = "Originator")]
        public string OwnerId { get; set; }
        [Display(Name = "Originator")]
        public virtual ApplicationUser Owner { get; set; }

        [Display(Name = "Expense")]
        public int? ExpenseId { get; set; }
        [Display(Name = "Expense")]
        public virtual Expense Expense { get; set; }

        [Display(Name = "Income")]
        public int? IncomeId { get; set; }
        [Display(Name = "Income")]
        public virtual Income Income { get; set; }

        [Display(Name = "Account")]
        public int? AccountId { get; set; }
        [Display(Name = "Account")]
        public virtual Account Account { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Display(Name = "Category")]
        public virtual TransactionCategory Category { get; set; }

        [Display(Name = "Budget Category")]
        public int BudgetPlanCategoryId { get; set; }
        [Display(Name = "Budget Category")]
        public virtual BudgetPlanCategory BudgetPlanCategory { get; set; }

        [Display(Name = "Reconciliation")]
        public int? ReconciliationId { get; set; }
        [Display(Name = "Reconciliation")]
        public virtual Reconciliation Reconciliation { get; set; }



        //Many Relationships

        //[Display(Name = "Notes")]
        //public virtual ICollection<Comment> Comments { get; set; }

    }
}