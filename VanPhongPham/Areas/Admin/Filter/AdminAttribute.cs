using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace VanPhongPham.Areas.Admin.Filter
{
    public class AdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session["Role"] == null || (string)session["Role"] != "Quản lý")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                    { "controller", "Dashboard" },
                    { "action", "Logout" }
                    });
            }
        }
    }
}