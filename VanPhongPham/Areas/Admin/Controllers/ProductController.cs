using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
namespace VanPhongPham.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductRepository productRepository;
        public ProductController()
        {
            productRepository = new ProductRepository();
        }
        public ActionResult Index()
        {
            List<product> listProduct = productRepository.GetAllProduct();
            return View(listProduct);
        }
        public ActionResult Category(string category_id)
        {
            if (category_id != null)
            {
                category category = productRepository.GetCategory(category_id);
                ViewBag.category = category;
            }
            List<category> listCategory = productRepository.GetAllCategory();
            ViewBag.category_id = productRepository.GenerateCategoryId();
            return View(listCategory);
        }
        [HttpPost]
        public ActionResult AddCategory(string action, category category)
        {
            if (action == "add")
            {
                productRepository.AddCategory(category);
            }
            else
            {
                productRepository.UpdateCategory(category);
            }
            return RedirectToAction("Category", "/Product");
        }
        public ActionResult DeleteCategory(string category_id)
        {
            productRepository.DeleteCategory(category_id);
            return RedirectToAction("Category");
        }
    }
}