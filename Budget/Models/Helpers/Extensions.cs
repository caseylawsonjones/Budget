using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Budget.Models.Helpers {
    public static class Extensions {

        public static int? GetHouseholdId(this IIdentity user) {
            var claimsIdentity = (ClaimsIdentity)user;
            var HouseholdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            if (HouseholdClaim != null)
                return Int32.Parse(HouseholdClaim.Value);
            else
                return null;
        }

         public static bool IsInAHousehold(this IIdentity user) {
            var claimsUser = (ClaimsIdentity)user;
            //var householdId = GetHouseholdId(claimsUser);
            var householdId = claimsUser.Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            return (householdId != null && !string.IsNullOrWhiteSpace(householdId.Value));
        }

        public static string GetFullName(this IPrincipal user) {
            var fullNameClaim = ((ClaimsIdentity)user.Identity).FindFirst("Name");

            if (fullNameClaim != null) {
                return fullNameClaim.Value;
            }
            else {
                return "";
            }
        }

    }
}