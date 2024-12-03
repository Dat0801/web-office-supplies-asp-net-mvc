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
        public ActionResult GetAllProducts(string categoryID, string priceRange, List<string> colors, List<string> brands)
        {
            var viewModel = _productRepository.GetAllProducts();
            if (viewModel == null || viewModel.ProductViewModel == null || !viewModel.ProductViewModel.Any())
            {
                ViewBag.ErrorMessage = "Không có sản phẩm!";
                return View(viewModel);
            }

            var products = viewModel.ProductViewModel;            
            
            if (!string.IsNullOrWhiteSpace(categoryID))
            {
                viewModel = _productRepository.GetProductsModelViewByCategory(categoryID);
                var category_name = _productRepository.GetCategory(categoryID).category_name;
                ViewBag.category_name = category_name;
                if (viewModel == null || viewModel.ProductViewModel == null)
                {
                    viewModel = new ViewModels();
                    ViewBag.ErrorMessage = "Không tìm thấy sản phẩm của danh mục này";
                    return View(viewModel);
                }
                products = viewModel.ProductViewModel;
            }
            
            if (!string.IsNullOrEmpty(priceRange))
            {
                var priceRanges = priceRange.Split(',');
                var filteredProducts = new List<ProductViewModel>();

                foreach (var range in priceRanges)
                {
                    var prices = range.Split('-');
                    if (prices.Length == 2 &&
                        double.TryParse(prices[0], out double minPrice) &&
                        double.TryParse(prices[1], out double maxPrice))
                    {
                        filteredProducts.AddRange(products.Where(p =>
                            p.Price >= minPrice && p.Price <= maxPrice));
                    }
                }

                products = filteredProducts.Distinct().ToList();
            }
            
            if (colors != null && colors.Any())
            {
                products = products.Where(p =>
                    p.Attributes.Any(a =>
                        a.AttributeName.ToLower() == "màu sắc" &&
                        colors.Contains(a.Value, StringComparer.OrdinalIgnoreCase)
                    )).ToList();
            }
            
            if (brands != null && brands.Any())
            {
                products = products.Where(p =>
                    p.Attributes.Any(a =>
                        a.AttributeName.ToLower() == "thương hiệu" &&
                        brands.Contains(a.Value, StringComparer.OrdinalIgnoreCase)
                    )).ToList();
            }

            viewModel.ProductViewModel = products;

            return View(viewModel);
        }
        public ActionResult Details(string id, string cart_id)
        {
            var product = _productRepository.GetProductsModelViewById(id);
            var deletedProduct = _productRepository.GetProductsDeletedModelViewById(id);
            ViewBag.CartID = cart_id;
            if (deletedProduct != null)
            {
                return View(deletedProduct);
            }
            else
            {
                return View(product);
            }
        }
    }
}