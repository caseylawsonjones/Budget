using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models.ViewModels {
    public class HomeViewModel {

        public string[] Colors {
            get {
                string[] tempArray = {
                    "'rgba(145, 30, 180, 1)'",
                    "'rgba(210, 245, 60, 1)'",
                    "'rgba(128, 0, 0, 1)'",
                    "'rgba(128, 128, 0, 1)'",
                    "'rgba(0, 128, 128, 1)'",
                    "'rgba(170, 255, 195, 1)'",
                    "'rgba(170, 110, 40, 1)'",
                    "'rgba(255, 250, 200, 1)'",
                    "'rgba(0, 0, 128, 1)'",
                    "'rgba(230, 190, 255, 1)'",
                    "'rgba(245, 130, 48, 1)'",
                    "'rgba(230, 25, 75, 1)'",
                    "'rgba(60, 180, 75, 1)'",
                    "'rgba(255, 225, 25, 1)'",
                    "'rgba(0, 130, 200, 1)'",
                    "'rgba(255, 215, 180, 1)'",
                    "'rgba(250, 190, 190, 1)'",
                    "'rgba(70, 240, 240, 1)'",
                    "'rgba(240, 50, 230, 1)'",
                    "'rgba(128, 128, 128, 1)'",
                    "'rgba(255, 255, 255, 1)'",
                    "'rgba(0, 0, 0, 1)'"
                        };
                return tempArray;
            }
        }
        //Expenditures by Budget Category
        public string[] ChartBudgetPlanCategories { get; set; }
        public decimal[] ChartBudgetPlanCategoryAmounts { get; set; }
        public string[] ChartBudgetPlanCategoryColors { get; set; }
        public IDictionary<string, decimal> ChartBudgetPlanCategoryAmountDictionary { get; set; }
        public string[,] ChartBudgetPlanCategoryAmountArray { get; set; }

        //Expenditures by Transaction Category
        public string[] ChartTransactionCategories { get; set; }
        public decimal[] ChartTransactionAmounts { get; set; }
        public string[] ChartTransactionColors { get; set; }

        //Expenditures by Household Member
        public string[] ChartMembers { get; set; }
        public decimal[] ChartMemberAmounts { get; set; }
        public string[] ChartMemberColors { get; set; }
        public Dictionary<string, decimal> ChartMemberAmountDictionary { get; set; }


        ////Frivilous Expenditures
        //public string[] ChartFrivilousCategories { get; set; }
        //public decimal[] ChartFrivilousCategoryAmounts { get; set; }
        //public string[] ChartFrivilousCategoryColors { get; set; }

        //Accounts List
        public List<Account> AccountsList { get; set; }

        //Overrun Budgets List
        public List<BudgetPlan> BudgetPlans { get; set; }

        //Transactions Over $100
        public List<Transaction> LargeTransactions { get; set; }
    }
}