﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models.Categories {
    public class AccountCategory {
        public int Id { get; set; }
        public string Category { get; set; }
        public int HouseholdId { get; set; }
    }
}