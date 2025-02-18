using Firebase.Auth;
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
        private readonly AISearchEngine aiSearchEngine;
        public ProductClientController()
        {
            _productRepository = new ProductRepository();
            if (aiSearchEngine == null)
            {
                var rawData = _productRepository.GetUserProductInteractions(); // Lấy dữ liệu tương tác của người dùng
                var products = _productRepository.GetAllProducts().ProductViewModel;

                aiSearchEngine = new AISearchEngine(rawData, products); // Khởi tạo AISearchEngine
            }
            //aiSearchEngine = new AISearchEngine();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAllProducts(string categoryID, string searchStr, string priceRange, List<string> colors, List<string> brands, int pageNumber = 1, int pageSize = 6)
        {
            var viewModel = _productRepository.GetAllProducts();
            ViewBag.Products = viewModel.ProductViewModel;
            if (viewModel == null || viewModel.ProductViewModel == null || !viewModel.ProductViewModel.Any())
            {
                ViewBag.ErrorMessage = "Không có sản phẩm!";
                return View(viewModel);
            }

            var products = viewModel.ProductViewModel;
            var userID = Session["userId"] as string;
            if (string.IsNullOrEmpty(userID))
            {
                userID = "anonymous";
            }
            if (string.IsNullOrEmpty(searchStr))
            {
                // Khi không có searchStr, gọi hàm gợi ý sản phẩm dựa trên tương tác của người dùng
                products = aiSearchEngine.RecommendProductsForUser(userID, products);
                if (products == null || !products.Any())
                {
                    products = viewModel.ProductViewModel;
                }
            }


            // Filter products by category
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

            // Filter by price range
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

            // Filter by colors
            if (colors != null && colors.Any())
            {
                products = products.Where(p =>
                    p.Attributes.Any(a =>
                        a.AttributeName.ToLower() == "màu sắc" &&
                        colors.Contains(a.Value, StringComparer.OrdinalIgnoreCase)
                    )).ToList();
            }

            // Filter by brands
            if (brands != null && brands.Any())
            {
                products = products.Where(p =>
                    p.Attributes.Any(a =>
                        a.AttributeName.ToLower() == "thương hiệu" &&
                        brands.Contains(a.Value, StringComparer.OrdinalIgnoreCase)
                    )).ToList();
            }
            //if (!string.IsNullOrEmpty(searchStr))
            //{
            //    var lowerQuery = searchStr.ToLower();
            //    products = products.Where(p => p.ProductName.ToLower().Contains(lowerQuery)).ToList();
            //}
            if (!string.IsNullOrEmpty(searchStr))
            {
                products = aiSearchEngine.FindRelevantProducts(searchStr, products);
                if (products == null || !products.Any())
                {
                    products = viewModel.ProductViewModel;
                }
            }

            // Pagination
            var totalProducts = products.Count();
            products = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            viewModel.ProductViewModel = products;

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            ViewBag.SearchStr = searchStr;
            //ViewBag.categoryID = categoryID;
            //ViewBag.PriceRange = priceRange;
            //ViewBag.Colors = colors != null ? string.Join(",", colors) : string.Empty;
            //ViewBag.Brands = brands != null ? string.Join(",", brands) : string.Empty;
            //ViewBag.PageNumber = pageNumber;
            //ViewBag.PageSize = pageSize;


            return View(viewModel);
        }
        [HttpGet]
        public ActionResult SearchSuggestions(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new List<string>(), JsonRequestBehavior.AllowGet); // Trả về danh sách rỗng nếu không có query
            var viewModel = _productRepository.GetAllProducts();
            if (viewModel == null || viewModel.ProductViewModel == null || !viewModel.ProductViewModel.Any())
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet); // Trả về danh sách rỗng nếu không có sản phẩm
            }
            var products = viewModel.ProductViewModel;
            var suggestions = aiSearchEngine.SuggestProducts(products, new List<string> { query });
            var suggestionKeywords = suggestions.Select(p => p.ProductName).Distinct().ToList();

            return Json(suggestionKeywords, JsonRequestBehavior.AllowGet); // Trả về danh sách gợi ý dạng JSON
        }
        [HttpGet]
        public JsonResult GetSuggestedProductsHaveMostInteractions()
        {
            var userID = Session["userId"] as string;
            if (string.IsNullOrEmpty(userID))
            {
                userID = "khachvanglai";
            }
            var products = _productRepository.GetAllProducts()?.ProductViewModel;

            if (products != null)
            {
                var suggestedProducts = aiSearchEngine.RecommendProductsForUser(userID, products)
                                                      .Take(5)
                                                      .Select(p => p.ProductName)
                                                      .ToList();

                return Json(suggestedProducts, JsonRequestBehavior.AllowGet);
            }

            return Json(new List<string>(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateProductInteraction(string productId, string interactionType)
        {
            var userId = Session["userId"] as string;
            if (string.IsNullOrEmpty(userId))
            {
                userId = "khachvanglai";
            }
            _productRepository.UpdateProductInteractions(userId, new List<string> { productId }, interactionType);
            return Json(new { success = true });
        }
        [HttpPost]
        public JsonResult GetProductIdByName(string productName)
        {
            var product = _productRepository.GetProductByName(productName);
            if (product != null)
            {
                return Json(new { success = true, productId = product.product_id });
            }
            return Json(new { success = false, message = "Product not found" });
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