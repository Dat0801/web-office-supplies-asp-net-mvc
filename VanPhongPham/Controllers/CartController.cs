using System;
using System.Collections.Generic;
using System.Data.Linq;
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

            var cart_items = db.cart_details.Where(d => d.cart_id == cart_id).ToList();

            if (cart_items != null)
            {
                foreach (var item in cart_items)
                {
                    if (item.product.promotion_price != 0)
                    {
                        item.total_amount = item.product.promotion_price * item.quantity;
                    }
                    else
                    {
                        item.total_amount = item.product.price * item.quantity;
                    }
                }
            }
            db.Refresh(RefreshMode.KeepChanges, cart_items);
            db.SubmitChanges();

            // Lấy danh sách khuyến mãi
            var currentDate = DateTime.Now;
            var promotions = db.promotions
                .Where(p => p.status == true && p.start_date <= currentDate && p.end_date >= currentDate)
                .ToList();

            // Lấy danh sách đơn hàng theo điều kiện
            var promotionIds = promotions.Select(p => p.promotion_id).ToList();

            var cartdetails = db.cart_details
                            .Where(o => o.cart_id == cart_id)
                            .Select(o => new OrderDetailViewModel
                            {
                                ProductID = o.product.product_id,
                                ProductName = o.product.product_name,
                                Quantity = o.quantity ?? 0,
                                QuantityProduct = o.product.stock_quantity ?? 0,
                                TotalAmount = o.total_amount ?? 0,
                                ImageUrl = o.product.images
                                            .Where(img => img.is_primary == true)
                                            .Select(img => img.image_url)
                                            .FirstOrDefault(),
                                Price = o.product.price ?? 0,
                                Promotion_Price = db.product_promotions
                                    .Where(pp => promotionIds.Contains(pp.promotion_id) && pp.product_id == o.product.product_id)
                                    .Select(pp => (o.product.price) * (1 - (pp.promotion.discount_percent / 100)))
                                    .FirstOrDefault() ?? 0,
                                isReviewed = false,
                                Product_status = o.product.status ?? false,
                                isSelected = o.isSelected ?? 0,
                                Promotions = db.product_promotions
                                    .Where(pp => promotionIds.Contains(pp.promotion_id) && pp.product_id == o.product.product_id)
                                    .Select(pp => new PromotionViewModel
                                    {
                                        PromotionId = pp.promotion_id,
                                        PromotionName = pp.promotion.promotion_name,
                                        Description = pp.promotion.description,
                                        DiscountPercent = pp.promotion.discount_percent,
                                        StartDate = pp.promotion.start_date,
                                        EndDate = pp.promotion.end_date
                                    }).FirstOrDefault()
                            }).ToList();

            return View(cartdetails);
        }

        public ActionResult GetCartQuantity(int cart_id)
        {
            int quantity = db.cart_details.Where(c => c.cart_id == cart_id).Count();
            return Json(new { quantity = quantity }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost] // AJAX sẽ sử dụng POST
        public JsonResult AddToCart(int cart_id, string productId, int quantity)
        {
            try
            {                
                var cart = db.cart_details.FirstOrDefault(c => c.cart_id == cart_id && c.product_id == productId);

                if (cart != null)
                {                    
                    cart.quantity += quantity;

                    cart.total_amount = (cart.product.promotion_price != 0)
                                        ? cart.product.promotion_price * cart.quantity
                                        : cart.product.price * cart.quantity;
                }
                else
                {                    
                    var product = db.products.FirstOrDefault(p => p.product_id == productId);
                    if (product != null)
                    {
                        cart_detail cd = new cart_detail
                        {
                            cart_id = cart_id,
                            product_id = productId,
                            quantity = quantity,
                            total_amount = (product.promotion_price != 0)
                                            ? product.promotion_price * quantity
                                            : product.price * quantity
                        };
                        db.cart_details.InsertOnSubmit(cd);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                    }
                }

                db.SubmitChanges();                
                return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng!" });
            }
            catch (Exception ex)
            {                
                return Json(new { success = false, message = "Đã xảy ra lỗi khi thêm sản phẩm vào giỏ hàng." });
            }
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
                if (cart_items.product.promotion_price != 0)
                {
                    cart_items.quantity = quantity;
                    cart_items.total_amount = cart_items.product.promotion_price * quantity;
                }
                else
                {
                    cart_items.quantity = quantity;
                    cart_items.total_amount = cart_items.product.price * quantity;
                }    
            }

            db.SubmitChanges();
            return Json(new { success = true, message = "Dữ liệu đã được lưu thành công." });
        }

        public ActionResult UpdateCartDetailsPromotionPrice(int cart_id)
        {
            var cart_items = db.cart_details.Where(d => d.cart_id == cart_id).ToList();

            if (cart_items != null)
            {
                foreach (var item in cart_items)
                {
                    if (item.product.promotion_price != 0)
                    {
                        item.total_amount = item.product.promotion_price * item.quantity;
                    }
                    else
                    {
                        item.total_amount = item.product.price * item.quantity;
                    }    
                }
            }    

            db.SubmitChanges();
            return Json(new { success = true, message = "Dữ liệu đã được cập nhật thành công." });
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
                if (cart_items.product.promotion_price != 0)
                {
                    cart_items.total_amount = cart_items.product.promotion_price * cart_items.quantity;
                }
                else
                {
                    cart_items.total_amount = cart_items.product.price * cart_items.quantity;
                }

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