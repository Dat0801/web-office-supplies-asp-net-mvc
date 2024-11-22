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
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            VanPhongPham.Services.FirebaseService.Initialize();

            GlobalConfiguration.Configuration.UseSqlServerStorage(ConfigurationManager.ConnectionStrings["DB_VanPhongPhamConnectionString1"].ConnectionString);

            // Cập nhật công việc lặp lại mỗi 2 phút một lần sử dụng RecurringJobOptions
            RecurringJob.AddOrUpdate<OrderUpdater>(
                "UpdateDeliveredOrders",  // Tên công việc
                updater => updater.UpdateDeliveredOrdersAsync(),  // Phương thức cần thực thi
                "*/2 * * * *",  // Mỗi 2 phút một lần
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")  // Múi giờ Việt Nam cho hệ điều hành Windows
                }
            );
        }
    }
}
