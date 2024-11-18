using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
using VanPhongPham.Services;

namespace VanPhongPham.Controllers
{
    public class OrderController : Controller
    {
        DB_VanPhongPhamDataContext db = new DB_VanPhongPhamDataContext();

        // GET: Order
        public ActionResult ChangeOrder_OrderStatus(string order_id, int order_status, string finishdate)
        {
            try
            {
                if (!string.IsNullOrEmpty(order_id))
                {
                    var ord = db.orders.FirstOrDefault(o => o.order_id == order_id);

                    if (ord != null)
                    {
                        if (order_status == 1)
                        {
                            ord.order_status_id = 4;
                            ord.created_at = DateTime.Now;
                        }
                        if (order_status == 2)
                        {
                            ord.order_status_id = 3;
                            // Kiểm tra và chuyển đổi finishDate sang kiểu DateTime
                            if (!string.IsNullOrEmpty(finishdate) && DateTime.TryParse(finishdate, out DateTime parsedDate))
                            {
                                ord.delivery_date = parsedDate.ToLocalTime(); // Chuyển đổi về giờ địa phương nếu cần
                            }
                            else
                            {
                                return Json(new { success = false, message = "Finish date is invalid or empty." });
                            }
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

        public ActionResult RequestCancellation(string order_id, string cancelReason)
        {
            try
            {
                if (!string.IsNullOrEmpty(order_id))
                {
                    var ord = db.orders.FirstOrDefault(o => o.order_id == order_id);

                    if (ord != null)
                    {

                        ord.cancellation_requested = 1;
                        ord.cancellation_reason = cancelReason;
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


        [HttpPost]
        public ActionResult SubmitReview(string user_id, string orderId, string currentStatusId, Dictionary<string, int> ratings, Dictionary<string, string> reviewContents)
        {
            foreach (var productId in ratings.Keys.OrderBy(x => x)) // productId được lấy từ keys của ratings
            {
                if (reviewContents.ContainsKey(productId))
                {
                    var review = new product_review
                    {
                        user_id = user_id, // Lấy ID người dùng đang đăng nhập
                        product_id = productId, // productId là string
                        rating = ratings[productId], // Nhận rating tương ứng
                        review_content = reviewContents[productId], // Nhận nội dung đánh giá tương ứng
                        created_at = DateTime.Now
                    };

                    db.product_reviews.InsertOnSubmit(review);

                    // Cập nhật trạng thái đã đánh giá trong bảng order_details
                    var orderDetail = db.order_details.FirstOrDefault(d => d.order_id == orderId && d.product_id == productId);
                    if (orderDetail != null)
                    {
                        orderDetail.isReviewed = true;
                    }
                }
            }

            db.SubmitChanges();
            return RedirectToAction("Index", "Profile", new
            {
                view = "OrderPartial",
                MaTaiKhoan = user_id,
                order_status_id = int.Parse(currentStatusId) // Trạng thái mặc định hoặc giá trị cần truyền
            });
        }


    }
}