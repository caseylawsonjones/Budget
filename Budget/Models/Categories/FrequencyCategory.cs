using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models.Categories {
    public class FrequencyCategory {
        public int Id { get; set; }
        public string Frequency { get; set; }
        public int HouseholdId { get; set; }

    }
}