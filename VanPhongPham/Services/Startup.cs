using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;
using Hangfire;
using System.Configuration;
using Hangfire.SqlServer;

[assembly: OwinStartup(typeof(VanPhongPham.Services.Startup))]

namespace VanPhongPham.Services
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Lấy chuỗi kết nối từ web.config
            string connectionString = ConfigurationManager.ConnectionStrings["DB_VanPhongPhamConnectionString1"].ConnectionString;

            // Cấu hình Hangfire với SQL Server
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    SchemaName = "Hangfire"  // Tên schema trong cơ sở dữ liệu (mặc định là "Hangfire")
                });

            // Cấu hình Hangfire Dashboard
            app.UseHangfireDashboard();

            //// Cấu hình Hangfire Server
            //app.UseHangfireServer();

            // Sử dụng chỉ một server cho Hangfire
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = 1 // Chỉ 1 worker
            });
        }
    }
}
