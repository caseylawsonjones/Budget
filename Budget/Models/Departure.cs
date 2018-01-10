using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class Departure {

        public int Id { get; set; }

        [Display(Name = "Departed on")]
        public DateTime DepartureDate { get; set; }

        public string DeparterId { get; set; }
        public int? HouseholdId { get; set; }
        public int CommentId { get; set; }


        [Display(Name = "Previous Member")]
        public virtual ApplicationUser Departer { get; set; }
        public virtual Household Household { get; set; }

        [Display(Name = "Comment")]
        public virtual Comment Comment { get; set; }


    }
}