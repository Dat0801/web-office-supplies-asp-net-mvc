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
        public ActionResult Index(string userid, int cartid, string msg)
        {
            // Lấy tất cả sản phẩm trong giỏ hàng mà người dùng đã chọn và có số lượng đủ
            var cart_items1 = db.cart_details
                                .Where(a => a.cart_id == cartid && a.isSelected == 1 && a.product.stock_quantity >= a.quantity)
                                .ToList();

            if (cart_items1.Count != 0)
            {
                // Kiểm tra xem có sản phẩm nào bị thiếu số lượng trong kho không
                foreach (var item1 in cart_items1)
                {
                    if (item1.quantity > item1.product.stock_quantity)
                    {
                        ViewBag.Check = 1;
                    }
                }
            }
            else
            {
                ViewBag.Check = 1;
            }

            var address = db.addresses.FirstOrDefault(u => u.user_id == userid);
            ViewBag.UserID = userid;
            ViewBag.CartID = cartid;
            if (!string.IsNullOrWhiteSpace(msg))
            {
                ViewBag.msg = msg;
            }
            return View(address);
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
            var cartdetails = db.cart_details
                .Where(o => o.cart_id == cart_id && o.isSelected == 1)
                .Select(o => new OrderDetailViewModel
                {
                    ProductID = o.product.product_id,
                    ProductName = o.product.product_name,
                    ProductWeight = o.product.product_attribute_values
                                    .Join(
                                        db.attribute_values, // Bảng thứ hai để kết hợp
                                        pav => pav.attribute_value_id, // Khóa từ bảng `product_attribute_values`
                                        av => av.attribute_value_id,   // Khóa từ bảng `attribute_values`
                                        (pav, av) => new { pav, av }   // Kết hợp cả hai bảng
                                    )
                                    .Where(x => x.av.attribute_id == "ATT004") // Lọc `attribute_id` là "ATT004"
                                    .Select(x => x.av.value) // Lấy `value` từ `attribute_values`
                                    .FirstOrDefault(), // Lấy giá trị đầu tiên hoặc `null` nếu không có
                    Quantity = o.quantity.HasValue ? o.quantity.Value : 0,
                    QuantityProduct = o.product.stock_quantity.HasValue ? o.product.stock_quantity.Value : 0,
                    TotalAmount = o.total_amount.HasValue ? o.total_amount.Value : 0,
                    ImageUrl = o.product.images
                                .Where(img => img.is_primary == true)
                                .Select(img => img.image_url)
                                .FirstOrDefault(),
                    Price = o.product.price.HasValue ? o.product.price.Value : 0,
                    Promotion_Price = o.product.promotion_price.HasValue ? o.product.promotion_price.Value : 0,
                    isReviewed = false,
                    Product_status = o.product.status.HasValue ? o.product.status.Value : false,
                    isSelected = o.isSelected.HasValue ? o.isSelected.Value : 0,
                }).ToList();

            // Tính tổng trọng lượng cho tất cả các sản phẩm đã chọn trong giỏ hàng
            double totalWeight = 0;
            foreach (var detail in cartdetails)
            {
                if (!string.IsNullOrEmpty(detail.ProductWeight))
                {
                    // Tìm vị trí khoảng trắng đầu tiên và cắt chuỗi
                    int spaceIndex = detail.ProductWeight.IndexOf(' ');
                    if (spaceIndex != -1)
                    {
                        string weightString = detail.ProductWeight.Substring(0, spaceIndex);
                        if (double.TryParse(weightString, out double productWeight))
                        {
                            totalWeight += detail.Quantity * productWeight;
                        }
                    }
                }
            }

            return PartialView(cartdetails);
        }

        public ActionResult PaymentCheckoutPartial(address adrs)
        {
            var paymentmethod = db.payment_methods.ToList();
            ViewBag.adrs = adrs;
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
        public ActionResult InitVNPay(string user_id, int cart_id, string info_adrs, string ordernote, float shipping_fee, float discount_amount, string coupon_applied)
        {
            // Lấy tất cả sản phẩm trong giỏ hàng mà người dùng đã chọn và có số lượng đủ
            var cart_items1 = db.cart_details
                                .Where(a => a.cart_id == cart_id && a.isSelected == 1 && a.product.stock_quantity >= a.quantity)
                                .ToList();

            if (cart_items1.Count != 0)
            {
                // Kiểm tra xem có sản phẩm nào bị thiếu số lượng trong kho không
                foreach (var item1 in cart_items1)
                {
                    if (item1.quantity > item1.product.stock_quantity)
                    {
                        return Json(new { success = false, message = "Dữ liệu không lưu thành công. Số lượng sản phẩm không đủ trong kho." });
                    }
                }
            }
            else
            {
                return Json(new { success = false, message = "Dữ liệu không lưu thành công. Số lượng sản phẩm không đủ trong kho." });
            }

            if (string.IsNullOrWhiteSpace(ordernote))
            {
                ordernote = "";
            }
            
            var current_orderid = GenerateOrderId();
            var cartDetails = db.cart_details.Where(a => a.cart_id == cart_id && a.isSelected == 1).ToList();
            double totalAmount = (double)cartDetails.Sum(c => c.total_amount);

            var dbuser = db.users.FirstOrDefault(u => u.user_id == user_id);
            var addressDefault = dbuser.addresses.FirstOrDefault(a => a.isDefault == true);
            string addressDefaultStr = addressDefault.full_name + ", " + addressDefault.phone_number + ", " + addressDefault.address_line;
            string address = info_adrs ?? addressDefaultStr;
            DateTime expireDate = DateTime.Now.AddDays(1);
            TempData["user_id"] = user_id;
            TempData["cart_id"] = cart_id;
            TempData["info_adrs"] = address;
            TempData["ordernote"] = ordernote;
            TempData["shipping_fee"] = shipping_fee;
            TempData["discount_amount"] = discount_amount;
            TempData["coupon_applied"] = coupon_applied;
            TempData["orderID"] = current_orderid;

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", "262XSFHX");
            vnpay.AddRequestData("vnp_Amount", ((int)(totalAmount+shipping_fee) * 100).ToString()); // Tổng số tiền thanh toán
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Request.UserHostAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang" + current_orderid); // Thông tin đơn hàng
            vnpay.AddRequestData("vnp_OrderType", "other"); // Loại hình thanh toán
            vnpay.AddRequestData("vnp_ReturnUrl", Url.Action("VNPayReturn", "Checkout", null, Request.Url.Scheme));
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); // Mã giao dịch
            vnpay.AddRequestData("vnp_ExpireDate", expireDate.ToString("yyyyMMddHHmmss"));

            string paymentUrl = vnpay.CreateRequestUrl("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html", "MMZXWISZNUUUNKGOZQPCPASLLTHYGMTB");

            return Json(new { success = true, vnpayUrl = paymentUrl });
        }
        public ActionResult VNPayReturn()
        {
            var vnpay = new VnPayLibrary();
            foreach (string s in Request.QueryString)
            {
                vnpay.AddResponseData(s, Request.QueryString[s]);
            }

            var userId = TempData["user_id"]?.ToString();
            var cartId = Convert.ToInt32(TempData["cart_id"]);
            var infoAdrs = TempData["info_adrs"]?.ToString();
            var ordernote = TempData["ordernote"]?.ToString();
            float shippingFee = float.Parse(TempData["shipping_fee"].ToString());
            float discount_amount = float.Parse(TempData["discount_amount"].ToString());
            string coupon_applied = TempData["coupon_applied"]?.ToString();
            var orderId = TempData["orderID"]?.ToString();

            string responseCode = vnpay.GetResponseData("vnp_ResponseCode");

            if (responseCode == "00")
            {
                SaveOrder(userId, cartId, infoAdrs, ordernote, "PAY002", shippingFee, discount_amount, coupon_applied, orderId);
                return RedirectToAction("Index", "Profile", new { order_status_id = 1, MaTaiKhoan = userId, view = "OrderPartial" });
            }
            else
            {                
                return RedirectToAction("Index", "Checkout", new { userid = userId, cartid = cartId, msg = "Thanh toán thất bại!" });
            }
        }

        public ActionResult SaveOrder(string user_id, int cart_id, string info_adrs, string ordernote, string method_id, float shipping_fee, float discount_amount, string coupon_applied,  string orderID)
        {
            // Lấy tất cả sản phẩm trong giỏ hàng mà người dùng đã chọn và có số lượng đủ
            var cart_items1 = db.cart_details
                                .Where(a => a.cart_id == cart_id && a.isSelected == 1 && a.product.stock_quantity >= a.quantity)
                                .ToList();

            if (cart_items1.Count != 0)
            {
                // Kiểm tra xem có sản phẩm nào bị thiếu số lượng trong kho không
                foreach (var item1 in cart_items1)
                {
                    if (item1.quantity > item1.product.stock_quantity)
                    {
                        return Json(new { success = false, message = "Dữ liệu không lưu thành công. Số lượng sản phẩm không đủ trong kho." });
                    }
                }
            }
            else
            {
                return Json(new { success = false, message = "Dữ liệu không lưu thành công. Số lượng sản phẩm không đủ trong kho." });
            }


            if (ordernote == "")
            {
                ordernote = null;
            }  
            
            var current_orderid = orderID ?? GenerateOrderId();

            var dbuser = db.users.FirstOrDefault(u => u.user_id == user_id);
            int paymentStatusID = 1;
            if (method_id == "PAY002")
            {
                paymentStatusID = 2;
            }
            if (info_adrs != null)
            {
                order ord = new order
                {
                    order_id = current_orderid,
                    customer_id = user_id,
                    info_address = info_adrs,
                    ordernote = ordernote,
                    method_id = method_id,
                    shipping_fee = shipping_fee,
                    discount_amount = discount_amount,
                    coupon_applied = coupon_applied,
                    order_status_id = 1,
                    created_at = DateTime.Now,
                    payment_status_id = paymentStatusID,
                    cancellation_requested = 0                    
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
                    shipping_fee = shipping_fee,
                    discount_amount = discount_amount,
                    coupon_applied = coupon_applied,
                    order_status_id = 1,
                    created_at = DateTime.Now,
                    payment_status_id = paymentStatusID,
                    cancellation_requested = 0                    
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
                        price = item.product.price,
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
                        price = item.product.price,
                        discountPrice = 0,
                        total_amount = item.total_amount
                    };

                    db.order_details.InsertOnSubmit(ord_detail);
                    db.SubmitChanges();
                }    
            }    
           
            return Json(new { success = true, message = "Dữ liệu đã được lưu thành công." });
        }

        [HttpPost]
        public ActionResult CheckCoupon(string coupon_code)
        {
            if (string.IsNullOrEmpty(coupon_code))
            {
                return Json(new { success = false, message = "Mã giảm giá không được để trống." });
            }

            var coupon = db.coupons.FirstOrDefault(c => c.coupon_code == coupon_code);

            if (coupon != null)
            {
                if (coupon.status == false)
                {
                    return Json(new { success = false, message = "Mã giảm giá đã ẩn." });
                }

                if (coupon.expires_at < DateTime.Now)
                {
                    return Json(new { success = false, message = "Mã giảm giá đã hết hạn." });
                }

                if (coupon.quantity <= 0)
                {
                    return Json(new { success = false, message = "Mã giảm giá đã hết số lượng." });
                }



                return Json(new { success = true, couponid = coupon.coupon_id, couponcode = coupon.coupon_code, percent = coupon.coupon_percent });
            }

            return Json(new { success = false, message = "Mã giảm giá không tồn tại." });
        }
    }
}