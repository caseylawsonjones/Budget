using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models.Categories {
    public class ExpenseCategory {
        public int Id { get; set; }
        public string Category { get; set; }
        public int HouseholdId { get; set; }

    }
}