using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class Reconciliation {

        public int Id { get; set; }

        [Display(Name = "Created on")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Created { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Reconciled Amount")]
        public decimal ReconciledAmount { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Reported Amount")]
        public decimal OriginalAmount { get; set; }

        [Display(Name = "Discrepancy?")]
        public bool IsDiscrepancy { get; set; }

        [Display(Name = "Voided?")]
        public bool IsVoided { get; set; }

        [Display(Name = "Explanation")]
        public string Explanation { get; set; }

        public int HouseholdId { get; set; }
        public string ReconcilerId { get; set; }
        public int? AccountId { get; set; }
        //public int? TransactionId { get; set; }

        public virtual Household Household { get; set; }

        [Display(Name = "Reconciled by")]
        public virtual ApplicationUser Reconciler { get; set; }

        [Display(Name = "Account Used")]
        public virtual Account Account { get; set; }

        //[Display(Name = "Transaction Used")]
        //public virtual Transaction Transaction { get; set; }


    }
}