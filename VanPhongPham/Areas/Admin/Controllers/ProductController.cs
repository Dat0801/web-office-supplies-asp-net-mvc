using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
using PagedList;
namespace VanPhongPham.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductRepository productRepository;
        public ProductController()
        {
            productRepository = new ProductRepository();
        }
        public ActionResult Index(string search_str)
        {
            List<product> listProduct;
            if (search_str != null)
            {
                listProduct = productRepository.SearchProduct(search_str);
                ViewBag.search_str = search_str;
            }
            else
            {
                listProduct = productRepository.GetAllProduct();
            }
            return View(listProduct);
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            ViewBag.product_id = productRepository.GenerateProductId();
            ViewBag.list_category = productRepository.GetAllCategory();
            return View();
        }


        public ActionResult Category(int? page, string category_id, string search_str)
        {
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            List<category> listCategory;
            if (category_id != null)
            {
                category category = productRepository.GetCategory(category_id);
                ViewBag.category = category;
            }
            if (search_str != null)
            {
                listCategory = productRepository.SearchCategory(search_str);
                ViewBag.search_str = search_str;
            }
            else
            {
                listCategory = productRepository.GetAllCategory();
            }
            ViewBag.category_id = productRepository.GenerateCategoryId();
            return View(listCategory.ToPagedList(pageNumber, pageSize));
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
        

        [HttpPost]
        public JsonResult AddCategoryAJAX(string categoryName)
        {
            category category = new category();
            category.category_id = productRepository.GenerateCategoryId();
            category.category_name = categoryName;
            bool result = productRepository.AddCategory(category);
            if (result)
            {
                return Json(new { success = true, newCategoryId = category.category_id });
            } else
            {
                return Json(new { success = false });
            }
        }

        public ActionResult DeleteCategory(string category_id)
        {
            productRepository.DeleteCategory(category_id);
            return RedirectToAction("Category");
        }
    }
}