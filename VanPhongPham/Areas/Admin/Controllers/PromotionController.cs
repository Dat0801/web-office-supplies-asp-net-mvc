using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;

namespace VanPhongPham.Areas.Admin.Controllers
{
    public class PromotionController : Controller
    {
        private readonly PromotionRepository promotionRepository;
        private readonly ProductRepository productRepository;
        public PromotionController()
        {
            promotionRepository = new PromotionRepository();
            productRepository = new ProductRepository();

        }
        // GET: Admin/Promotion
        public ActionResult Index(int? page, string promo_id, string search_str, bool? onlyActive)
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

            List<PromotionViewModel> listPromo;
            
            var products = productRepository.GetAllProducts();
            ViewBag.Products = products;

            if (!string.IsNullOrEmpty(promo_id))
            {                
                var promotion = promotionRepository.GetPromotionById(promo_id);
                ViewBag.Promotion = promotion;                
                ViewBag.SelectedProductIds = promotion.ProductIds;
            }
            else
            {
                ViewBag.SelectedProductIds = new List<string>();
            }

            if (!string.IsNullOrEmpty(search_str))
            {
                listPromo = promotionRepository.SearchPromotion(search_str);
                ViewBag.SearchStr = search_str;
            }
            else if (onlyActive.HasValue && onlyActive.Value)
            {
                listPromo = promotionRepository.GetActivePromotions();
                ViewBag.OnlyActive = true;
            }
            else
            {
                listPromo = promotionRepository.GetPromotions();
            }

            ViewBag.PromoId = promotionRepository.GeneratePromotionId();

            return View(listPromo.ToPagedList(pageNumber, pageSize));
        }        
        public ActionResult RecoverPromotion(string search_str)
        {
            List<PromotionViewModel> sup;
            if (search_str != null)
            {
                sup = promotionRepository.SearchDeletedPromotion(search_str);
                ViewBag.searchStr = search_str;
            }
            else
                sup = promotionRepository.GetDeletedPromotions();
            return View(sup);
        }
        [HttpGet]
        public ActionResult RecoverSinglePromotion(string promotion_id)
        {
            if (!string.IsNullOrEmpty(promotion_id))
            {
                var result = promotionRepository.RecoverPromotion(new List<string> { promotion_id });
                if (result)
                {
                    TempData["Message"] = "Khôi phục khuyến mãi thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Khôi phục khuyến mãi thất bại!";
                    TempData["MessageType"] = "danger";
                }
            }
            return RedirectToAction("Index", "Promotion", new { area = "Admin" });
        }
        [HttpPost]
        public ActionResult RecoverPromotion(List<string> selectedPromotions)
        {
            if (selectedPromotions != null && selectedPromotions.Any())
            {
                var result = promotionRepository.RecoverPromotion(selectedPromotions);
                if (result)
                {
                    TempData["Message"] = "Khôi phục khuyến mãi thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Khôi phục khuyến mãi thất bại!";
                    TempData["MessageType"] = "danger";
                }
            }
            return RedirectToAction("Index", "Promotion", new { area = "Admin" });
        }
        [HttpPost]
        public ActionResult ManagePromotion(string action, PromotionViewModel promotion, List<string> SelectedProductIds)
        {

            if (action == "add")
            {
                var existPromotion = promotionRepository.GetPromotionByName(promotion.PromotionName);
                if (existPromotion != null)
                {
                    TempData["Message"] = "Tên khuyến mãi đã tồn tại! Vui lòng thêm tên mới hoặc kiểm tra phần khôi phục!";
                    TempData["MessageType"] = "danger";
                }
                else
                {
                    promotion.ProductIds = SelectedProductIds;
                    var result = promotionRepository.AddPromotion(promotion);
                    if (result)
                    {
                        TempData["Message"] = "Thêm khuyến mãi thành công!";
                        TempData["MessageType"] = "success";
                    }
                    else
                    {
                        TempData["Message"] = "Khuyến mãi đã tồn tại!";
                        TempData["MessageType"] = "danger";
                    }
                }
            }
            if (action == "edit")
            {
                promotion.ProductIds = SelectedProductIds;
                var result = promotionRepository.UpdatePromotion(promotion);
                if (result)
                {
                    TempData["Message"] = "Cập nhật khuyến mãi thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Cập nhật khuyến mãi thất bại!";
                    TempData["MessageType"] = "success";
                }
            }
            return RedirectToAction("Index", "Promotion", new { area = "Admin" });
        }
        [HttpGet]
        public ActionResult DeletePromotion(string promotion_id)
        {
            if (!string.IsNullOrEmpty(promotion_id))
            {
                var result = promotionRepository.DeletePromotion(new List<string> { promotion_id });
                if (result)
                {
                    TempData["Message"] = "Xóa khuyến mãi thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Xóa khuyến mãi thất bại!";
                    TempData["MessageType"] = "success";
                }
            }
            return RedirectToAction("Index", "Promotion", new { area = "Admin" });
        }

        [HttpPost]
        public ActionResult DeletePromotion(List<string> selectedPromotions)
        {
            if (selectedPromotions != null && selectedPromotions.Any())
            {
                var result = promotionRepository.DeletePromotion(selectedPromotions);
                if (result)
                {
                    TempData["Message"] = "Xóa khuyến mãi thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Xóa khuyến mãi thất bại!";
                    TempData["MessageType"] = "success";
                }
            }
            return RedirectToAction("Index", "Promotion", new { area = "Admin" });
        }
    }
}