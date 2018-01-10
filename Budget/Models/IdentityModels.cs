using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Budget.Models.Categories;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Budget.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() {
            //Notifications = new HashSet<Notification>();
            //Transactions = new HashSet<Transaction>();
            //Comments = new HashSet<Comment>();
            Invitations = new HashSet<Invitation>();
        }

        //PROPERTIES
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Name")]
        public string FullName {
            get {
                return FirstName + " " + LastName;
            }
        }
        public decimal Amount {  // Amount spent, not received in income
            get {
                DateTime tempTime = DateTime.UtcNow;
                decimal amount = 0;
                if (Transactions != null)
                {
                    foreach (var tx in Transactions.Where(t => (t.Date.Year == tempTime.Year && t.Date.Month == tempTime.Month) && t.Category.Category != "Voided" && t.Category.Category != "Paycheck" && t.BudgetPlanCategory.Category != "Income"))
                    {
                        if (tx.IsCredit)
                        {
                            amount -= tx.Amount;
                        }
                        else
                        {
                            amount += tx.Amount;
                        }
                    }
                }
                return amount;
            }
        }

        [Display(Name = "Date Joined")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateJoined { get; set; }

        public int? HouseholdId { get; set; }

        public virtual Household Household { get; set; }

        public virtual ICollection<Invitation> Invitations { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
  
        //METHODS
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("HouseholdId", HouseholdId.ToString()));
            userIdentity.AddClaim(new Claim("Name", this.FullName));
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //public DbSet<Departure> Departures { get; set; }
        //public DbSet<BudgetItem> BudgetItems { get; set; }
        //public DbSet<AccountMonthlyHistoryItem> MonthlyHistoryItems { get; set; }
        //public DbSet<Reconciliation> Reconciliations { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<BudgetPlan> BudgetPlans { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Household> Households { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        // Categories
        //public DbSet<FrequencyCategory> FrequencyCategories { get; set; }
        //public DbSet<ReminderDuration> ReminderDurations { get; set; }
        public DbSet<AccountCategory> AccountCategories { get; set; }
        public DbSet<BudgetPlanCategory> BudgetPlanCategories { get; set; }
        public DbSet<TransactionCategory> TransactionCategories { get; set; }
        public DbSet<CommentCategory> CommentCategories { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<IncomeCategory> IncomeCategories { get; set; }
        public DbSet<NotificationCategory> NotificationCategories { get; set; }

        // This method takes care of an error caused by the many to many relationshihp betweeen BudgetItems and BudgetPlans
        // Error:
        // Introducing FOREIGN KEY constraint 'FK_dbo.BudgetItemBudgetPlans_dbo.BudgetPlans_BudgetPlan_Id' on table 'BudgetItemBudgetPlans' may cause cycles or multiple cascade paths.
        // Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.
        // Could not create constraint or index.See previous errors.

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}