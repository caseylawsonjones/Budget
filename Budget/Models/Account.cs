using Budget.Models.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class Account {
        public Account() {
            Reconciliations = new HashSet<Reconciliation>();
            Transactions = new HashSet<Transaction>();
            Comments = new HashSet<Comment>();
            Incomes = new HashSet<Income>();
            Expenses = new HashSet<Expense>();
            }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name = "Opened on")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Opened { get; set; }
        [Display(Name = "Active?")]
        public bool IsActive { get; set; }
        [Display(Name = "Closed on")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? Closed { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Balance")]
        public decimal Balance {
            get {
                decimal balance = 0;
                foreach (var tx in Transactions) {
                    if (tx.IsCredit) {
                        balance += tx.Amount;
                    }
                    else {
                        balance -= tx.Amount;
                    }
                }
                return balance;
            }
        }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Initial Balance")]
        public decimal InitiationBalance { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Closing Balance")]
        public decimal? ClosingBalance { get; set; }

        [Display(Name = "Acct Number")]
        public string AccountNumber { get; set; }
        [Display(Name = "Last Reconciled on")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? LastReconciled { get; set; }
        public string OwnerId { get; set; }
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public int HouseholdId { get; set; }

        public virtual Household Household { get; set; }
        public virtual AccountCategory Category { get; set; }

        //public virtual ICollection<AccountMonthlyHistoryItem> MonthlyHistoryItems { get; set; }
        public virtual ICollection<Reconciliation> Reconciliations { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Income> Incomes { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }



    }
}