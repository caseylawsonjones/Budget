using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class Invitation {

        public int Id { get; set; }

        [Display(Name = "Invited on")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Created { get; set; }

        [Display(Name = "Accepted?")]
        public bool HasAccepted { get; set; }

        [Display(Name = "Declined?")]
        public bool HasDeclined { get; set; }

        public string InviteeEmail { get; set; }

        [Display(Name = "Invitation Message")]
        public string InvitationMessage { get; set; }

        [Display(Name = "Response Message")]
        public string ResponseMessage { get; set; }

        public string InviterId { get; set; }
        public int HouseholdId { get; set; }
        public int? NotificationId { get; set; }


        [Display(Name = "Invited by")]
        public virtual ApplicationUser Inviter { get; set; }

        [Display(Name = "Invited to")]
        public virtual Household Household { get; set; }

        public virtual Notification Notification { get; set; }
    }
}