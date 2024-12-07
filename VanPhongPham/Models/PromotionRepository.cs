using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class PromotionRepository
    {
        private readonly DB_VanPhongPhamDataContext _context;

        public PromotionRepository()
        {
            _context = new DB_VanPhongPhamDataContext();
        }
        public List<PromotionViewModel> GetPromotions()
        {
            var promotions = _context.promotions.Where(p => p.status == true).Select(p => new PromotionViewModel
            {
                PromotionId = p.promotion_id,
                PromotionName = p.promotion_name,
                DiscountPercent = p.discount_percent,
                StartDate = p.start_date,
                EndDate = p.end_date,
                Description = p.description,
                Status = p.status ?? false,
                ProductIds = _context.product_promotions
                             .Where(pp => pp.promotion_id == p.promotion_id)
                             .Select(pp => pp.product_id)
                             .ToList()
            }).ToList();

            return promotions;
        }

        public PromotionViewModel GetPromotionById(string promotionId)
        {
            var promotion = _context.promotions.Where(p => p.promotion_id == promotionId && p.status == true).Select(p => new PromotionViewModel
            {
                PromotionId = p.promotion_id,
                PromotionName = p.promotion_name,
                DiscountPercent = p.discount_percent,
                StartDate = p.start_date,
                EndDate = p.end_date,
                Description = p.description,    
                Status = p.status ?? false,
                ProductIds = _context.product_promotions
                             .Where(pp => pp.promotion_id == p.promotion_id)
                             .Select(pp => pp.product_id)
                             .ToList()
            }).FirstOrDefault();

            return promotion;
        }

        public PromotionViewModel GetExistingPromotions(string promotionName, DateTime startDate, DateTime endDate)
        {
            var promotion = _context.promotions.Where(p => p.promotion_name == promotionName && p.status == true || p.start_date >= startDate && p.end_date <= endDate && p.status == true)
                            .Select(p => new PromotionViewModel
                            {
                                PromotionId = p.promotion_id,
                                PromotionName = p.promotion_name,
                                DiscountPercent = p.discount_percent,
                                StartDate = p.start_date,
                                EndDate = p.end_date,
                                Description = p.description,
                                Status = p.status ?? false,
                                ProductIds = _context.product_promotions
                                                .Where(pp => pp.promotion_id == p.promotion_id)
                                                .Select(pp => pp.product_id)
                                                .ToList()
                            }).FirstOrDefault();
            return promotion;
        }
        public bool AddPromotion(PromotionViewModel promotion)
        {
            try
            {
                var existingPromotion = _context.promotions.FirstOrDefault(p => p.promotion_id == promotion.PromotionId);
                if (existingPromotion != null)
                {
                    return false;
                }

                var newPromotion = new promotion
                {
                    promotion_id = promotion.PromotionId,
                    promotion_name = promotion.PromotionName,
                    discount_percent = promotion.DiscountPercent,
                    start_date = promotion.StartDate,
                    end_date = promotion.EndDate,
                    description = promotion.Description,
                    status = promotion.Status
                };

                _context.promotions.InsertOnSubmit(newPromotion);

                foreach (var productId in promotion.ProductIds)
                {
                    var productPromotion = new product_promotion
                    {
                        promotion_id = promotion.PromotionId,
                        product_id = productId
                    };
                    _context.product_promotions.InsertOnSubmit(productPromotion);
                }

                _context.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdatePromotion(PromotionViewModel promotion)
        {
            try
            {
                var existingPromotion = _context.promotions.FirstOrDefault(p => p.promotion_id == promotion.PromotionId);
                if (existingPromotion == null)
                {
                    return false;
                }

                existingPromotion.promotion_name = promotion.PromotionName;
                existingPromotion.discount_percent = promotion.DiscountPercent;
                existingPromotion.start_date = promotion.StartDate;
                existingPromotion.end_date = promotion.EndDate;
                existingPromotion.description = promotion.Description;
                existingPromotion.status = promotion.Status;

                // Xóa danh sách sản phẩm cũ
                var oldProductPromotions = _context.product_promotions
                    .Where(pp => pp.promotion_id == promotion.PromotionId)
                    .ToList();
                _context.product_promotions.DeleteAllOnSubmit(oldProductPromotions);

                // Thêm danh sách sản phẩm mới
                foreach (var productId in promotion.ProductIds)
                {
                    var newProductPromotion = new product_promotion
                    {
                        promotion_id = promotion.PromotionId,
                        product_id = productId
                    };
                    _context.product_promotions.InsertOnSubmit(newProductPromotion);
                }

                _context.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeletePromotion(List<string> ids)
        {
            try
            {
                var promotionToDelete = _context.promotions.Where(p => ids.Contains(p.promotion_id)).ToList();
                promotionToDelete.ForEach(p => p.status = false);

                _context.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool RecoverPromotion(List<string> ids)
        {
            try
            {
                var promotionsToRecover = _context.promotions.Where(s => ids.Contains(s.promotion_id)).ToList();
                promotionsToRecover.ForEach(s => s.status = true);

                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public string GeneratePromotionId()
        {
            promotion us = _context.promotions.ToList().LastOrDefault();
            if (us == null)
            {
                return "PROMO001";
            }
            else
            {
                int num = int.Parse(us.promotion_id.Substring(5)) + 1;
                string promo_id = "PROMO";
                if (num < 10)
                    promo_id = "PROMO00";
                else if (num < 100)
                    promo_id = "PROMO0";
                promo_id += num;
                return promo_id;
            }
        }
        public List<PromotionViewModel> GetDeletedPromotions()
        {
            var promotions = _context.promotions.Where(p => p.status == false).Select(p => new PromotionViewModel
            {
                PromotionId = p.promotion_id,
                PromotionName = p.promotion_name,
                DiscountPercent = p.discount_percent,
                StartDate = p.start_date,
                EndDate = p.end_date,
                Description = p.description,
                Status = p.status ?? false,
                ProductIds = _context.product_promotions
                             .Where(pp => pp.promotion_id == p.promotion_id)
                             .Select(pp => pp.product_id)
                             .ToList()
            }).ToList();
            return promotions;
        }
        public List<PromotionViewModel> SearchPromotion(string searchStr)
        {
            var promotions = _context.promotions.Where(p =>
                p.promotion_name.Contains(searchStr) ||
                (p.description != null && p.description.Contains(searchStr)) ||
                p.start_date.ToString("yyyy-MM-dd").Contains(searchStr) ||
                p.end_date.ToString("yyyy-MM-dd").Contains(searchStr))
            .Select(p => new PromotionViewModel
            {
                PromotionId = p.promotion_id,
                PromotionName = p.promotion_name,
                DiscountPercent = p.discount_percent,
                StartDate = p.start_date,
                EndDate = p.end_date,
                Description = p.description,
                Status = p.status ?? false,
                ProductIds = _context.product_promotions
                             .Where(pp => pp.promotion_id == p.promotion_id)
                             .Select(pp => pp.product_id)
                             .ToList()
            }).ToList();

            return promotions;
        }

        public List<PromotionViewModel> GetActivePromotions()
        {
            var promotions = _context.promotions.Where(p => p.status == true && p.start_date <= DateTime.Now && p.end_date >= DateTime.Now).Select(p => new PromotionViewModel
            {
                PromotionId = p.promotion_id,
                PromotionName = p.promotion_name,
                DiscountPercent = p.discount_percent,
                StartDate = p.start_date,
                EndDate = p.end_date,
                Description = p.description,
                Status = p.status ?? false,
                ProductIds = _context.product_promotions
                             .Where(pp => pp.promotion_id == p.promotion_id)
                             .Select(pp => pp.product_id)
                             .ToList()
            }).ToList();
            return promotions;
        }
        public List<PromotionViewModel> SearchDeletedPromotion(string search_str)
        {
            var promotions = _context.promotions.Where(p => p.status == false && (p.promotion_name.Contains(search_str) ||
                       (p.description != null && p.description.Contains(search_str)) ||
                                  p.start_date.ToString("yyyy-MM-dd").Contains(search_str) ||
                                             p.end_date.ToString("yyyy-MM-dd").Contains(search_str))).Select(p => new PromotionViewModel
                                             {
                PromotionId = p.promotion_id,
                PromotionName = p.promotion_name,
                DiscountPercent = p.discount_percent,
                StartDate = p.start_date,
                EndDate = p.end_date,
                Description = p.description,
                Status = p.status ?? false,
            }).ToList();
            return promotions;
        }

    }
}