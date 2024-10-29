using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;

namespace VanPhongPham.Controllers
{
    public class ProfileController : Controller
    {
        DB_VanPhongPhamDataContext db = new DB_VanPhongPhamDataContext();
        // GET: Profile
        public ActionResult Index(string view, string MaTaiKhoan, int order_status_id = -1)
        {
            ViewBag.PartialView = string.IsNullOrEmpty(view) ? "ProfilePartial" : view;
            ViewBag.MaTaiKhoan = MaTaiKhoan;
            ViewBag.CurrentStatus = order_status_id;

            if (!string.IsNullOrEmpty(MaTaiKhoan))
            {
                var dbUser = db.users.FirstOrDefault(u => u.user_id == MaTaiKhoan);
                if (dbUser != null)
                {
                    return View(dbUser); // Trả về view với đối tượng người dùng
                }
                else
                {
                    // Trường hợp không tìm thấy tài khoản
                    return View("Error", new { message = "Không tìm thấy tài khoản" });
                }
            }

            return View();
        }

        public ActionResult ProfilePartial(string MaTaiKhoan)
        {
            if (!string.IsNullOrEmpty(MaTaiKhoan))
            {
                var dbUser = db.users.FirstOrDefault(u => u.user_id == MaTaiKhoan);

                if (dbUser != null)
                {
                    return PartialView(dbUser); // Truyền thông tin user từ database vào view
                }
            }

            return PartialView();
        }

        public ActionResult AddressPartial(string MaTaiKhoan) // Thêm tham số MaTaiKhoan
        {
            if (!string.IsNullOrEmpty(MaTaiKhoan))
            {
                var dbUser = db.users.Include("addresses").FirstOrDefault(u => u.user_id == MaTaiKhoan);
                var adrs = db.addresses.Where(a => a.user_id == MaTaiKhoan).ToList();

                if (dbUser != null)
                {
                    return PartialView(adrs); // Truyền thông tin user từ database vào view
                }
            }

            return PartialView();
        }

        public ActionResult ChangePasswordPartial(string MaTaiKhoan) // Thêm tham số MaTaiKhoan
        {
            if (!string.IsNullOrEmpty(MaTaiKhoan))
            {
                var dbUser = db.users.FirstOrDefault(u => u.user_id == MaTaiKhoan);

                if (dbUser != null)
                {
                    return PartialView(dbUser); // Truyền thông tin user từ database vào view
                }
            }

            return PartialView();
        }

        public ActionResult ChangeEmailPartial(string MaTaiKhoan) // Thêm tham số MaTaiKhoan
        {
            if (!string.IsNullOrEmpty(MaTaiKhoan))
            {
                var dbUser = db.users.FirstOrDefault(u => u.user_id == MaTaiKhoan);

                if (dbUser != null)
                {
                    return PartialView(dbUser); // Truyền thông tin user từ database vào view
                }
            }

            return PartialView();
        }

        public ActionResult OrderPartial(int order_status_id, string MaTaiKhoan) // Thêm tham số MaTaiKhoan
        {
            // Lấy danh sách đơn hàng theo điều kiện
            var orders = db.orders
                .Where(o => (order_status_id == -1 || o.order_status_id == order_status_id)
                             && o.customer_id == MaTaiKhoan) // Kiểm tra cả trạng thái và tài khoản
                .Select(o => new OrderViewModel // Sử dụng OrderViewModel
                {
                    OrderId = o.order_id,
                    EmployeeId = o.employee_id,
                    CustomerId = o.customer_id,
                    AddressId = o.address_id,
                    MethodId = o.method_id,
                    DeliveryDate = o.delivery_date,
                    TotalAmount = o.total_amount,
                    OrderStatusName = o.order_status.order_status_name,
                    CreatedAt = o.created_at,
                    OrderDetails = o.order_details.Select(od => new OrderDetailViewModel // Sử dụng OrderDetailViewModel
                    {
                        ProductID = od.product.product_id, // Thêm thuộc tính ProductID nếu cần
                        ProductName = od.product.product_name,
                        Quantity = od.quantity.HasValue ? od.quantity.Value : 0, // Gán giá trị 0 nếu null
                        TotalAmount = od.total_amount.HasValue ? od.total_amount.Value : 0, // Gán giá trị 0 nếu null
                        ImageUrl = od.product.images
                                    .Where(img => img.is_primary == true) // Kiểm tra hình ảnh chính
                                    .Select(img => img.image_url)
                                    .FirstOrDefault(), // Lấy hình ảnh đầu tiên
                        Price = od.product.price.HasValue ? od.product.price.Value : 0, // Gán giá trị 0 nếu null
                        isReviewed = od.isReviewed ?? false, // Giả sử có thuộc tính này trong order_detail
                    }).ToList()
                }).ToList();

            ViewBag.OrderStatus = db.order_status.ToList();
            ViewBag.CurrentStatus = order_status_id; // Thêm dòng này để lưu trạng thái hiện tại

            return PartialView(orders);
        }

        [HttpPost]
        public ActionResult UpdateProfile(user updateUser)
        {
            try
            {
                var currentUser = db.users.FirstOrDefault(u => u.user_id == updateUser.user_id);
                if (currentUser != null)
                {
                    currentUser.full_name = updateUser.full_name;
                    currentUser.gender = updateUser.gender;
                    currentUser.dob = updateUser.dob;
                    currentUser.avt_url = updateUser.avt_url;

                    db.SubmitChanges();
                }
                return RedirectToAction("Index", new { view = "ProfilePartial", MaTaiKhoan = updateUser.user_id });
            }
            catch (Exception ex)
            {
                return View("Error", new { message = ex.Message });
            }
        }
    }
}