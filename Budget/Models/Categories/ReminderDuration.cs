using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models.Categories {
    public class ReminderDuration {
        public int Id { get; set; }
        public string Duration { get; set; }
        public int HouseholdId { get; set; }

    }
}