using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HRACCPortal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                 defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            //  defaults: new { controller = "Account", action = "Register", id = UrlParameter.Optional }
            //  defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //defaults: new { controller = "Manage", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Timesheet",
                url: "Timesheet/SaveTimesheet",
                defaults: new { controller = "Timesheet", action = "SaveTimesheet" }
            );


            routes.MapRoute(
            name: "EmployerRoute",
            url: "Employer/{action}/{id}",
            defaults: new { controller = "Employer", action = "Index", id = UrlParameter.Optional }
         );
        }
    }
}
