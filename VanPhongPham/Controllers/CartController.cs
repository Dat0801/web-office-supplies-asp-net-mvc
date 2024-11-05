using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;

namespace VanPhongPham.Controllers
{
    public class CartController : Controller
    {
        DB_VanPhongPhamDataContext db = new DB_VanPhongPhamDataContext();
        // GET: Cart
        public ActionResult Index(int cart_id)
        {
            var cartsection = db.cart_sections.FirstOrDefault(c => c.cart_id == cart_id);
            return View(cartsection);
        }
        public ActionResult GetCartQuantity(int cart_id)
        {
            int quantity = db.cart_details.Where(c => c.cart_id == cart_id).Count();
            return Json(new { quantity = quantity }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CartPartial(int cart_id)
        {
            // Lấy danh sách đơn hàng theo điều kiện
            var cartdetails = db.cart_details
                .Where(o => o.cart_id == cart_id)// Kiểm tra cả trạng thái và tài khoản
                .Select(o => new OrderDetailViewModel // Sử dụng OrderViewModel
                {
                    ProductID = o.product.product_id, // Thêm thuộc tính ProductID nếu cần
                    ProductName = o.product.product_name,
                    Quantity = o.quantity.HasValue ? o.quantity.Value : 0, // Gán giá trị 0 nếu null
                    TotalAmount = o.total_amount.HasValue ? o.total_amount.Value : 0, // Gán giá trị 0 nếu null
                    ImageUrl = o.product.images
                                .Where(img => img.is_primary == true) // Kiểm tra hình ảnh chính
                                .Select(img => img.image_url)
                                .FirstOrDefault(), // Lấy hình ảnh đầu tiên
                    Price = o.product.price.HasValue ? o.product.price.Value : 0, // Gán giá trị 0 nếu null
                    isReviewed = false, // Giả sử có thuộc tính này trong order_detail
                }).ToList();

            return PartialView(cartdetails);
        }
        public ActionResult AddCartSection(string user_id)
        {
            try
            {
                if (!string.IsNullOrEmpty(user_id))
                {
                    cart_section cart = new cart_section
                    {
                        user_id = user_id
                    };
                    db.cart_sections.InsertOnSubmit(cart);
                    db.SubmitChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}