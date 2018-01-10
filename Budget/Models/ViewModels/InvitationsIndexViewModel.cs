using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models.ViewModels {
    public class InvitationsIndexViewModel {
        public List<Invitation> Invitations = new List<Invitation>();
        public bool UserIsInHousehold = false;

    }
}