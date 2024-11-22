using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Hangfire;
using VanPhongPham.Services;
using VanPhongPham.Models;

namespace VanPhongPham
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            VanPhongPham.Services.FirebaseService.Initialize();

            GlobalConfiguration.Configuration.UseSqlServerStorage(ConfigurationManager.ConnectionStrings["DB_VanPhongPhamConnectionString1"].ConnectionString);

            // Đăng ký công việc lặp lại với Hangfire
            RecurringJob.AddOrUpdate<OrderUpdater>(
                "UpdateDeliveredOrders",  // Tên công việc
                updater => updater.UpdateDeliveredOrdersAsync(),  // Phương thức cần thực thi
                Cron.Daily);  // Lập lại mỗi ngày
        }
    }
}
