using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HRACCPortal.Helpers
{
   
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedRoles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            this.allowedRoles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["UserRole"] == null)
                return false;

            var role = httpContext.Session["UserRole"].ToString();
            return allowedRoles.Contains(role);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "Account", action = "UnauthorizedAccess" }));
        }
    }

}