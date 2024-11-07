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
            ViewBag.CartID = cart_id;
            // Lấy danh sách đơn hàng theo điều kiện
            var cartdetails = db.cart_details
                .Where(o => o.cart_id == cart_id)// Kiểm tra cả trạng thái và tài khoản
                .Select(o => new OrderDetailViewModel // Sử dụng OrderViewModel
                {
                    ProductID = o.product.product_id, // Thêm thuộc tính ProductID nếu cần
                    ProductName = o.product.product_name,
                    Quantity = o.quantity.HasValue ? o.quantity.Value : 0, // Gán giá trị 0 nếu null
                    QuantityProduct = o.product.stock_quantity,
                    TotalAmount = o.total_amount.HasValue ? o.total_amount.Value : 0, // Gán giá trị 0 nếu null
                    ImageUrl = o.product.images
                                .Where(img => img.is_primary == true) // Kiểm tra hình ảnh chính
                                .Select(img => img.image_url)
                                .FirstOrDefault(), // Lấy hình ảnh đầu tiên
                    Price = o.product.price.HasValue ? o.product.price.Value : 0, // Gán giá trị 0 nếu null
                    isReviewed = false, // Giả sử có thuộc tính này trong order_detail
                    isSelected = o.isSelected.HasValue ? o.isSelected.Value : 0,
                }).ToList();
            return View(cartdetails);
        }
        public ActionResult GetCartQuantity(int cart_id)
        {
            int quantity = db.cart_details.Where(c => c.cart_id == cart_id).Count();
            return Json(new { quantity = quantity }, JsonRequestBehavior.AllowGet);
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
        public ActionResult CheckStock(string productId, int quantity)
        {
            var product = db.products.FirstOrDefault(p => p.product_id == productId);
            bool isInStock = product.stock_quantity >= quantity;

            return Json(new { isInStock, availableQuantity = product.stock_quantity, productName = product.product_name });
        }
        public ActionResult UpdateQuantity(int cart_id, string productId, int quantity)
        {
            var cart_items = db.cart_details.FirstOrDefault(d => d.cart_id == cart_id && d.product_id == productId);
            if (cart_items != null)
            {
                cart_items.quantity = quantity;
                cart_items.total_amount = cart_items.product.price * quantity;
            }

            db.SubmitChanges();
            return Json(new { success = true, message = "Dữ liệu đã được lưu thành công." });
        }
        public ActionResult RemoveFromCart(int cart_id, string productId)
        {
            var cart_items = db.cart_details.FirstOrDefault(d => d.cart_id == cart_id && d.product_id == productId);
            if (cart_items != null)
            {
                db.cart_details.DeleteOnSubmit(cart_items);
            }

            db.SubmitChanges();
            return Json(new { success = true, message = "Dữ liệu đã được xóa thành công." });
        }
        public ActionResult UpdateIsSelected(int cart_id, string productId, bool isSelected)
        {
            var cart_items = db.cart_details.FirstOrDefault(d => d.cart_id == cart_id && d.product_id == productId);
            if (cart_items != null)
            {
                if (isSelected)
                {
                    cart_items.isSelected = 1;
                }
                else
                {
                    cart_items.isSelected = 0;
                }    
            }

            db.SubmitChanges();
            return Json(new { success = true, message = "Dữ liệu đã được cập nhật thành công." });
        }
        public ActionResult DeleteSelectedItems(int cart_id, List<string> productIds)
        {
            try
            {
                foreach (var productId in productIds)
                {
                    var cart_item = db.cart_details.FirstOrDefault(d => d.cart_id == cart_id && d.product_id == productId);

                    if (cart_item != null)
                    {
                        db.cart_details.DeleteOnSubmit(cart_item);
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