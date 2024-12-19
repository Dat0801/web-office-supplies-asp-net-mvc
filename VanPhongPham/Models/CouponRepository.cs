using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class CouponRepository
    {
        private readonly DB_VanPhongPhamDataContext _context;

        public CouponRepository()
        {
            _context = new DB_VanPhongPhamDataContext();
        }

        public List<CouponViewModel> GetCoupons()
        {
            var coupons = _context.coupons.Where(c => c.status == true).Select(c => new CouponViewModel
            {
                CouponId = c.coupon_id,
                CouponCode = c.coupon_code,
                ImageUrl = c.coupon_imgurl,
                Title = c.coupon_title,
                Description = c.coupon_description,
                DiscountPercent = c.coupon_percent,
                Quantity = (int)c.quantity,
                CreatedAt = (DateTime)c.created_at,
                ExpiresAt = c.expires_at,
                UpdatedAt = (DateTime)c.updated_at,
                Status = c.status ?? false
            }).ToList();

            return coupons;
        }

        public CouponViewModel GetCouponById(string couponId)
        {
            var coupon = _context.coupons.Where(c => c.coupon_id == couponId && c.status == true).Select(c => new CouponViewModel
            {
                CouponId = c.coupon_id,
                CouponCode = c.coupon_code,
                ImageUrl = c.coupon_imgurl,
                Title = c.coupon_title,
                Description = c.coupon_description,
                DiscountPercent = c.coupon_percent,
                Quantity = (int)c.quantity,
                CreatedAt = (DateTime)c.created_at,
                ExpiresAt = c.expires_at,
                UpdatedAt = (DateTime)c.updated_at,
                Status = c.status ?? false
            }).FirstOrDefault();

            return coupon;
        }

        public bool AddCoupon(CouponViewModel coupon)
        {
            try
            {
                var existingCoupon = _context.coupons.FirstOrDefault(c => c.coupon_code == coupon.CouponCode);
                if (existingCoupon != null)
                {
                    return false;
                }

                var newCoupon = new coupon
                {
                    coupon_id = coupon.CouponId,
                    coupon_code = coupon.CouponCode,
                    coupon_imgurl = coupon.ImageUrl,
                    coupon_title = coupon.Title,
                    coupon_description = coupon.Description,
                    coupon_percent = coupon.DiscountPercent,
                    quantity = coupon.Quantity,
                    created_at = DateTime.Now,
                    expires_at = coupon.ExpiresAt,
                    updated_at = DateTime.Now,
                    status = coupon.Status
                };

                _context.coupons.InsertOnSubmit(newCoupon);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateCoupon(CouponViewModel coupon)
        {
            try
            {
                var existingCoupon = _context.coupons.FirstOrDefault(c => c.coupon_id == coupon.CouponId);
                if (existingCoupon == null)
                {
                    return false;
                }

                existingCoupon.coupon_code = coupon.CouponCode;
                existingCoupon.coupon_imgurl = coupon.ImageUrl;
                existingCoupon.coupon_title = coupon.Title;
                existingCoupon.coupon_description = coupon.Description;
                existingCoupon.coupon_percent = coupon.DiscountPercent;
                existingCoupon.quantity = coupon.Quantity;
                existingCoupon.expires_at = coupon.ExpiresAt;
                existingCoupon.updated_at = DateTime.Now;
                existingCoupon.status = coupon.Status;

                _context.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteCoupon(List<string> ids)
        {
            try
            {
                var couponsToDelete = _context.coupons.Where(c => ids.Contains(c.coupon_id)).ToList();
                couponsToDelete.ForEach(c => c.status = false);

                _context.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RecoverCoupon(List<string> ids)
        {
            try
            {
                var couponsToRecover = _context.coupons.Where(c => ids.Contains(c.coupon_id)).ToList();
                couponsToRecover.ForEach(c => c.status = true);

                _context.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GenerateCouponId()
        {
            var lastCoupon = _context.coupons.OrderByDescending(c => c.coupon_id).FirstOrDefault();
            if (lastCoupon == null)
            {
                return "CPN001";
            }
            else
            {
                int num = int.Parse(lastCoupon.coupon_id.Substring(3)) + 1;
                return $"CPN{num:D3}";
            }
        }

        public List<CouponViewModel> SearchCoupons(string searchStr)
        {
            var coupons = _context.coupons.Where(c =>
                c.coupon_code.Contains(searchStr) ||
                c.coupon_title.Contains(searchStr) ||
                (c.coupon_description != null && c.coupon_description.Contains(searchStr)) ||
                (c.coupon_percent.ToString().Contains(searchStr))
                )
            .Select(c => new CouponViewModel
            {
                CouponId = c.coupon_id,
                CouponCode = c.coupon_code,
                ImageUrl = c.coupon_imgurl,
                Title = c.coupon_title,
                Description = c.coupon_description,
                DiscountPercent = c.coupon_percent,
                Quantity = (int)c.quantity,
                CreatedAt = (DateTime)c.created_at,
                ExpiresAt = c.expires_at,
                UpdatedAt = (DateTime)c.updated_at,
                Status = c.status ?? false
            }).ToList();

            return coupons;
        }
        public List<CouponViewModel> SearchDeletedCoupons(string searchStr)
        {
            var coupons = _context.coupons.Where(c => c.status == false &&
                            (c.coupon_code.Contains(searchStr) ||
                            c.coupon_title.Contains(searchStr) ||
                            (c.coupon_description != null && c.coupon_description.Contains(searchStr)) ||
                            (c.coupon_percent.ToString().Contains(searchStr))
                            ))
            .Select(c => new CouponViewModel
            {
                CouponId = c.coupon_id,
                CouponCode = c.coupon_code,
                ImageUrl = c.coupon_imgurl,
                Title = c.coupon_title,
                Description = c.coupon_description,
                DiscountPercent = c.coupon_percent,
                Quantity = (int)c.quantity,
                CreatedAt = (DateTime)c.created_at,
                ExpiresAt = c.expires_at,
                UpdatedAt = (DateTime)c.updated_at,
                Status = c.status ?? false
            }).ToList();

            return coupons;
        }
        public List<CouponViewModel> GetDeletedCoupons()
        {
            var coupons = _context.coupons.Where(c => c.status == false).Select(c => new CouponViewModel
            {
                CouponId = c.coupon_id,
                CouponCode = c.coupon_code,
                ImageUrl = c.coupon_imgurl,
                Title = c.coupon_title,
                Description = c.coupon_description,
                DiscountPercent = c.coupon_percent,
                Quantity = (int)c.quantity,
                CreatedAt = (DateTime)c.created_at,
                ExpiresAt = c.expires_at,
                UpdatedAt = (DateTime)c.updated_at,
                Status = c.status ?? false
            }).ToList();

            return coupons;
        }
        public List<CouponViewModel> GetExistingCoupons(string couponCode)
        {
            var coupons = _context.coupons.Where(c => c.coupon_code == couponCode).Select(c => new CouponViewModel
            {
                CouponId = c.coupon_id,
                CouponCode = c.coupon_code,
                ImageUrl = c.coupon_imgurl,
                Title = c.coupon_title,
                Description = c.coupon_description,
                DiscountPercent = c.coupon_percent,
                Quantity = (int)c.quantity,
                CreatedAt = (DateTime)c.created_at,
                ExpiresAt = c.expires_at,
                UpdatedAt = (DateTime)c.updated_at,
                Status = c.status ?? false
            }).ToList();

            return coupons;
        }
        public List<CouponViewModel> GetActiveCoupons()
        {
            var coupons = _context.coupons.Where(c => c.status == false && c.expires_at >= DateTime.Now).Select(c => new CouponViewModel
            {
                CouponId = c.coupon_id,
                CouponCode = c.coupon_code,
                ImageUrl = c.coupon_imgurl,
                Title = c.coupon_title,
                Description = c.coupon_description,
                DiscountPercent = c.coupon_percent,
                Quantity = (int)c.quantity,
                CreatedAt = (DateTime)c.created_at,
                ExpiresAt = c.expires_at,
                UpdatedAt = (DateTime)c.updated_at,
                Status = c.status ?? false
            }).ToList();

            return coupons;
        }
    }

}