using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models.Categories {
    public class BudgetPlanCategory {

        public BudgetPlanCategory() {
            Transactions = new HashSet<Transaction>();
        }
        public int Id { get; set; }
        public string Category { get; set; }
        public int? HouseholdId { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}