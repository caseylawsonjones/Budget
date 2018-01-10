namespace Budget.Migrations
{
    using Budget.Models;
    using Budget.Models.Categories;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            // DEFAULT ROLE CREATION
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            ApplicationDbContext db = new ApplicationDbContext();

            if (!context.Roles.Any(r => r.Name == "Head")) {
                roleManager.Create(new IdentityRole { Name = "Head" });
            }
            if (!context.Roles.Any(r => r.Name == "Adult")) {
                roleManager.Create(new IdentityRole { Name = "Adult" });
            }
            if (!context.Roles.Any(r => r.Name == "Member")) {
                roleManager.Create(new IdentityRole { Name = "Member" });
            }
            if (!context.Roles.Any(r => r.Name == "DemoAccount")) {
                roleManager.Create(new IdentityRole { Name = "DemoAccount" });
            }

            // DEFAULT USER CREATION
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "caseylawsonjones@yahoo.com")) {
                userManager.Create(new ApplicationUser {
                    UserName = "caseylawsonjones@yahoo.com",
                    FirstName = "Casey",
                    LastName = "Jones",
                    Email = "caseylawsonjones@yahoo.com",
                    EmailConfirmed = true,
                }, "CoderFoundry1!");
            }
            if (!context.Users.Any(u => u.Email == "mjaang@coderfoundry.com")) {
                userManager.Create(new ApplicationUser {
                    UserName = "mjaang@coderfoundry.com",
                    FirstName = "Mark",
                    LastName = "Jaang",
                    Email = "mjaang@coderfoundry.com",
                    EmailConfirmed = true,
                }, "Budgeter8**");
            }
            if (!context.Users.Any(u => u.Email == "rchapman@coderfoundry.com")) {
                userManager.Create(new ApplicationUser {
                    UserName = "rchapman@coderfoundry.com",
                    FirstName = "Ryan",
                    LastName = "Chapman",
                    Email = "rchapman@coderfoundry.com",
                    EmailConfirmed = true,
                }, "Budgeter8**");
            }
            if (!context.Users.Any(u => u.Email == "DemoHead@coderfoundry.com")) {
                userManager.Create(new ApplicationUser {
                    UserName = "DemoHead@coderfoundry.com",
                    FirstName = "Demo",
                    LastName = "Head",
                    Email = "DemoHead@coderfoundry.com",
                    EmailConfirmed = true,
                }, "Budgeter8**");
            }
            if (!context.Users.Any(u => u.Email == "DemoAdult@coderfoundry.com")) {
                userManager.Create(new ApplicationUser {
                    UserName = "DemoAdult@coderfoundry.com",
                    FirstName = "Demo",
                    LastName = "Adult",
                    Email = "DemoAdult@coderfoundry.com",
                    EmailConfirmed = true,
                }, "Budgeter8**");
            }
            if (!context.Users.Any(u => u.Email == "DemoMember@coderfoundry.com")) {
                userManager.Create(new ApplicationUser {
                    UserName = "DemoMember@coderfoundry.com",
                    FirstName = "Demo",
                    LastName = "Member",
                    Email = "DemoMember@coderfoundry.com",
                    EmailConfirmed = true,
                }, "Budgeter8**");
            }


            // ASSIGNMENT OF ROLES TO EACH INITIAL USER

            //Casey Jones
            var userID = userManager.FindByEmail("caseylawsonjones@yahoo.com").Id;
            userManager.AddToRole(userID, "Head");
            userID = userManager.FindByEmail("caseylawsonjones@yahoo.com").Id;
            userManager.AddToRole(userID, "Adult");
            userID = userManager.FindByEmail("caseylawsonjones@yahoo.com").Id;
            userManager.AddToRole(userID, "Member");


            //Mark Jaang
            userID = userManager.FindByEmail("mjaang@coderfoundry.com").Id;
            userManager.AddToRole(userID, "Head");
            userID = userManager.FindByEmail("mjaang@coderfoundry.com").Id;
            userManager.AddToRole(userID, "Adult");
            userID = userManager.FindByEmail("mjaang@coderfoundry.com").Id;
            userManager.AddToRole(userID, "Member");

            //Ryan Chapman
            userID = userManager.FindByEmail("rchapman@coderfoundry.com").Id;
            userManager.AddToRole(userID, "Head");
            userID = userManager.FindByEmail("rchapman@coderfoundry.com").Id;
            userManager.AddToRole(userID, "Adult");
            userID = userManager.FindByEmail("rchapman@coderfoundry.com").Id;
            userManager.AddToRole(userID, "Member");

            // ASSIGNMENT OF DEMO USERS FOR EACH ROLE

            //DemoHead
            userID = userManager.FindByEmail("DemoHead@coderfoundry.com").Id;
            userManager.AddToRole(userID, "Head");
            userID = userManager.FindByEmail("DemoHead@coderfoundry.com").Id;
            userManager.AddToRole(userID, "Adult");
            userID = userManager.FindByEmail("DemoHead@coderfoundry.com").Id;
            userManager.AddToRole(userID, "Member");
            userID = userManager.FindByEmail("DemoHead@coderfoundry.com").Id;
            userManager.AddToRole(userID, "DemoAccount");

            //DemoAdult
            userID = userManager.FindByEmail("DemoAdult@coderfoundry.com").Id;
            userManager.AddToRole(userID, "Adult");
            userID = userManager.FindByEmail("DemoAdult@coderfoundry.com").Id;
            userManager.AddToRole(userID, "Member");
            userID = userManager.FindByEmail("DemoAdult@coderfoundry.com").Id;
            userManager.AddToRole(userID, "DemoAccount");

            //DemoMember
            userID = userManager.FindByEmail("DemoMember@coderfoundry.com").Id;
            userManager.AddToRole(userID, "Member");
            userID = userManager.FindByEmail("DemoMember@coderfoundry.com").Id;
            userManager.AddToRole(userID, "DemoAccount");


            //ACCOUNT TYPE/CATEGORY SEEDING
            if (!context.AccountCategories.Any(p => p.Category == "Savings")) {
                var ac = new AccountCategory();
                ac.Category = "Savings";
                context.AccountCategories.Add(ac);
            }
            if (!context.AccountCategories.Any(p => p.Category == "Checking")) {
                var ac = new AccountCategory();
                ac.Category = "Checking";
                context.AccountCategories.Add(ac);
            }
            if (!context.AccountCategories.Any(p => p.Category == "Mortgage")) {
                var ac = new AccountCategory();
                ac.Category = "Mortgage";
                context.AccountCategories.Add(ac);
            }
            if (!context.AccountCategories.Any(p => p.Category == "Credit Card")) {
                var ac = new AccountCategory();
                ac.Category = "Credit Card";
                context.AccountCategories.Add(ac);
            }
            if (!context.AccountCategories.Any(p => p.Category == "Investment")) {
                var ac = new AccountCategory();
                ac.Category = "Investment";
                context.AccountCategories.Add(ac);
            }
            if (!context.AccountCategories.Any(p => p.Category == "Retirement")) {
                var ac = new AccountCategory();
                ac.Category = "Retirement";
                context.AccountCategories.Add(ac);
            }
            if (!context.AccountCategories.Any(p => p.Category == "Student Loan")) {
                var ac = new AccountCategory();
                ac.Category = "Student Loan";
                context.AccountCategories.Add(ac);
            }
            if (!context.AccountCategories.Any(p => p.Category == "Loan")) {
                var ac = new AccountCategory();
                ac.Category = "Loan";
                context.AccountCategories.Add(ac);
            }


            //EXPENSE TYPE/CATEGORY SEEDING
            if (!context.ExpenseCategories.Any(p => p.Category == "Utilities")) {
                var ec = new ExpenseCategory();
                ec.Category = "Utilities";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "Mobile Devices")) {
                var ec = new ExpenseCategory();
                ec.Category = "Mobile Devices";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "Rent")) {
                var ec = new ExpenseCategory();
                ec.Category = "Rent";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "Mortgage")) {
                var ec = new ExpenseCategory();
                ec.Category = "Mortgage";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "Credit Card Payment")) {
                var ec = new ExpenseCategory();
                ec.Category = "Credit Card Payment";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "Insurance")) {
                var ec = new ExpenseCategory();
                ec.Category = "Insurance";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "Daycare")) {
                var ec = new ExpenseCategory();
                ec.Category = "Daycare";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "Student Loan Payment")) {
                var ec = new ExpenseCategory();
                ec.Category = "Student Loan Payment";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "Loan Payment")) {
                var ec = new ExpenseCategory();
                ec.Category = "Loan Payment";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "Tuition")) {
                var ec = new ExpenseCategory();
                ec.Category = "Tuition";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "School Supplies")) {
                var ec = new ExpenseCategory();
                ec.Category = "School Supplies";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "Taxes")) {
                var ec = new ExpenseCategory();
                ec.Category = "Taxes";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "Investment")) {
                var ec = new ExpenseCategory();
                ec.Category = "Investment";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "Retirement Contribution")) {
                var ec = new ExpenseCategory();
                ec.Category = "Retirement Contribution";
                context.ExpenseCategories.Add(ec);
            }

            if (!context.ExpenseCategories.Any(p => p.Category == "Child Support Payment")) {
                var ec = new ExpenseCategory();
                ec.Category = "Child Support Payment";
                context.ExpenseCategories.Add(ec);
            }
            if (!context.ExpenseCategories.Any(p => p.Category == "Alimony Payment")) {
                var ec = new ExpenseCategory();
                ec.Category = "Alimony Payment";
                context.ExpenseCategories.Add(ec);
            }

            //INCOME TYPE/CATEGORY SEEDING
            if (!context.IncomeCategories.Any(p => p.Category == "Paycheck")) {
                var ic = new IncomeCategory();
                ic.Category = "Paycheck";
                context.IncomeCategories.Add(ic);
            }
            if (!context.IncomeCategories.Any(p => p.Category == "Investment Return")) {
                var ic = new IncomeCategory();
                ic.Category = "Investment Return";
                context.IncomeCategories.Add(ic);
            }
            if (!context.IncomeCategories.Any(p => p.Category == "Retirement Benefit")) {
                var ic = new IncomeCategory();
                ic.Category = "Retirement Benefit";
                context.IncomeCategories.Add(ic);
            }
            if (!context.IncomeCategories.Any(p => p.Category == "Loan Dispursement")) {
                var ic = new IncomeCategory();
                ic.Category = "Loan Dispursement";
                context.IncomeCategories.Add(ic);
            }
            if (!context.IncomeCategories.Any(p => p.Category == "Unemployment")) {
                var ic = new IncomeCategory();
                ic.Category = "Unemployment";
                context.IncomeCategories.Add(ic);
            }
            if (!context.IncomeCategories.Any(p => p.Category == "Child Support")) {
                var ic = new IncomeCategory();
                ic.Category = "Child Support";
                context.IncomeCategories.Add(ic);
            }
            if (!context.IncomeCategories.Any(p => p.Category == "Alimony")) {
                var ic = new IncomeCategory();
                ic.Category = "Alimony";
                context.IncomeCategories.Add(ic);
            }
            if (!context.IncomeCategories.Any(p => p.Category == "Social Security")) {
                var ic = new IncomeCategory();
                ic.Category = "Social Security";
                context.IncomeCategories.Add(ic);
            }
            if (!context.IncomeCategories.Any(p => p.Category == "Tax Return")) {
                var ic = new IncomeCategory();
                ic.Category = "Tax Return";
                context.IncomeCategories.Add(ic);
            }
            if (!context.IncomeCategories.Any(p => p.Category == "Student Loan")) {
                var ic = new IncomeCategory();
                ic.Category = "Student Loan";
                context.IncomeCategories.Add(ic);
            }
            if (!context.IncomeCategories.Any(p => p.Category == "Reverse Mortgage")) {
                var ic = new IncomeCategory();
                ic.Category = "Reverse Mortgage";
                context.IncomeCategories.Add(ic);
            }

            // TRANSACTION TYPE/CATEGORY SEEDING
            // Expenses
            if (!context.TransactionCategories.Any(p => p.Category == "Gas")) {
                var tc = new TransactionCategory();
                tc.Category = "Gas";
                tc.BudgetPlanCategoryId = db.BudgetPlanCategories.First(b => b.Category == "Transportation" && b.HouseholdId == null).Id;
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Food")) {
                var tc = new TransactionCategory();
                tc.Category = "Food";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Utilities")) {
                var tc = new TransactionCategory();
                tc.Category = "Utilities";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Mobile Devices")) {
                var tc = new TransactionCategory();
                tc.Category = "Mobile Devices";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Rent")) {
                var tc = new TransactionCategory();
                tc.Category = "Rent";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Mortgage")) {
                var tc = new TransactionCategory();
                tc.Category = "Mortgage";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Credit Card Payment")) {
                var tc = new TransactionCategory();
                tc.Category = "Credit Card Payment";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Insurance")) {
                var tc = new TransactionCategory();
                tc.Category = "Insurance";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Daycare")) {
                var tc = new TransactionCategory();
                tc.Category = "Daycare";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Student Loan Payment")) {
                var tc = new TransactionCategory();
                tc.Category = "Student Loan Payment";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Loan Payment")) {
                var tc = new TransactionCategory();
                tc.Category = "Loan Payment";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Tuition")) {
                var tc = new TransactionCategory();
                tc.Category = "Tuition";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "School Supplies")) {
                var tc = new TransactionCategory();
                tc.Category = "School Supplies";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Taxes")) {
                var tc = new TransactionCategory();
                tc.Category = "Taxes";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Investment")) {
                var tc = new TransactionCategory();
                tc.Category = "Investment";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Child Support Payment")) {
                var tc = new TransactionCategory();
                tc.Category = "Child Support Payment";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Alimony Payment")) {
                var tc = new TransactionCategory();
                tc.Category = "Alimony Payment";
                context.TransactionCategories.Add(tc);
            }
            // Incomes
            if (!context.TransactionCategories.Any(p => p.Category == "Paycheck")) {
                var tc = new TransactionCategory();
                tc.Category = "Paycheck";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Investment Return")) {
                var tc = new TransactionCategory();
                tc.Category = "Investment Return";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Retirement")) {
                var tc = new TransactionCategory();
                tc.Category = "Retirement";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Loan Dispursement")) {
                var tc = new TransactionCategory();
                tc.Category = "Loan Dispursement";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Unemployment")) {
                var tc = new TransactionCategory();
                tc.Category = "Unemployment";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Child Support")) {
                var tc = new TransactionCategory();
                tc.Category = "Child Support";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Alimony")) {
                var tc = new TransactionCategory();
                tc.Category = "Alimony";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Social Security")) {
                var tc = new TransactionCategory();
                tc.Category = "Social Security";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Tax Return")) {
                var tc = new TransactionCategory();
                tc.Category = "Tax Return";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Student Loan Dispursement")) {
                var tc = new TransactionCategory();
                tc.Category = "Student Loan Dispursement";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Reverse Mortgage")) {
                var tc = new TransactionCategory();
                tc.Category = "Reverse Mortgage";
                context.TransactionCategories.Add(tc);
            }
            // Other
            if (!context.TransactionCategories.Any(p => p.Category == "Clothing")) {
                var tc = new TransactionCategory();
                tc.Category = "Clothing";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Home Repair/Maintenance")) {
                var tc = new TransactionCategory();
                tc.Category = "Home Repair/Maintenance";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Auto Repair/Maintenance")) {
                var tc = new TransactionCategory();
                tc.Category = "Auto Repair/Maintenance";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Money Transfer")) {
                var tc = new TransactionCategory();
                tc.Category = "Money Transfer";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Travel/Vacation")) {
                var tc = new TransactionCategory();
                tc.Category = "Travel/Vacation";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Pharmacy")) {
                var tc = new TransactionCategory();
                tc.Category = "Pharmacy";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Healthcare")) {
                var tc = new TransactionCategory();
                tc.Category = "Healthcare";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Restaurant")) {
                var tc = new TransactionCategory();
                tc.Category = "Restaurant";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Pet Care")) {
                var tc = new TransactionCategory();
                tc.Category = "Pet Care";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Entertainment")) {
                var tc = new TransactionCategory();
                tc.Category = "Entertainment";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Gift")) {
                var tc = new TransactionCategory();
                tc.Category = "Gift";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Charity")) {
                var tc = new TransactionCategory();
                tc.Category = "Charity";
                context.TransactionCategories.Add(tc);
            }
            if (!context.TransactionCategories.Any(p => p.Category == "Hobby")) {
                var tc = new TransactionCategory();
                tc.Category = "Hobby";
                context.TransactionCategories.Add(tc);
            }

            // BUDGETPLAN CATEGORIES - !!!! DO *NOT* *NOT* *NOT* CHANGE THE ORDER OF THESE !!!!
            if (!context.BudgetPlanCategories.Any(p => p.Category == "Housing")) {
                var bpc = new BudgetPlanCategory();
                bpc.Category = "Housing";
                context.BudgetPlanCategories.Add(bpc);
            }
            if (!context.BudgetPlanCategories.Any(p => p.Category == "Transportation")) {
                var bpc = new BudgetPlanCategory();
                bpc.Category = "Transportation";
                context.BudgetPlanCategories.Add(bpc);
            }
            if (!context.BudgetPlanCategories.Any(p => p.Category == "Food")) {
                var bpc = new BudgetPlanCategory();
                bpc.Category = "Food";
                context.BudgetPlanCategories.Add(bpc);
            }
            if (!context.BudgetPlanCategories.Any(p => p.Category == "Entertainment")) {
                var bpc = new BudgetPlanCategory();
                bpc.Category = "Entertainment";
                context.BudgetPlanCategories.Add(bpc);
            }
            if (!context.BudgetPlanCategories.Any(p => p.Category == "Savings")) {
                var bpc = new BudgetPlanCategory();
                bpc.Category = "Savings";
                context.BudgetPlanCategories.Add(bpc);
            }
            if (!context.BudgetPlanCategories.Any(p => p.Category == "Debt")) {
                var bpc = new BudgetPlanCategory();
                bpc.Category = "Debt";
                context.BudgetPlanCategories.Add(bpc);
            }
            if (!context.BudgetPlanCategories.Any(p => p.Category == "Healthcare")) {
                var bpc = new BudgetPlanCategory();
                bpc.Category = "Healthcare";
                context.BudgetPlanCategories.Add(bpc);
            }
            if (!context.BudgetPlanCategories.Any(p => p.Category == "Education")) {
                var bpc = new BudgetPlanCategory();
                bpc.Category = "Education";
                context.BudgetPlanCategories.Add(bpc);
            }
            if (!context.BudgetPlanCategories.Any(p => p.Category == "Personal")) {
                var bpc = new BudgetPlanCategory();
                bpc.Category = "Personal";
                context.BudgetPlanCategories.Add(bpc);
            }
            if (!context.BudgetPlanCategories.Any(p => p.Category == "Childcare")) {
                var bpc = new BudgetPlanCategory();
                bpc.Category = "Childcare";
                context.BudgetPlanCategories.Add(bpc);
            }
            if (!context.BudgetPlanCategories.Any(p => p.Category == "Miscellaneous")) {
                var bpc = new BudgetPlanCategory();
                bpc.Category = "Miscellaneous";
                context.BudgetPlanCategories.Add(bpc);
            }

            // FREQUENCY CATEGORIES
            if (!context.FrequencyCategories.Any(p => p.Frequency == "Monthly")) {
                var fc = new FrequencyCategory();
                fc.Frequency = "Monthly";
                context.FrequencyCategories.Add(fc);
            }
            if (!context.FrequencyCategories.Any(p => p.Frequency == "Annually")) {
                var fc = new FrequencyCategory();
                fc.Frequency = "Annually";
                context.FrequencyCategories.Add(fc);
            }
            if (!context.FrequencyCategories.Any(p => p.Frequency == "Weekly")) {
                var fc = new FrequencyCategory();
                fc.Frequency = "Weekly";
                context.FrequencyCategories.Add(fc);
            }
            if (!context.FrequencyCategories.Any(p => p.Frequency == "Biweekly ")) {
                var fc = new FrequencyCategory();
                fc.Frequency = "Biweekly";
                context.FrequencyCategories.Add(fc);
            }
            if (!context.FrequencyCategories.Any(p => p.Frequency == "Bimonthly")) {
                var fc = new FrequencyCategory();
                fc.Frequency = "Bimonthly";
                context.FrequencyCategories.Add(fc);
            }


        }
    }
}
