using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class Household {
        public Household() {
            Members = new HashSet<ApplicationUser>();
            Accounts = new HashSet<Account>();
            BudgetPlans = new HashSet<BudgetPlan>();
            Transactions = new HashSet<Transaction>();
            Incomes = new HashSet<Income>();
            Expenses = new HashSet<Expense>();
            Invitations = new HashSet<Invitation>();
            Departures = new HashSet<Departure>();
        }

        public int Id { get; set; }

        [Display(Name = "Household Name")]
        public string Name { get; set; }

        [Display(Name = "Household Description")]
        public string Description { get; set; }

        [Display(Name = "Created on")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Created { get; set; }

        [Display(Name = "Currently Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Deactivated on")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? Closed { get; set; }

        [Display(Name = "Household Logo")]
        public string LogoUrl { get; set; }

        public string OwnerId { get; set; }

        //public virtual ApplicationUser Owner { get; set; }
        //No virtual relationship is created for AuthorId because that would define
        //a second type of relationship of one to many when the statement below
        //already defines a many to many relationship


        //public virtual ICollection<ApplicationUser> PreviousMembers { get; set; } //May not be able to have two 1-many relationships and a 1-1 relationship
        public virtual ICollection<ApplicationUser> Members { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<BudgetPlan> BudgetPlans { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Income> Incomes { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
        public virtual ICollection<Invitation> Invitations { get; set; }
        public virtual ICollection<Departure> Departures { get; set; }
    }
}