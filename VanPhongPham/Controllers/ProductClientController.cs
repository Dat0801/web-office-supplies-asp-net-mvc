using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;

namespace VanPhongPham.Controllers
{
    public class ProductClientController : Controller
    {
        // GET: Product
        private readonly ProductRepository _productRepository;
        public ProductClientController()
        {
            _productRepository = new ProductRepository();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Details(string id)
        {
            var product = _productRepository.GetProductsModelViewById(id);
            return View(product);
        }
    }
}