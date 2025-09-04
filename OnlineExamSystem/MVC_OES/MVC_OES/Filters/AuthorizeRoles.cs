using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_OES.Filters
{
    public class AuthorizeRoles : ActionFilterAttribute
    {
        private readonly string role;

        public AuthorizeRoles(string r)
        {
            role = r;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sessionRole = filterContext.HttpContext.Session["UserRole"]?.ToString();

            if (sessionRole != role)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "Main" },
                        { "action", "Login" }
                    });
            }


            base.OnActionExecuting(filterContext);
        }
    }
}