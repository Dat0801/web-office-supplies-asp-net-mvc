using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;

namespace VanPhongPham.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly CategoryRepository _categoryRepository;
        public HomeController()
        {
            _productRepository = new ProductRepository();
            _categoryRepository = new CategoryRepository();
        }
        // GET: Home
        public ActionResult Index(string cart_id)
        {
            var product = _productRepository.GetTopSellingProducts();
            ViewBag.CartID = cart_id;
            return View(product);
        }
        public ActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories();
            return PartialView("_Dropdown", categories);
        }        
        
    }
}