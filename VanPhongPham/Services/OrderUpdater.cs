using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using VanPhongPham.Models;

namespace VanPhongPham.Services
{
    public class OrderUpdater
    {
        private readonly GHNService _ghnService;
        private readonly DB_VanPhongPhamDataContext db;
        public OrderUpdater(GHNService ghnService, DB_VanPhongPhamDataContext context)
        {
            _ghnService = ghnService;
            db = context;
        }

        // Constructor mặc định (nếu cần cho Hangfire)
        public OrderUpdater() : this(new GHNService(), new DB_VanPhongPhamDataContext()) { }

        public async Task UpdateDeliveredOrdersAsync()
        {
            // Lấy danh sách đơn hàng có ordercode != null
            var ordersToCheck = db.orders.Where(o => o.ordercode != null && o.order_status_id == 2).ToList();

            foreach (var order in ordersToCheck)
            {
                // Kiểm tra trạng thái từ API GHN và lấy finish_date nếu có
                var (isDelivered, finishDate) = await _ghnService.IsOrderDeliveredAsync(order.ordercode);

                if (isDelivered)
                {
                    // Kiểm tra nếu finish_date cách ngày hiện tại ít nhất 1 ngày
                    if (finishDate.HasValue && (DateTime.Now - finishDate.Value).TotalDays >= 1)
                    {
                        // Cập nhật order_status_id thành 3
                        order.order_status_id = 3;

                        // Cập nhật delivery_date nếu finish_date có giá trị
                        order.delivery_date = finishDate.Value;
                    }
                }

                try
                {
                    // Lưu thay đổi vào database
                    db.SubmitChanges();
                }
                catch (ChangeConflictException)
                {
                    // Nếu gặp lỗi xung đột, có thể đồng bộ lại dữ liệu
                    db.Refresh(System.Data.Linq.RefreshMode.KeepChanges, order); // Giữ lại các thay đổi
                    db.SubmitChanges(); // Thử lại cập nhật
                }
            }
        }

    }
}