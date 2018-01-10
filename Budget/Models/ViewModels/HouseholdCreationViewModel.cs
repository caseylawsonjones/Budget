using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models.ViewModels {

    public class HouseholdCreationViewModel {

        //CONSTRUCTOR
        public HouseholdCreationViewModel() {
            UserAlreadyInHousehold = false;
            UserIsLastHouseholdMember = false;
        }
        public bool UserAlreadyInHousehold { get; set; }
        public bool UserIsLastHouseholdMember { get; set; }
        public Household Household { get; set; }
    }
}