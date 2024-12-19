using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Areas.Admin.Filter;
using VanPhongPham.Models;

namespace VanPhongPham.Controllers
{
    [Guest]
    public class CouponClientController : Controller
    {
        private readonly CouponRepository _couponRepository;
        public CouponClientController()
        {
            _couponRepository = new CouponRepository();
        }
        // GET: Coupon
        public ActionResult GetAllCoupons(string searchStr, string discountRange, string status, string expiryDate, int pageNumber = 1, int pageSize = 6)
        {
            var coupons = _couponRepository.GetCoupons();
            if (coupons == null || !coupons.Any())
            {                
                return View(coupons);
            }            

            // Filter by status (active, inactive, or all)
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                bool isActive = status == "active";
                coupons = coupons.Where(c => c.Status == isActive).ToList();
            }

            // Filter by expiry date (upcoming, expired, or all)
            if (!string.IsNullOrEmpty(expiryDate) && expiryDate != "all")
            {
                if (expiryDate == "upcoming")
                {
                    coupons = coupons.Where(c => c.ExpiresAt >= DateTime.Now.Date).ToList();
                }
                else if (expiryDate == "expired")
                {
                    coupons = coupons.Where(c => c.ExpiresAt < DateTime.Now.Date).ToList();
                }
            }

            // Filter by discount range
            if (!string.IsNullOrEmpty(discountRange))
            {
                var ranges = discountRange.Split(',');
                var filteredCoupons = new List<CouponViewModel>();
                foreach (var range in ranges)
                {
                    var discounts = range.Split('-');
                    if (discounts.Length == 2 &&
                        int.TryParse(discounts[0], out int minDiscount) &&
                        int.TryParse(discounts[1], out int maxDiscount))
                    {
                        filteredCoupons.AddRange(coupons.Where(c =>
                            c.DiscountPercent >= minDiscount && c.DiscountPercent <= maxDiscount));
                    }
                }
                coupons = filteredCoupons.Distinct().ToList();
            }

            // Filter by search string
            if (!string.IsNullOrEmpty(searchStr))
            {
                var lowerQuery = searchStr.ToLower();
                coupons = coupons.Where(c => c.CouponCode.ToLower().Contains(lowerQuery) ||
                                              c.Description.ToLower().Contains(lowerQuery)).ToList();
            }

            // Pagination
            var totalCoupons = coupons.Count();
            coupons = coupons.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            coupons = coupons;

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCoupons / (double)pageSize);
            ViewBag.SearchStr = searchStr;
            ViewBag.StatusFilter = status;
            ViewBag.ExpiryDateFilter = expiryDate;

            return View(coupons);
        }


    }
}