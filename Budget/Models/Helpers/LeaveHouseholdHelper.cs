using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Data;
using Budget.Models.Helpers;
using System.Web.Mvc;

namespace Budget.Models.Helpers {
    [Authorize]
    public class LeaveHouseholdHelper {
        public void LeaveHousehold(ApplicationUser user) {
            ApplicationDbContext db = new ApplicationDbContext();
            Household household = db.Households.Find(user.HouseholdId.Value);
            user.HouseholdId = null;
            // If last member of Household, close household
            if (household.Members.Count() <= 0) {
                household.IsActive = false;
                // Clean up all pending Invitations so nobody can then be added to the closed Household
                if (household.Invitations.Where(i => i.HasAccepted == false && i.HasDeclined == false).Count() > 0) {
                    foreach (var inv in household.Invitations.Where(i => i.HasAccepted == false && i.HasDeclined == false)) {
                        inv.HasDeclined = true;
                        inv.HasAccepted = true;
                        db.Entry(inv).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                db.Entry(household).State = EntityState.Modified;
            }
            else { 
                // If user was the Owner of the Household, we will reassign OwnerId to the next oldest member.
                if (household.OwnerId == user.Id) {
                    ApplicationUser oldestMember = new ApplicationUser();
                    int i = 0;
                    foreach (var member in household.Members) {
                        i++;
                        // The fist user is assigned to oldestMember, otherwise it is an empty object and we cannnot compare against it.
                        if (i == 1) {
                            oldestMember = member;
                        }
                        else {
                            if (member.DateJoined < oldestMember.DateJoined) {
                                oldestMember = member;
                            }
                        }
                    }
                }
            }
            user.DateJoined = null;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}