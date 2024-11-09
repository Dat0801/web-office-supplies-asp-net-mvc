using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;

namespace VanPhongPham.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DB_VanPhongPhamDataContext db = new DB_VanPhongPhamDataContext();
        // GET: Checkout
        public ActionResult Index(string userid, int cartid)
        {
            ViewBag.UserID = userid;
            ViewBag.CartID = cartid;
            return View();
        }

        public ActionResult AddressCheckoutPartial(string userid)
        {
            if (!string.IsNullOrEmpty(userid))
            {
                var adrs = db.addresses.Where(a => a.user_id == userid).ToList();

                if (adrs != null)
                {
                    return PartialView(adrs); // Truyền thông tin user từ database vào view
                }
            }

            return PartialView();
        }
        public ActionResult ProductsCheckoutPartial(int cart_id)
        {
            // Lấy danh sách đơn hàng theo điều kiện
            var cartdetails = db.cart_details
                .Where(o => o.cart_id == cart_id && o.isSelected == 1)// Kiểm tra cả trạng thái và tài khoản
                .Select(o => new OrderDetailViewModel // Sử dụng OrderViewModel
                {
                    ProductID = o.product.product_id, // Thêm thuộc tính ProductID nếu cần
                    ProductName = o.product.product_name,
                    Quantity = o.quantity.HasValue ? o.quantity.Value : 0, // Gán giá trị 0 nếu null
                    QuantityProduct = o.product.stock_quantity.HasValue ? o.product.stock_quantity.Value : 0,
                    TotalAmount = o.total_amount.HasValue ? o.total_amount.Value : 0, // Gán giá trị 0 nếu null
                    ImageUrl = o.product.images
                                .Where(img => img.is_primary == true) // Kiểm tra hình ảnh chính
                                .Select(img => img.image_url)
                                .FirstOrDefault(), // Lấy hình ảnh đầu tiên
                    Price = o.product.price.HasValue ? o.product.price.Value : 0, // Gán giá trị 0 nếu null
                    Promotion_Price = o.product.promotion_price.HasValue ? o.product.promotion_price.Value : 0,
                    isReviewed = false, // Giả sử có thuộc tính này trong order_detail
                    Product_status = o.product.status.HasValue ? o.product.status.Value : false,
                    isSelected = o.isSelected.HasValue ? o.isSelected.Value : 0,
                }).ToList();
            return PartialView(cartdetails);
        }
        public ActionResult PaymentCheckoutPartial()
        {
            var paymentmethod = db.payment_methods.ToList();
            ViewBag.TotalAmount = db.cart_details.Where(a => a.isSelected == 1).Sum(a => a.total_amount);
            return PartialView(paymentmethod);
        }

        public string GenerateOrderId()
        {
            // Lấy bản ghi cuối cùng trong bảng orders, nếu có
            var ord = db.orders.OrderByDescending(o => o.order_id).FirstOrDefault();

            int num = 1; // Gán số mặc định là 1 nếu không có bản ghi nào
            if (ord != null)
            {
                // Kiểm tra nếu order_id có định dạng hợp lệ
                if (ord.order_id.Length > 3 && ord.order_id.StartsWith("ORDER"))
                {
                    num = int.Parse(ord.order_id.Substring(5)) + 1;
                }
            }

            // Tạo order_id mới với định dạng đúng (bao gồm số thứ tự có 3 chữ số)
            string order_id = "ORDER" + num.ToString("D3"); // "D3" đảm bảo có 3 chữ số

            return order_id;
        }

        public ActionResult SaveOrder(string user_id, int cart_id, string info_adrs, string ordernote, string method_id)
        {
            if (ordernote == "")
            {
                ordernote = null;
            }  
            
            var current_orderid = GenerateOrderId();

            var dbuser = db.users.FirstOrDefault(u => u.user_id == user_id);

            if (info_adrs != null)
            {
                order ord = new order
                {
                    order_id = current_orderid,
                    customer_id = user_id,
                    info_address = info_adrs,
                    ordernote = ordernote,
                    method_id = method_id,
                    order_status_id = 1,
                    created_at = DateTime.Now
                };

                db.orders.InsertOnSubmit(ord);
            }
            else
            {
                string address_str = dbuser.addresses.FirstOrDefault(a => a.isDefault == true).full_name + ", " + dbuser.addresses.FirstOrDefault(a => a.isDefault == true).phone_number + ", " + dbuser.addresses.FirstOrDefault(a => a.isDefault == true).address_line;
                order ord = new order
                {
                    order_id = current_orderid,
                    customer_id = user_id,
                    info_address = address_str,
                    ordernote = ordernote,
                    method_id = method_id,
                    order_status_id = 1,
                    created_at = DateTime.Now
                };

                db.orders.InsertOnSubmit(ord);
            }    

            db.SubmitChanges();

            var cart_items = db.cart_details.Where(a => a.cart_id == cart_id && a.isSelected == 1).ToList();
            foreach(var item in cart_items)
            {
                if (item.product.promotion_price != 0)
                {
                    order_detail ord_detail = new order_detail
                    {
                        order_id = current_orderid,
                        product_id = item.product_id,
                        quantity = item.quantity,
                        discountPrice = item.product.promotion_price,
                        total_amount = item.total_amount,
                    };

                    db.order_details.InsertOnSubmit(ord_detail);
                    db.SubmitChanges();
                }
                else
                {
                    order_detail ord_detail = new order_detail
                    {
                        order_id = current_orderid,
                        product_id = item.product_id,
                        quantity = item.quantity,
                        discountPrice = 0,
                        total_amount = item.total_amount
                    };

                    db.order_details.InsertOnSubmit(ord_detail);
                    db.SubmitChanges();
                }    
            }    
           
            return Json(new { success = true, message = "Dữ liệu đã được lưu thành công." });
        }
    }
}