using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Firebase.Auth.Repository;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Areas.Admin.Filter;
using VanPhongPham.Models;

namespace VanPhongPham.Areas.Admin.Controllers
{    
    public class CouponController : Controller
    {
        private readonly CouponRepository couponRepository;
        private readonly ProductRepository productRepository;
        private readonly Account _cloudinaryAccount = new Account("dgvcrawly", "173992459599594", "F0N2oghE7dRuopEEVqmtRUf9mIQ");
        private readonly Cloudinary _cloudinary;
        public CouponController()
        {
            couponRepository = new CouponRepository();
            productRepository = new ProductRepository();
            _cloudinary = new Cloudinary(_cloudinaryAccount);
        }

        // GET: Admin/Coupon
        [SalesStaff]
        public ActionResult Index(int? page, string coupon_id, string search_str, bool? onlyActive)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var message = TempData["Message"];
            var messageType = TempData["MessageType"];
            if (message != null)
            {
                ViewBag.Message = message;
                ViewBag.MessageType = messageType;
            }

            List<CouponViewModel> listCoupons;            

            if (!string.IsNullOrEmpty(coupon_id))
            {
                var coupon = couponRepository.GetCouponById(coupon_id);
                ViewBag.Coupon = coupon;                
            }
            else
            {
                ViewBag.SelectedProductIds = new List<string>();
            }

            if (!string.IsNullOrEmpty(search_str))
            {
                listCoupons = couponRepository.SearchCoupons(search_str);
                ViewBag.SearchStr = search_str;
            }
            else if (onlyActive.HasValue && onlyActive.Value)
            {
                listCoupons = couponRepository.GetActiveCoupons();
                ViewBag.OnlyActive = true;
            }
            else
            {
                listCoupons = couponRepository.GetCoupons();
            }

            ViewBag.CouponId = couponRepository.GenerateCouponId();

            return View(listCoupons.ToPagedList(pageNumber, pageSize));
        }

        [Admin]
        public ActionResult RecoverCoupon(string search_str)
        {
            List<CouponViewModel> deletedCoupons;
            if (!string.IsNullOrEmpty(search_str))
            {
                deletedCoupons = couponRepository.SearchDeletedCoupons(search_str);
                ViewBag.SearchStr = search_str;
            }
            else
            {
                deletedCoupons = couponRepository.GetDeletedCoupons();
            }
            return View(deletedCoupons);
        }

        [Admin]
        [HttpGet]
        public ActionResult RecoverSingleCoupon(string coupon_id)
        {
            if (!string.IsNullOrEmpty(coupon_id))
            {
                var result = couponRepository.RecoverCoupon(new List<string> { coupon_id });
                if (result)
                {
                    TempData["Message"] = "Khôi phục mã giảm giá thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Khôi phục mã giảm giá thất bại!";
                    TempData["MessageType"] = "danger";
                }
            }
            return RedirectToAction("Index", "Coupon", new { area = "Admin" });
        }

        [Admin]
        [HttpPost]
        public ActionResult RecoverCoupon(List<string> selectedCoupons)
        {
            if (selectedCoupons != null && selectedCoupons.Any())
            {
                var result = couponRepository.RecoverCoupon(selectedCoupons);
                if (result)
                {
                    TempData["Message"] = "Khôi phục mã giảm giá thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Khôi phục mã giảm giá thất bại!";
                    TempData["MessageType"] = "danger";
                }
            }
            return RedirectToAction("Index", "Coupon", new { area = "Admin" });
        }

        [Admin]
        [HttpPost]
        public ActionResult ManageCoupon(string action, CouponViewModel coupon, List<string> SelectedProductIds)
        {
            if(coupon.CouponCode.Length > 10)
            {
                TempData["Message"] = "Mã giảm giá không được quá 10 ký tự!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index", "Coupon", new { area = "Admin" });
            }
            if (coupon.DiscountPercent < 5 || coupon.DiscountPercent > 30)
            {
                TempData["Message"] = "Giảm giá phải nằm trong khoảng từ 5 đến 30!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index", "Coupon", new { area = "Admin" });
            }
            if (coupon.ExpiresAt < DateTime.Now)
            {
                TempData["Message"] = "Ngày hết hạn phải lớn hơn ngày hiện tại!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index", "Coupon", new { area = "Admin" });
            }            
            if(coupon.Quantity <= 0)
            {
                TempData["Message"] = "Số lượng phải lớn hơn 0!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index", "Coupon", new { area = "Admin" });
            }
            if(coupon.Quantity > 100)
            {
                TempData["Message"] = "Số lượng phải nhỏ hơn hoặc bằng 100!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index", "Coupon", new { area = "Admin" });
            }            
            if (coupon.CouponFile != null && coupon.CouponFile.ContentLength > 0)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(coupon.CouponFile.FileName, coupon.CouponFile.InputStream),
                    Folder = "coupon_images"
                };
                var uploadResult = _cloudinary.Upload(uploadParams);

                if (uploadResult.StatusCode == HttpStatusCode.OK)
                {
                    coupon.ImageUrl = uploadResult.SecureUrl.ToString();
                }
            }
            else
            {
                var coupons = couponRepository.GetCouponById(coupon.CouponId);
                coupon.ImageUrl = coupons?.ImageUrl ?? "https://res.cloudinary.com/dgvcrawly/image/upload/v1734583342/coupon_images/pcdrzfvvhpw9rxxuijm6.png";
            }
            if (action == "add")
            {
                var existCoupon = couponRepository.GetExistingCoupons(coupon.CouponCode);
                if (existCoupon.Count != 0)
                {
                    TempData["Message"] = "Mã giảm giá đã tồn tại!";
                    TempData["MessageType"] = "danger";
                }
                else
                {
                    //coupon.ProductIds = SelectedProductIds;
                    var result = couponRepository.AddCoupon(coupon);
                    if (result)
                    {
                        TempData["Message"] = "Thêm mã giảm giá thành công!";
                        TempData["MessageType"] = "success";
                    }
                    else
                    {
                        TempData["Message"] = "Thêm mã giảm giá thất bại!";
                        TempData["MessageType"] = "danger";
                    }
                }
            }

            if (action == "edit")
            {
                //coupon.ProductIds = SelectedProductIds;
                var result = couponRepository.UpdateCoupon(coupon);
                if (result)
                {
                    TempData["Message"] = "Cập nhật mã giảm giá thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Cập nhật mã giảm giá thất bại!";
                    TempData["MessageType"] = "danger";
                }
            }
            return RedirectToAction("Index", "Coupon", new { area = "Admin" });
        }

        [Admin]
        [HttpGet]
        public ActionResult DeleteCoupon(string coupon_id)
        {
            if (!string.IsNullOrEmpty(coupon_id))
            {
                var result = couponRepository.DeleteCoupon(new List<string> { coupon_id });
                if (result)
                {
                    TempData["Message"] = "Xóa mã giảm giá thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Xóa mã giảm giá thất bại!";
                    TempData["MessageType"] = "danger";
                }
            }
            return RedirectToAction("Index", "Coupon", new { area = "Admin" });
        }

        [Admin]
        [HttpPost]
        public ActionResult DeleteCoupon(List<string> selectedCoupons)
        {
            if (selectedCoupons != null && selectedCoupons.Any())
            {
                var result = couponRepository.DeleteCoupon(selectedCoupons);
                if (result)
                {
                    TempData["Message"] = "Xóa mã giảm giá thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Xóa mã giảm giá thất bại!";
                    TempData["MessageType"] = "danger";
                }
            }
            return RedirectToAction("Index", "Coupon", new { area = "Admin" });
        }
    }

}