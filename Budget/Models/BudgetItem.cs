using Budget.Models.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    //public class BudgetItem {

    //    public BudgetItem() {
    //        BudgetPlans = new HashSet<BudgetPlan>();
    //    }

    //    public int Id { get; set; }
    //    [Display(Name = "Budget Item Name")]
    //    public string Name { get; set; }
    //    [Display(Name = "Budget Item Description")]
    //    public string Description { get; set; }
    //    [Display(Name = "")]
    //    public int? FrequencyIndex { get; set; }

    //    [DataType(DataType.Currency)]
    //    [DisplayFormat(DataFormatString = "{0:C}")]
    //    [Display(Name = "Actual Amount Spent")]
    //    public decimal? AmountActual {
    //        get {
    //            decimal amountActual = 0;
    //            if (Category != null) {
    //            foreach (var tx in Category.Transactions) {
    //                if (tx.IsCredit) {
    //                    amountActual -= tx.Amount;
    //                }
    //                else {
    //                    amountActual += tx.Amount;
    //                }
    //            }
    //        }
    //            return amountActual;
    //        }
    //    }

    //    [DataType(DataType.Currency)]
    //    [DisplayFormat(DataFormatString = "{0:C}")]
    //    [Display(Name = "Budgeted Amount")]
    //    public decimal AmountBudgeted { get; set; }
    //    public bool IsActive { get; set; }

    //    public string OwnerId { get; set; }
    //    public int CategoryId { get; set; }
    //    //public int BudgetCategoryId { get; set; }
    //    public int FrequencyId { get; set; }
    //    public int? HouseholdId { get; set; }
    //    public int? IncomeId { get; set; }
    //    public int? ExpenseId { get; set; }
    //    public int? AccountId { get; set; }

    //    public virtual FrequencyCategory Frequency { get; set; }
    //    [Display(Name = "Budget Item Frequency")]
    //    public virtual Household Household { get; set; }
    //    [Display(Name = "Income Source")]
    //    public virtual Income Income { get; set; }
    //    [Display(Name = "Expense")]
    //    public virtual Expense Expense { get; set; }
    //    [Display(Name = "Account Used")]
    //    public virtual Account Account { get; set; }
    //    //[Display(Name = "Budget Item Category")]
    //    //public virtual BudgetItemCategory Category { get; set; }

    //    [Display(Name = "Budgets")]
    //    public virtual ICollection<BudgetPlan> BudgetPlans { get; set; }
    //}
}