using Budget.Models.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models.ViewModels {
    public class BudgetPlanViewModel {
        public List<BudgetPlan> BudgetPlans { get; set; }
        public List<BudgetPlanCategory> BudgetPlanCategories { get; set; }
    }
}