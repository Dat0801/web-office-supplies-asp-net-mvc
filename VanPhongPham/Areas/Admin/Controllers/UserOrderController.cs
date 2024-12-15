using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
using PagedList;
using VanPhongPham.Areas.Admin.Filter;

namespace VanPhongPham.Areas.Admin.Controllers
{
    [SalesStaff]
    public class UserOrderController : Controller
    {
        private readonly DB_VanPhongPhamDataContext db = new DB_VanPhongPhamDataContext();
        // GET: Admin/UserOrder
        public ActionResult Index(int? page, string search_str)
        {
            var orders = db.orders.ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            
            if (search_str != null)
            {
                orders = orders.Where(o => o.order_id.Contains(search_str.ToUpper())).ToList();
                ViewBag.search_str = search_str;
            }
            return View(orders.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ConfirmRequest(int? page, string search_str)
        {
            var orders = db.orders.Where(o => o.order_status_id == 1).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (search_str != null)
            {
                orders = orders.Where(o => o.order_id.Contains(search_str.ToUpper())).ToList();
                ViewBag.search_str = search_str;
            }
            return View(orders.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult CancelRequest(int? page, string search_str)
        {
            var orders = db.orders.Where(o => o.cancellation_requested == 1 && o.order_status_id == 2).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (search_str != null)
            {
                orders = orders.Where(o => o.order_id.Contains(search_str.ToUpper())).ToList();
                ViewBag.search_str = search_str;
            }
            return View(orders.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult DeliveringOrders(int? page, string search_str)
        {
            var orders = db.orders.Where(o => o.order_status_id == 2).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (search_str != null)
            {
                orders = orders.Where(o => o.order_id.Contains(search_str.ToUpper())).ToList();
                ViewBag.search_str = search_str;
            }
            return View(orders.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult DeliveredOrders(int? page, string search_str)
        {
            var orders = db.orders.Where(o => o.order_status_id == 3).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (search_str != null)
            {
                orders = orders.Where(o => o.order_id.Contains(search_str.ToUpper())).ToList();
                ViewBag.search_str = search_str;
            }
            return View(orders.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult CanceledOrders(int? page, string search_str)
        {
            var orders = db.orders.Where(o => o.order_status_id == 4).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (search_str != null)
            {
                orders = orders.Where(o => o.order_id.Contains(search_str.ToUpper())).ToList();
                ViewBag.search_str = search_str;
            }
            return View(orders.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ReturnRequest(int? page, string search_str)
        {
            var orders = db.orders.Where(o => o.cancellation_requested == 1 && o.order_status_id == 3).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (search_str != null)
            {
                orders = orders.Where(o => o.order_id.Contains(search_str.ToUpper())).ToList();
                ViewBag.search_str = search_str;
            }
            return View(orders.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ReturnOrders(int? page, string search_str)
        {
            var orders = db.orders.Where(o => o.order_status_id == 5).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (search_str != null)
            {
                orders = orders.Where(o => o.order_id.Contains(search_str.ToUpper())).ToList();
                ViewBag.search_str = search_str;
            }
            return View(orders.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult UserOrderDetails(string ord_id, string view)
        {
            var cartdetails = db.orders
                .Where(o => o.order_id == ord_id)
                .Select(o => new OrderViewModel
                {
                    OrderId = o.order_id,
                    EmployeeId = o.employee_id,
                    FullNameEmployee = db.users.FirstOrDefault(u => u.user_id == o.employee_id).full_name,
                    CustomerId = o.customer_id,
                    FullNameCustomer = db.users.FirstOrDefault(u => u.user_id == o.customer_id).full_name,
                    InfoAddress = o.info_address,
                    OrderNote = o.ordernote,
                    OrderCode = o.ordercode,
                    MethodId = o.method_id,
                    MethodName = o.payment_method.method_name,
                    DeliveryDate = o.delivery_date,
                    ShippingFee = o.shipping_fee,
                    DiscountAmount = o.discount_amount,
                    CounponApplied = o.coupon_applied,
                    TotalAmount = o.total_amount,
                    OrderStatusID = o.order_status_id,
                    OrderStatusName = o.order_status.order_status_name,
                    CancellationRequested = o.cancellation_requested ?? 0,
                    CancellationReason = o.cancellation_reason,
                    ReturnImages = o.return_images,
                    CreatedAt = o.created_at,
                    OrderDetails = o.order_details.Select(od => new OrderDetailViewModel
                                    {
                                        ProductID = od.product.product_id,
                                        ProductName = od.product.product_name,
                                        ProductWeight = od.product.product_attribute_values
                                                        .Join(
                                                            db.attribute_values, // Bảng thứ hai để kết hợp
                                                            pav => pav.attribute_value_id, // Khóa từ bảng `product_attribute_values`
                                                            av => av.attribute_value_id,   // Khóa từ bảng `attribute_values`
                                                            (pav, av) => new { pav, av }   // Kết hợp cả hai bảng
                                                        )
                                                        .Where(x => x.av.attribute_id == "ATT004") // Lọc `attribute_id` là "ATT004"
                                                        .Select(x => x.av.value) // Lấy `value` từ `attribute_values`
                                                        .FirstOrDefault(), // Lấy giá trị đầu tiên hoặc `null` nếu không có
                                        Quantity = od.quantity.HasValue ? od.quantity.Value : 0,
                                        QuantityProduct = od.product.stock_quantity.HasValue ? od.product.stock_quantity.Value : 0,
                                        TotalAmount = od.total_amount ?? 0,
                                        ImageUrl = od.product.images
                                                    .Where(img => img.is_primary == true)
                                                    .Select(img => img.image_url)
                                                    .FirstOrDefault(),
                                        Price = od.price ?? 0,
                                        Promotion_Price = od.discountPrice.HasValue ? od.discountPrice.Value : 0
                                    }).ToList()
                })
                .FirstOrDefault();

            // Tính tổng trọng lượng cho tất cả các sản phẩm đã chọn trong giỏ hàng
            int totalWeight = 0;
            foreach (var detail in cartdetails.OrderDetails)
            {
                if (!string.IsNullOrEmpty(detail.ProductWeight))
                {
                    // Tìm vị trí khoảng trắng đầu tiên và cắt chuỗi
                    int spaceIndex = detail.ProductWeight.IndexOf(' ');
                    if (spaceIndex != -1)
                    {
                        string weightString = detail.ProductWeight.Substring(0, spaceIndex);
                        if (int.TryParse(weightString, out int productWeight))
                        {
                            totalWeight += detail.Quantity * productWeight;
                        }
                    }
                }
            }

            ViewBag.TotalWeight = totalWeight;
            ViewBag.View = view;
            return View(cartdetails);
        }
        public ActionResult UpdateOrderAfterConfirm(string ord_id, string employeeid, string ordercode)
        {
            try
            {
                var dborder = db.orders.FirstOrDefault(o => o.order_id == ord_id);

                if (dborder != null)
                {
                    dborder.employee_id = employeeid;
                    dborder.ordercode = ordercode;
                    dborder.order_status_id = 2;
                }

                db.SubmitChanges();

                return Json(new { success = true, message = "Dữ liệu đã được lưu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult UpdateRequestCancelOrder(string order_id, string finishdate, int check)
        {
            try
            {
                if (!string.IsNullOrEmpty(order_id))
                {
                    var ord = db.orders.FirstOrDefault(o => o.order_id == order_id);

                    if (ord != null)
                    {
                        if (check == 1)
                        {
                            ord.order_status_id = 4;
                            ord.cancellation_requested = 3;
                            if (!string.IsNullOrEmpty(finishdate) && DateTime.TryParse(finishdate, out DateTime parsedDate))
                            {
                                ord.created_at = parsedDate.ToLocalTime();
                            }

                            if (ord.method_id != "PAY001")
                            {
                                var wallet = db.user_wallets.FirstOrDefault(w => w.user_id == ord.user.user_id);

                                wallet.balance = wallet.balance + ord.total_amount;
                            }
                        }
                        else if (check == 0)
                        {
                            ord.cancellation_requested = 2;
                        }    

                        try
                        {
                            db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error during SubmitChanges: " + ex.Message);
                            return Json(new { success = false, message = ex.Message });
                        }
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult UpdateRequestReturnOrder(string order_id, string order_code, int check)
        {
            try
            {
                if (!string.IsNullOrEmpty(order_id))
                {
                    var ord = db.orders.FirstOrDefault(o => o.order_id == order_id);

                    if (ord != null)
                    {
                        if (check == 1)
                        {
                            ord.ordercode = order_code;
                            ord.order_status_id = 5;
                            ord.cancellation_requested = 3;
                            ord.created_at = DateTime.Now;
                        }
                        else if (check == 0)
                        {
                            ord.cancellation_requested = 2;
                        }

                        try
                        {
                            db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error during SubmitChanges: " + ex.Message);
                            return Json(new { success = false, message = ex.Message });
                        }
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult CheckRechargeWallet(string order_id, string discardrechargeReason, int check)
        {
            try
            {
                if (!string.IsNullOrEmpty(order_id))
                {
                    var ord = db.orders.FirstOrDefault(o => o.order_id == order_id);

                    if (ord != null)
                    {
                        if (check == 1)
                        {
                            ord.cancellation_requested = 6;
                            ord.created_at = DateTime.Now;

                            var wallet = db.user_wallets.FirstOrDefault(w => w.user_id == ord.user.user_id);

                            wallet.balance = wallet.balance + (ord.total_amount - ord.shipping_fee);
                        }
                        else if (check == 0)
                        {
                            ord.cancellation_requested = 5;
                            ord.cancellation_reason = ord.cancellation_reason + "," + discardrechargeReason;
                            ord.created_at = DateTime.Now;
                        }

                        try
                        {
                            db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error during SubmitChanges: " + ex.Message);
                            return Json(new { success = false, message = ex.Message });
                        }
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult CancelOrderWaitingConfirm(string employeeid, string order_id, string cancelReason)
        {
            try
            {
                if (!string.IsNullOrEmpty(order_id))
                {
                    var ord = db.orders.FirstOrDefault(o => o.order_id == order_id);

                    if (ord != null)
                    {
                        ord.employee_id = employeeid;
                        ord.order_status_id = 4;
                        ord.cancellation_requested = 4;
                        ord.cancellation_reason = cancelReason;
                        ord.created_at = DateTime.Now;
                    }

                    if (ord.method_id != "PAY001")
                    {
                        var wallet = db.user_wallets.FirstOrDefault(w => w.user_id == ord.user.user_id);

                        wallet.balance = wallet.balance + ord.total_amount;
                    }    
                }

                db.SubmitChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}