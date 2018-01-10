using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models.ViewModels {
    public class CloseAccountViewModel {

        // Incomes and Expenses Associated with this account no longer have an associated account.
        // The returned View will list these so they can be reassigned to a new Account
        public List<Income> IncomesWithNoAssociatedAccount { get; set; }
        public List<Expense> ExpensesWithNoAssociatedAccount { get; set; }

        // The VM needs to know what Account was closed as well as a list of all Active Accounts to choose from
        // in the next view when reassigning the Incomes and Expenses.
        public Account ClosedAccount { get; set; }
        public List<Account> AllActiveAccounts { get; set; }
    }
}