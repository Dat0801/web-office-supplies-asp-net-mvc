using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VanPhongPham.Areas.Admin.Filter
{
    public class SalesStaffAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            var role = (string)session["Role"];
            if (role == null || role != "Nhân viên bán hàng" && role != "Quản lý")
            {
                filterContext.Result = new RedirectToRouteResult(
                                       new System.Web.Routing.RouteValueDictionary
                                       {
                        { "controller", "Dashboard" },
                        { "action", "Logout" }
                    });
            }
        }
    }
}