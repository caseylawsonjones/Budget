using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Budget.Models.Helpers {
    [Authorize]
    public class AuthorizeHouseholdRequired : AuthorizeAttribute {

        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
                return false;
            return httpContext.User.Identity.IsInAHousehold();
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext) {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                filterContext.Result = new RedirectToRouteResult
                    (new RouteValueDictionary
                    (new { controller = "Home", Action = "CreateJoinHousehold" }));
            else
                base.HandleUnauthorizedRequest(filterContext);
        }
    }
}