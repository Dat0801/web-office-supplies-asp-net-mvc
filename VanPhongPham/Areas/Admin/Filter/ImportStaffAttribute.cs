using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VanPhongPham.Areas.Admin.Filter
{
    public class ImportStaffAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session["Role"] == null || (string)session["Role"] != "Nhân viên nhập hàng" && (string)session["Role"] != "Quản lý")
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