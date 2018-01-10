using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budget.Models.Helpers {
    [Authorize]
    public class TransactionProcessor {

        private ApplicationDbContext db = new ApplicationDbContext();

        public void AccountInitiatorTransaction(int acctId, decimal acctBalance, ApplicationUser user) {
            Account account = db.Accounts.Find(acctId);
            Transaction transaction = new Transaction();
            decimal balance = acctBalance;
            transaction.Amount = Math.Abs(balance);
            transaction.Title = account.Name + "Balance Initiation Transaction";
            transaction.Description = "This transaction sets the initial balance of the account, based on the amount entered during the account creation.";
            transaction.Date = DateTime.UtcNow;
            if (balance < 0) {
                transaction.IsCredit = false;
            }
            else {
                transaction.IsCredit = true;
            }
            transaction.IsReconciled = false;
            transaction.IsDiscrepancy = false;
            transaction.IsVoided = false;
            transaction.BudgetPlanCategoryId = db.BudgetPlanCategories.First(b => b.Category == "Miscellaneous").Id;
            transaction.HouseholdId = user.HouseholdId.Value;
            transaction.OwnerId = user.Id;
            transaction.AccountId = acctId;
            transaction.CategoryId = db.TransactionCategories.First(b => b.Category == "Paycheck").Id;
            db.Transactions.Add(transaction);
            db.SaveChanges();
        }

        public void CreateIncomeTransaction(int incomeId, decimal amount, DateTime date) {
            ApplicationUser user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            Income income = db.Incomes.Find(incomeId);
            Transaction transaction = new Transaction();

            // Set the property values for the Transaction
            transaction.Title = income.Name;
            transaction.Description = income.Description;
            transaction.Amount = Math.Abs(amount);
            transaction.OwnerId = user.Id;
            transaction.CategoryId = income.CategoryId;
            transaction.HouseholdId = user.HouseholdId.Value;
            transaction.AccountId = income.AccountId;
            transaction.Institution = income.Institution;
            transaction.BudgetPlanCategoryId = income.BudgetPlanCategoryId.Value;
            transaction.Date = date;
            transaction.IsCredit = true;
            transaction.IsReconciled = false;
            transaction.IsVoided = false;
            transaction.IncomeId = income.Id;
            db.Transactions.Add(transaction);
            db.SaveChanges();
        }

        public void CreateExpenseTransaction(int expenseId, decimal amount, DateTime date) {
            ApplicationUser user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            Expense expense = db.Expenses.Find(expenseId);
            Transaction transaction = new Transaction();

            // Set the property values for the Transaction
            transaction.Title = expense.Name;
            transaction.Description = expense.Description;
            transaction.Amount = Math.Abs(amount);
            transaction.OwnerId = user.Id;
            transaction.CategoryId = expense.CategoryId;
            transaction.HouseholdId = user.HouseholdId.Value;
            transaction.AccountId = expense.AccountId;
            transaction.Institution = expense.Institution;
            transaction.BudgetPlanCategoryId = expense.BudgetPlanCategoryId.Value;
            transaction.Date = date;
            transaction.IsCredit = false;
            transaction.IsReconciled = false;
            transaction.IsVoided = false;
            transaction.ExpenseId = expense.Id;
            db.Transactions.Add(transaction);
            db.SaveChanges();
        }


    }
}
    // THE REST OF THIS ENTIRE CLASS WAS MADE OBSOLETE BY PASSIVELY ASSIGNING
    // ACCOUNT AND BUDGET BALANCES IN THE DECLARATION OF THE PROPERTIES IN THE MODELS

    //    public void ProcessNewTransaction(int id) {
    //        Transaction transaction = db.Transactions.Find(id);

    //        // Process the associated Account
    //        if (transaction.AccountId != null) {
    //            Account account = db.Accounts.Find(transaction.AccountId);
    //            if (transaction.IsCredit == true) {
    //                account.Balance += transaction.Amount;
    //            }
    //            else {  // IsDebit
    //                account.Balance -= transaction.Amount;
    //            }
    //            db.Entry(account).State = EntityState.Modified;
    //            db.SaveChanges();
    //        }


    //        // Process the affected BudgetItems
    //        List<BudgetItem> budgetItems = db.BudgetItems.Where(b => b.Category.Id == transaction.Category.Id && b.HouseholdId == transaction.HouseholdId).ToList();
    //        if (budgetItems.Count > 0) {
    //            if (transaction.IsCredit) {
    //                foreach (var item in budgetItems) {
    //                    item.AmountActual -= transaction.Amount;
    //                    db.Entry(item).State = EntityState.Modified;
    //                    db.SaveChanges();
    //                }
    //            }
    //            else {  // IsDebit
    //                foreach (var item in budgetItems) {
    //                    item.AmountActual += transaction.Amount;
    //                    db.Entry(item).State = EntityState.Modified;
    //                    db.SaveChanges();
    //                }
    //            }
    //        }

    //    }

    //    public void ProcessEditedTransaction(Transaction newTransaction, Transaction oldTransaction) {

    //        Account oldAccount = db.Accounts.Find(oldTransaction.AccountId);
    //        Account newAccount = db.Accounts.Find(newTransaction.AccountId);

    //        // Did the Account changed?
    //        if(oldTransaction.AccountId != newTransaction.AccountId || oldTransaction.Amount != newTransaction.Amount) {
    //            // Changing the Account also takes care of the Amount, if it is also changed.
    //            // Remove changes by the Old Transacton to the Old Account (or current Account if it is unchanged)
    //            if (oldTransaction.AccountId != null) {
    //                if (oldTransaction.IsCredit == true) {
    //                    // Remove the previous credit, applied by the Old Transaction
    //                    oldAccount.Balance -= oldTransaction.Amount;
    //                }
    //                else {  // IsDebit
    //                        // Remove the previous debit applied by the Old Transaction
    //                    oldAccount.Balance += oldTransaction.Amount;
    //                }
    //                db.Entry(oldAccount).State = EntityState.Modified;
    //                db.SaveChanges();
    //            }

    //            // Apply changes by the New Transaction to the New Account (or current Account if it is unchanged)
    //            if (newTransaction.AccountId != null) {
    //                if (newTransaction.IsCredit == true) {
    //                    // Apply credit from New Transaction
    //                    newAccount.Balance += newTransaction.Amount;
    //                }
    //                else {  // IsDebit
    //                        // Apply debit from New Transaction
    //                    newAccount.Balance -= newTransaction.Amount;
    //                }
    //                db.Entry(newAccount).State = EntityState.Modified;
    //                db.SaveChanges();
    //            }

    //        }

    //        // Did the Budget Item Category change?
    //        if (oldTransaction.CategoryId != newTransaction.CategoryId || oldTransaction.Amount != newTransaction.Amount) {
    //            // Remove changes by the Old Transaction to the Old Budget Items (or current Budget Item if it is unchanged)
    //            List<BudgetItem> oldBudgetItems = db.BudgetItems.Where(b => b.CategoryId == oldTransaction.CategoryId && b.HouseholdId == oldTransaction.HouseholdId).ToList();
    //            if (oldBudgetItems.Count > 0) {
    //                foreach (var oldItem in oldBudgetItems) {
    //                    if (oldTransaction.IsCredit == true) {
    //                        // Remove the previous credit, applied by the Old Transaction (Budgets work the opposite of Accounts for Credits and Debits)
    //                        oldItem.AmountActual += oldTransaction.Amount;
    //                        db.Entry(oldItem).State = EntityState.Modified;
    //                        db.SaveChanges();
    //                    }
    //                    else {  // IsDebit
    //                            // Remove the previous debit applied by the Old Transaction (Budgets work the opposite of Accounts for Credits and Debits)
    //                        oldItem.AmountActual -= oldTransaction.Amount;
    //                        db.Entry(oldItem).State = EntityState.Modified;
    //                        db.SaveChanges();
    //                    }
    //                }
    //            }

    //            // Apply changes by the New Transaction to the New Budget Items (or current Budget Item if it is unchanged)
    //            List<BudgetItem> newBudgetItems = db.BudgetItems.Where(b => b.CategoryId == newTransaction.CategoryId && b.HouseholdId == newTransaction.HouseholdId).ToList();
    //            if (newBudgetItems.Count > 0) {
    //                foreach (var newItem in newBudgetItems) {
    //                    if (newTransaction.IsCredit == true) {
    //                        // Apply credit from New Transaction (Budgets work the opposite of Accounts for Credits and Debits)
    //                        newItem.AmountActual -= newTransaction.Amount;
    //                        db.Entry(newItem).State = EntityState.Modified;
    //                        db.SaveChanges();
    //                    }
    //                    else {  // IsDebit
    //                            // Apply debit from New Transaction (Budgets work the opposite of Accounts for Credits and Debits)
    //                        newItem.AmountActual += newTransaction.Amount;
    //                        db.Entry(newItem).State = EntityState.Modified;
    //                        db.SaveChanges();
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    public void ProcessVoidedTransaction(int id) {
    //        Transaction transaction = db.Transactions.Find(id);

    //            // MODIFY ASSOCIATED ACCOUNTS
    //        if (transaction.AccountId != null) {

    //            // Remove changes by the Transacton to the associated Account
    //            Account account = db.Accounts.Find(transaction.AccountId);
    //            if (transaction.IsCredit == true) {
    //                // Remove the previous credit, applied by the Transaction
    //                account.Balance -= transaction.Amount;
    //            }
    //            else {  // IsDebit
    //                    // Remove the previous debit applied by the Transaction
    //                account.Balance += transaction.Amount;
    //            }
    //            db.Entry(account).State = EntityState.Modified;
    //            db.SaveChanges();
    //        }

    //        // MODIFY ASSOCIATED BUDGET ITEMS
    //        // Remove changes by the Transaction to the Budget Items
    //        List<BudgetItem> budgetItems = db.BudgetItems.Where(b => b.Category.Id == transaction.Category.Id && b.HouseholdId == transaction.HouseholdId).ToList();
    //        if (budgetItems.Count > 0) {

    //            if (transaction.IsCredit) {
    //                foreach (var item in budgetItems) {
    //                    // Remove the previous credit, applied by the Transaction (Budgets work the opposite of Accounts for Credits and Debits)
    //                    item.AmountActual += transaction.Amount;
    //                    db.Entry(item).State = EntityState.Modified;
    //                    db.SaveChanges();
    //                }
    //            }
    //            else {  // IsDebit
    //                foreach (var item in budgetItems) {
    //                    // Remove the previous debit applied by the Transaction (Budgets work the opposite of Accounts for Credits and Debits)
    //                    item.AmountActual -= transaction.Amount;
    //                    db.Entry(item).State = EntityState.Modified;
    //                    db.SaveChanges();
    //                }
    //            }
    //        }
    //    }

    //    public void ReconcileTransaction(int id) {
    //        Transaction transaction = db.Transactions.Find(id);
    //        if (transaction.IsDiscrepancy) {
    //            decimal oldAmount = transaction.ReconciledIncorrectAmount.Value; // Contains the incorrect amount in case of Reconciliation discrepancy
    //            decimal newAmount = transaction.Amount;

    //            // MODIFY ASSOCIATED ACCOUNT
    //            if (transaction.AccountId != null) {
    //                Account account = db.Accounts.Find(transaction.AccountId);
    //                // Remove changes by the Old Amount to the Account and Apply changes by the New Amount
    //                if (transaction.IsCredit == true) {
    //                    // Remove the previous credit applied by the Old Amount
    //                    account.Balance -= oldAmount;
    //                    // Apply credit from New Amount
    //                    account.Balance += newAmount;
    //                }
    //                else {  // IsDebit
    //                        // Remove the previous debit applied by the Old Amount
    //                    account.Balance += oldAmount;
    //                    // Apply debit from New Amount
    //                    account.Balance -= newAmount;
    //                }
    //                db.Entry(account).State = EntityState.Modified;
    //                db.SaveChanges();
    //            }

    //            // MODIFY ASSOCIATED BUDGET ITEMS
    //            List<BudgetItem> budgetItems = db.BudgetItems.Where(b => b.CategoryId == transaction.CategoryId && b.HouseholdId == transaction.HouseholdId).ToList();
    //            if (budgetItems.Count > 0) {
    //                // Remove changes by the Old Amount to the Budget Items and Apply changes by the New Amount
    //                foreach (var item in budgetItems) {
    //                    if (transaction.IsCredit == true) {
    //                        // Remove the previous credit, applied by the Old Amount (Budgets work the opposite of Accounts for Credits and Debits)
    //                        item.AmountActual += oldAmount;
    //                        // Apply credit from New Transaction (Budgets work the opposite of Accounts for Credits and Debits)
    //                        item.AmountActual -= newAmount;
    //                        db.Entry(item).State = EntityState.Modified;
    //                        db.SaveChanges();
    //                    }
    //                    else {  // IsDebit
    //                            // Remove the previous debit applied by the Old Amount (Budgets work the opposite of Accounts for Credits and Debits)
    //                        item.AmountActual -= oldAmount;
    //                        // Apply debit from New Transaction (Budgets work the opposite of Accounts for Credits and Debits)
    //                        item.AmountActual += newAmount;
    //                        db.Entry(item).State = EntityState.Modified;
    //                        db.SaveChanges();
    //                    }
    //                }
    //            }
    //        }
    //    }
