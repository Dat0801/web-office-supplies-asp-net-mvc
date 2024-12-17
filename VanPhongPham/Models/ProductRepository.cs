using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;

namespace VanPhongPham.Models
{
    public class ProductRepository
    {
        private readonly DB_VanPhongPhamDataContext _context;

        public ProductRepository()
        {
            _context = new DB_VanPhongPhamDataContext();
        }

        public ViewModels GetAllProducts(string priceRange = null, List<string> colors = null, List<string> brands = null)
        {
            var currentDate = DateTime.Now;
            var query = _context.products.Where(p => p.status == true);

            // Apply price range filter
            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                var prices = priceRange.Split('-');
                if (prices.Length == 2 && double.TryParse(prices[0], out double minPrice) &&
                    double.TryParse(prices[1], out double maxPrice))
                {
                    query = query.Where(p => p.price >= minPrice && p.price <= maxPrice);
                }
            }

            // Get promotions once for all products
            var promotions = _context.promotions
                .Where(p => p.status == true && p.start_date <= currentDate && p.end_date >= currentDate)
                .ToList();

            // Get products with their attributes
            var products = query
                .Select(p => new
                {
                    p.product_id,
                    p.product_name,
                    p.description,
                    p.purchase_price,
                    p.price,
                    p.stock_quantity,
                    p.sold,
                    p.avgRating,
                    p.visited,
                    p.category,
                    Images = p.images.Select(i => new ImageViewModel
                    {
                        ImageId = i.image_id,
                        ImageUrl = i.image_url,
                        IsPrimary = (bool)i.is_primary
                    }).ToList(),
                    Attributes = (from pav in _context.product_attribute_values
                                  join av in _context.attribute_values on pav.attribute_value_id equals av.attribute_value_id
                                  join a in _context.attributes on av.attribute_id equals a.attribute_id
                                  where pav.product_id == p.product_id && av.status == true && a.status == true
                                  select new AttributeViewModel
                                  {
                                      AttributeName = a.attribute_name,
                                      Value = av.value
                                  }).ToList()
                })
                .ToList();  // Materialize the query

            // Apply color and brand filters in memory
            if (colors != null && colors.Any())
            {
                products = products.Where(p => p.Attributes
                    .Any(a => a.AttributeName.ToLower() == "color" &&
                             colors.Contains(a.Value, StringComparer.OrdinalIgnoreCase)))
                    .ToList();
            }

            if (brands != null && brands.Any())
            {
                products = products.Where(p => p.Attributes
                    .Any(a => a.AttributeName.ToLower() == "brand" &&
                             brands.Contains(a.Value, StringComparer.OrdinalIgnoreCase)))
                    .ToList();
            }

            // Convert to ProductViewModel
            var productViewModels = products.Select(p => new ProductViewModel
            {
                ProductId = p.product_id,
                ProductName = p.product_name,
                Description = p.description,
                PurchasePrice = p.purchase_price,
                Price = p.price,
                PromotionPrice = promotions
                    .Where(promo => _context.product_promotions
                        .Any(pp => pp.product_id == p.product_id && pp.promotion_id == promo.promotion_id))
                    .Select(promo => p.price * (1 - promo.discount_percent / 100))
                    .FirstOrDefault() ?? p.price,
                StockQuantity = p.stock_quantity,
                SoldQuantity = p.sold,
                AvgRating = p.avgRating,
                VisitCount = p.visited,
                Images = p.Images,
                Categories = p.category,
                Attributes = p.Attributes
            }).ToList();

            var viewModels = new ViewModels
            {
                ProductViewModel = productViewModels,
                PromotionViewModel = promotions.Select(p => new PromotionViewModel
                {
                    PromotionId = p.promotion_id,
                    PromotionName = p.promotion_name,
                    Description = p.description,
                    DiscountPercent = p.discount_percent,
                    StartDate = p.start_date,
                    EndDate = p.end_date,
                }).ToList()
            };

            return viewModels;
        }


        public ViewModels GetTopSellingProducts(int topCount = 8)
        {
            var currentDate = DateTime.Now;

            var topSellingProducts = _context.products
                .Where(p => p.status == true)
                .OrderByDescending(p => p.sold)
                .Take(topCount)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.product_id,
                    ProductName = p.product_name,
                    Description = p.description,
                    PurchasePrice = p.purchase_price,
                    Price = p.price,
                    PromotionPrice = _context.product_promotions
                        .Where(pp => pp.product_id == p.product_id)
                        .Join(_context.promotions,
                            pp => pp.promotion_id,
                            promo => promo.promotion_id,
                            (pp, promo) => new { promo.discount_percent, promo.start_date, promo.end_date, promo.status })
                        .Where(promo => promo.status == true
                            && promo.start_date <= currentDate
                            && promo.end_date >= currentDate)
                        .Select(promo => p.price * (1 - promo.discount_percent / 100))
                        .FirstOrDefault() ?? p.price,
                    StockQuantity = p.stock_quantity,
                    SoldQuantity = p.sold,
                    AvgRating = p.avgRating,
                    VisitCount = p.visited,
                    Images = p.images.Select(i => new ImageViewModel
                    {
                        ImageId = i.image_id,
                        ImageUrl = i.image_url,
                        IsPrimary = (bool)i.is_primary
                    }).ToList(),
                    Categories = p.category
                })
                .ToList();

            var promotions = _context.promotions
                .Where(p => p.status == true)
                .Select(p => new PromotionViewModel
                {
                    PromotionId = p.promotion_id,
                    PromotionName = p.promotion_name,
                    Description = p.description,
                    DiscountPercent = p.discount_percent,
                    StartDate = p.start_date,
                    EndDate = p.end_date,
                }).ToList();

            var viewModels = new ViewModels
            {
                ProductViewModel = topSellingProducts,
                PromotionViewModel = promotions
            };

            return viewModels;
        }



        public ViewModels GetProductsModelViewById(string pro_id)
        {
            var currentDate = DateTime.Now;

            // Lấy danh sách khuyến mãi hợp lệ
            var promotions = _context.promotions
                .Where(p => p.status == true && p.start_date <= currentDate && p.end_date >= currentDate)
                .ToList();  // Lấy danh sách khuyến mãi vào bộ nhớ

            // Lấy sản phẩm theo product_id
            var product = _context.products
                .Where(p => p.status == true && p.product_id == pro_id)
                .Select(p => new
                {
                    p.product_id,
                    p.product_name,
                    p.description,
                    p.purchase_price,
                    p.price,
                    p.stock_quantity,
                    p.sold,
                    p.avgRating,
                    p.visited,
                    p.category,
                    p.status,
                    Images = p.images.Select(i => new ImageViewModel
                    {
                        ImageId = i.image_id,
                        ImageUrl = i.image_url,
                        IsPrimary = (bool)i.is_primary
                    }).ToList(),
                    Attributes = (from pav in _context.product_attribute_values
                                  join av in _context.attribute_values on pav.attribute_value_id equals av.attribute_value_id
                                  join a in _context.attributes on av.attribute_id equals a.attribute_id
                                  where pav.product_id == pro_id && av.status == true && a.status == true
                                  select new AttributeViewModel
                                  {
                                      AttributeName = a.attribute_name,
                                      Value = av.value
                                  }).ToList()
                })
                .FirstOrDefault();

            if (product == null)
            {
                return null;
            }

            // Tính toán PromotionPrice cho sản phẩm chính từ danh sách promotions đã tải vào bộ nhớ
            var promotionPrice = promotions
                .Where(promo => _context.product_promotions
                    .Any(pp => pp.product_id == product.product_id && pp.promotion_id == promo.promotion_id))
                .Select(promo => product.price * (1 - promo.discount_percent / 100))
                .FirstOrDefault() ?? product.price;

            var productViewModel = new ProductViewModel
            {
                ProductId = product.product_id,
                ProductName = product.product_name,
                Description = product.description,
                PurchasePrice = product.purchase_price,
                Price = product.price,
                PromotionPrice = promotionPrice,
                StockQuantity = product.stock_quantity,
                SoldQuantity = product.sold,
                AvgRating = product.avgRating,
                VisitCount = product.visited,
                Images = product.Images,
                Categories = product.category,
                Attributes = product.Attributes,
                Status = product.status
            };

            // Lấy danh sách sản phẩm liên quan trong cùng một danh mục, loại trừ sản phẩm hiện tại
            var relatedProducts = _context.products
                .Where(p => p.status == true && p.category == product.category && p.product_id != pro_id)
                .ToList()  // Chuyển dữ liệu sản phẩm liên quan vào bộ nhớ
                .Select(p => new ProductViewModel
                {
                    ProductId = p.product_id,
                    ProductName = p.product_name,
                    Description = p.description,
                    PurchasePrice = p.purchase_price,
                    Price = p.price,
                    // Tính toán PromotionPrice cho các sản phẩm liên quan từ danh sách promotions
                    PromotionPrice = promotions
                        .Where(promo => _context.product_promotions
                            .Any(pp => pp.product_id == p.product_id && pp.promotion_id == promo.promotion_id))
                        .Select(promo => p.price * (1 - promo.discount_percent / 100))
                        .FirstOrDefault() ?? p.price,
                    StockQuantity = p.stock_quantity,
                    SoldQuantity = p.sold,
                    AvgRating = p.avgRating,
                    VisitCount = p.visited,
                    Images = p.images.Select(i => new ImageViewModel
                    {
                        ImageId = i.image_id,
                        ImageUrl = i.image_url,
                        IsPrimary = (bool)i.is_primary
                    }).ToList()
                })
                .ToList();  // Chuyển dữ liệu vào bộ nhớ sau khi tính toán
            var reviews = _context.product_reviews.Where(r => r.product_id == pro_id).ToList()
                .Select(r => new ProductReviewViewModel
                {
                    ReviewId = r.review_id,
                    UserName = r.user.username,
                    AvtUrl = r.user.avt_url,
                    ProductId = r.product_id,
                    UserId = r.user_id,
                    Rating = r.rating,
                    ReviewContent = r.review_content,
                    CreatedAt = (DateTime)r.created_at
                }).ToList();
            var viewModels = new ViewModels
            {
                ProductViewModel = new List<ProductViewModel> { productViewModel },
                PromotionViewModel = promotions.Select(p => new PromotionViewModel
                {
                    PromotionId = p.promotion_id,
                    PromotionName = p.promotion_name
                }).ToList(),
                RelatedProducts = relatedProducts,
                ReviewViewModel = reviews
            };

            return viewModels;
        }
        public ViewModels GetProductsDeletedModelViewById(string pro_id)
        {
            var currentDate = DateTime.Now;

            // Lấy danh sách khuyến mãi hợp lệ
            var promotions = _context.promotions
                .Where(p => p.status == true && p.start_date <= currentDate && p.end_date >= currentDate)
                .ToList();  // Lấy danh sách khuyến mãi vào bộ nhớ

            // Lấy sản phẩm theo product_id
            var product = _context.products
                .Where(p => p.status == false && p.product_id == pro_id)
                .Select(p => new
                {
                    p.product_id,
                    p.product_name,
                    p.description,
                    p.purchase_price,
                    p.price,
                    p.stock_quantity,
                    p.sold,
                    p.avgRating,
                    p.visited,
                    p.category,
                    p.status,
                    Images = p.images.Select(i => new ImageViewModel
                    {
                        ImageId = i.image_id,
                        ImageUrl = i.image_url,
                        IsPrimary = (bool)i.is_primary
                    }).ToList(),
                    Attributes = (from pav in _context.product_attribute_values
                                  join av in _context.attribute_values on pav.attribute_value_id equals av.attribute_value_id
                                  join a in _context.attributes on av.attribute_id equals a.attribute_id
                                  where pav.product_id == pro_id && av.status == true && a.status == true
                                  select new AttributeViewModel
                                  {
                                      AttributeName = a.attribute_name,
                                      Value = av.value
                                  }).ToList()
                })
                .FirstOrDefault();

            if (product == null)
            {
                return null;
            }

            // Tính toán PromotionPrice cho sản phẩm chính từ danh sách promotions đã tải vào bộ nhớ
            var promotionPrice = promotions
                .Where(promo => _context.product_promotions
                    .Any(pp => pp.product_id == product.product_id && pp.promotion_id == promo.promotion_id))
                .Select(promo => product.price * (1 - promo.discount_percent / 100))
                .FirstOrDefault() ?? product.price;

            var productViewModel = new ProductViewModel
            {
                ProductId = product.product_id,
                ProductName = product.product_name,
                Description = product.description,
                PurchasePrice = product.purchase_price,
                Price = product.price,
                PromotionPrice = promotionPrice,
                StockQuantity = product.stock_quantity,
                SoldQuantity = product.sold,
                AvgRating = product.avgRating,
                VisitCount = product.visited,
                Images = product.Images,
                Status = product.status,
                Categories = product.category,
                Attributes = product.Attributes
            };

            // Lấy danh sách sản phẩm liên quan trong cùng một danh mục, loại trừ sản phẩm hiện tại
            var relatedProducts = _context.products
                .Where(p => p.status == true && p.category == product.category && p.product_id != pro_id)
                .ToList()  // Chuyển dữ liệu sản phẩm liên quan vào bộ nhớ
                .Select(p => new ProductViewModel
                {
                    ProductId = p.product_id,
                    ProductName = p.product_name,
                    Description = p.description,
                    PurchasePrice = p.purchase_price,
                    Price = p.price,
                    // Tính toán PromotionPrice cho các sản phẩm liên quan từ danh sách promotions
                    PromotionPrice = promotions
                        .Where(promo => _context.product_promotions
                            .Any(pp => pp.product_id == p.product_id && pp.promotion_id == promo.promotion_id))
                        .Select(promo => p.price * (1 - promo.discount_percent / 100))
                        .FirstOrDefault() ?? p.price,
                    StockQuantity = p.stock_quantity,
                    SoldQuantity = p.sold,
                    AvgRating = p.avgRating,
                    VisitCount = p.visited,
                    Images = p.images.Select(i => new ImageViewModel
                    {
                        ImageId = i.image_id,
                        ImageUrl = i.image_url,
                        IsPrimary = (bool)i.is_primary
                    }).ToList()
                })
                .ToList();  // Chuyển dữ liệu vào bộ nhớ sau khi tính toán

            var reviews = _context.product_reviews.Where(r => r.product_id == pro_id).ToList()
                .Select(r => new ProductReviewViewModel
                {
                    ReviewId = r.review_id,
                    UserName = r.user.username,
                    AvtUrl = r.user.avt_url,
                    ProductId = r.product_id,
                    UserId = r.user_id,
                    Rating = r.rating,
                    ReviewContent = r.review_content,
                    CreatedAt = (DateTime)r.created_at
                }).ToList();


            var viewModels = new ViewModels
            {
                ProductViewModel = new List<ProductViewModel> { productViewModel },
                PromotionViewModel = promotions.Select(p => new PromotionViewModel
                {
                    PromotionId = p.promotion_id,
                    PromotionName = p.promotion_name
                }).ToList(),
                RelatedProducts = relatedProducts,
                ReviewViewModel = reviews
            };

            return viewModels;
        }





        public ViewModels GetProductsModelViewByCategory(string cate_id)
        {
            var currentDate = DateTime.Now;

            // Lấy danh sách promotions
            var promotions = _context.promotions
                .Where(p => p.status == true && p.start_date <= currentDate && p.end_date >= currentDate)
                .ToList();

            // Lấy danh sách sản phẩm từ database
            var products = _context.products
                .Where(p => p.status == true && p.category_id == cate_id)
                .Select(p => new
                {
                    ProductId = p.product_id,
                    ProductName = p.product_name,
                    Description = p.description,
                    PurchasePrice = p.purchase_price,
                    Price = p.price,
                    StockQuantity = p.stock_quantity,
                    SoldQuantity = p.sold,
                    AvgRating = p.avgRating,
                    VisitCount = p.visited,
                    Images = p.images.Select(i => new ImageViewModel
                    {
                        ImageId = i.image_id,
                        ImageUrl = i.image_url,
                        IsPrimary = (bool)i.is_primary
                    }).ToList(),
                    Categories = p.category,
                    Attributes = (from pav in _context.product_attribute_values
                                  join av in _context.attribute_values on pav.attribute_value_id equals av.attribute_value_id
                                  join a in _context.attributes on av.attribute_id equals a.attribute_id
                                  where pav.product_id == p.product_id && av.status == true && a.status == true
                                  select new AttributeViewModel
                                  {
                                      AttributeName = a.attribute_name,
                                      Value = av.value
                                  }).ToList()
                })
                .ToList();

            // Lấy danh sách khuyến mãi áp dụng cho sản phẩm
            var promotionPrices = _context.product_promotions
                .Where(pp => promotions.Select(p => p.promotion_id).Contains(pp.promotion_id))
                .ToList()
                .GroupBy(pp => pp.product_id)
                .ToDictionary(
                    g => g.Key,
                    g => g.Join(promotions,
                                pp => pp.promotion_id,
                                promo => promo.promotion_id,
                                (pp, promo) => promo.discount_percent).FirstOrDefault()
                );

            // Xử lý tính toán khuyến mãi trên bộ nhớ
            var productViewModels = products.Select(p => new ProductViewModel
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Description = p.Description,
                PurchasePrice = p.PurchasePrice,
                Price = p.Price,
                PromotionPrice = promotionPrices.ContainsKey(p.ProductId)
                    ? p.Price * (1 - promotionPrices[p.ProductId] / 100)
                    : p.Price,
                StockQuantity = p.StockQuantity,
                SoldQuantity = p.SoldQuantity,
                AvgRating = p.AvgRating,
                VisitCount = p.VisitCount,
                Images = p.Images,
                Categories = p.Categories,
                Attributes = p.Attributes
            }).ToList();

            // Kiểm tra danh sách sản phẩm
            if (!productViewModels.Any())
            {
                return null;
            }

            // Trả về ViewModels
            return new ViewModels
            {
                ProductViewModel = productViewModels,
                PromotionViewModel = promotions.Select(p => new PromotionViewModel
                {
                    PromotionId = p.promotion_id,
                    PromotionName = p.promotion_name
                }).ToList(),
            };
        }


        public List<category> GetAllCategory()
        {
            return _context.categories.Where(c => c.status == true).ToList();
        }

        public string GenerateProductId()
        {
            product product = _context.products.ToList().LastOrDefault();
            int num = int.Parse(product.product_id.Substring(3)) + 1;
            string product_id = "PRO";
            if (num < 10)
                product_id = "PRO00";
            else if (num < 100)
                product_id = "PRO0";
            product_id += num;
            return product_id;
        }

        public List<product> GetProducts()
        {
            return _context.products.Where(p => p.status == true && p.category.status == true).ToList();
        }

        public product GetProduct(string product_id)
        {
            return _context.products.FirstOrDefault(p => p.status == true && p.product_id == product_id);
        }

        public product GetRecycleProduct(string product_name)
        {
            return _context.products.FirstOrDefault(p => p.status == false && p.product_name == product_name);
        }

        public product GetProductByName(string product_name)
        {
            return _context.products.FirstOrDefault(p => p.status == true && p.product_name == product_name);
        }

        public List<product> GetProductByCategoryName(string category_name)
        {
            return _context.products.Where(p => p.category.category_name == category_name && p.status == true).ToList();
        }

        public List<product> GetRecycleProducts()
        {
            return _context.products.Where(p => p.status == false).ToList();
        }

        public bool RecycleProduct(string product_id)
        {
            product product = _context.products.FirstOrDefault(p => p.product_id == product_id);
            try
            {
                product.status = true;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<product> SearchProduct(string search_str)
        {
            return _context.products
                .Where(p => p.status == true &&
                            (p.product_name.Contains(search_str) ||
                            p.product_id.Contains(search_str) ||
                            p.category.category_name.Contains(search_str)))
                .ToList();
        }

        public bool AddProduct(product product)
        {
            try
            {
                product.price = string.IsNullOrEmpty(product.price.ToString()) ? 0 : product.price;
                product.promotion_price = string.IsNullOrEmpty(product.promotion_price.ToString()) ? 0 : product.promotion_price;
                product.purchase_price = string.IsNullOrEmpty(product.purchase_price.ToString()) ? 0 : product.purchase_price;
                product.stock_quantity = string.IsNullOrEmpty(product.stock_quantity.ToString()) ? 0 : product.stock_quantity;
                product.sold = string.IsNullOrEmpty(product.sold.ToString()) ? 0 : product.sold;
                product.avgRating = string.IsNullOrEmpty(product.avgRating.ToString()) ? 0 : product.avgRating;
                product.visited = string.IsNullOrEmpty(product.visited.ToString()) ? 0 : product.visited;
                product.created_at = string.IsNullOrEmpty(product.created_at.ToString()) ? DateTime.Now : product.created_at;
                _context.products.InsertOnSubmit(product);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateProduct(product p_product)
        {
            product product = _context.products.FirstOrDefault(p => p.product_id == p_product.product_id);
            try
            {
                product.product_name = p_product.product_name;
                product.category_id = p_product.category_id;
                product.description = p_product.description;
                product.purchase_price = p_product.purchase_price;
                product.price_coefficient = p_product.price_coefficient;
                product.stock_quantity = p_product.stock_quantity;
                product.status = p_product.status;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteProduct(string p_product_id)
        {
            product product = _context.products.FirstOrDefault(p => p.product_id == p_product_id);
            try
            {
                product.status = false;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<image> GetMainImages()
        {
            return _context.images.Where(img => img.is_primary == true).ToList();
        }

        public image GetMainImageByProductId(string product_id)
        {
            return _context.images.FirstOrDefault(img => img.product_id == product_id && img.is_primary == true);
        }

        public List<image> GetAdditionalImageByProductId(string product_id)
        {
            return _context.images.Where(img => img.product_id == product_id && img.is_primary == false).ToList();
        }

        public image GetMainImageByProductUrl(string url)
        {
            return _context.images.FirstOrDefault(img => img.image_url == url);
        }

        public bool AddImages(image image)
        {
            try
            {
                _context.images.InsertOnSubmit(image);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateImage(image pimage)
        {
            image image = _context.images.FirstOrDefault(img => img.product_id == pimage.product_id);
            try
            {
                image.image_url = pimage.image_url;
                image.is_primary = pimage.is_primary;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteImage(string image_url)
        {
            image image = _context.images.FirstOrDefault(img => img.image_url == image_url);
            try
            {
                _context.images.DeleteOnSubmit(image);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateAdditionalImages(string productId, List<string> newImageUrls)
        {
            try
            {
                var existingImages = GetAdditionalImageByProductId(productId);

                foreach (var image in existingImages.ToList())
                {
                    if (!newImageUrls.Contains(image.image_url))
                    {
                        DeleteImage(image.image_url);
                    }
                }

                foreach (var newImageUrl in newImageUrls)
                {
                    if (!existingImages.Any(img => img.image_url == newImageUrl))
                    {
                        image additionalImage = new image
                        {
                            image_url = newImageUrl,
                            is_primary = false,
                            product_id = productId

                        };
                        AddImages(additionalImage);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public string GenerateCategoryId()
        {
            category category = _context.categories.ToList().LastOrDefault();
            int num = int.Parse(category.category_id.Substring(3)) + 1;
            string category_id = "CAT";
            if (num < 10)
                category_id = "CAT00";
            else if (num < 100)
                category_id = "CAT0";
            category_id += num;
            return category_id;
        }

        public List<category> GetCategories()
        {
            return _context.categories.Where(c => c.status == true).ToList();
        }

        public category GetCategory(string category_id)
        {
            return _context.categories.FirstOrDefault(cat => cat.category_id == category_id);
        }

        public category GetCategoryByName(string category_name)
        {
            return _context.categories.FirstOrDefault(cat => cat.category_name == category_name);
        }

        public List<category> SearchCategory(string search_str)
        {
            return _context.categories
                .Where(c => c.category_name.Contains(search_str) ||
                            c.category_id.Contains(search_str))
                .ToList();
        }

        public List<category> GetRecycleCategories()
        {
            return _context.categories.Where(c => c.status == false).ToList();
        }

        public bool RecycleCategory(string category_id)
        {
            category category = _context.categories.FirstOrDefault(c => c.category_id == category_id);
            try
            {
                category.status = true;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public category GetRecycleCategory(string category_name)
        {
            return _context.categories.FirstOrDefault(c => c.category_name == category_name && c.status == false);
        }

        public bool AddCategory(category category)
        {
            try
            {
                category.status = (category.status == null) ? true : category.status;
                category.created_at = (category.created_at == null) ? DateTime.Now : category.created_at;
                _context.categories.InsertOnSubmit(category);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateCategory(category pcategory)
        {
            category category = _context.categories.FirstOrDefault(cat => cat.category_id == pcategory.category_id);
            try
            {
                category.category_name = pcategory.category_name;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public bool DeleteCategory(string pcategory_id)
        {
            category category = _context.categories.FirstOrDefault(cat => cat.category_id == pcategory_id);
            try
            {
                category.status = false;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public string GenerateAttributeId()
        {
            attribute attribute = _context.attributes.ToList().LastOrDefault();
            int num = int.Parse(attribute.attribute_id.Substring(3)) + 1;
            string attribute_id = "ATT";
            if (num < 10)
                attribute_id = "ATT00";
            else if (num < 100)
                attribute_id = "ATT0";
            attribute_id += num;
            return attribute_id;
        }

        public List<attribute> GetAttributes()
        {
            return _context.attributes.Where(att => att.status == true).ToList();
        }

        public attribute GetAttribute(string attribute_id)
        {
            return _context.attributes.FirstOrDefault(att => att.attribute_id == attribute_id);
        }

        public attribute GetAttributeByName(string attribute_name)
        {
            return _context.attributes.FirstOrDefault(att => att.attribute_name == attribute_name);
        }

        public List<attribute> SearchAttribute(string search_str)
        {
            return _context.attributes
                .Where(c => c.attribute_name.Contains(search_str) ||
                            c.attribute_id.Contains(search_str))
                .ToList();
        }

        public bool AddAttribute(attribute attribute)
        {
            try
            {
                attribute.status = (attribute.status == null) ? true : attribute.status;
                _context.attributes.InsertOnSubmit(attribute);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateAttribute(attribute p_attribute)
        {
            attribute attribute = _context.attributes.FirstOrDefault(att => att.attribute_id == p_attribute.attribute_id);
            try
            {
                attribute.attribute_id = p_attribute.attribute_id;
                attribute.attribute_name = p_attribute.attribute_name;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteAttribute(string pattribute_id)
        {
            attribute attribute = _context.attributes.FirstOrDefault(att => att.attribute_id == pattribute_id);
            try
            {
                attribute.status = false;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<attribute> GetRecycleAttributes()
        {
            return _context.attributes.Where(c => c.status == false).ToList();
        }

        public bool RecycleAttribute(string attribute_id)
        {
            attribute attribute = _context.attributes.FirstOrDefault(att => att.attribute_id == attribute_id);
            try
            {
                attribute.status = true;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public attribute GetRecycleAttribute(string attribute_name)
        {
            return _context.attributes.FirstOrDefault(att => att.attribute_name == attribute_name && att.status == false);
        }

        public List<attribute_value> GetAttributeValues()
        {
            return _context.attribute_values.Where(att => att.status == true && att.attribute.status == true).ToList();
        }

        public attribute_value GetAttributeValue(string attribute_value_id)
        {
            return _context.attribute_values.FirstOrDefault(att_val => att_val.attribute_value_id == attribute_value_id);
        }

        public attribute_value GetAttributeValueByValue(string value)
        {
            return _context.attribute_values.FirstOrDefault(att => att.value == value);
        }

        public List<attribute_value> GetAttributeValueByProduct(string product_id)
        {
            var attributeValueIds = _context.product_attribute_values
                .Where(pav => pav.product_id == product_id)
                .Select(pav => pav.attribute_value_id)
                .ToList();
            var attributeValues = _context.attribute_values
                .Where(av => attributeValueIds.Contains(av.attribute_value_id))
                .ToList();
            return attributeValues;
        }

        public List<attribute_value> SearchAttributeValue(string search_str)
        {
            return _context.attribute_values
                .Where(c => c.value.Contains(search_str) ||
                            c.attribute_value_id.Contains(search_str))
                .ToList();
        }

        public bool AddAttributeValue(attribute_value attribute_Value)
        {
            try
            {
                attribute_Value.status = (attribute_Value.status == null) ? true : attribute_Value.status;
                _context.attribute_values.InsertOnSubmit(attribute_Value);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateAttributeValue(attribute_value p_attribute_Value)
        {
            attribute_value attribute_Value = _context.attribute_values.FirstOrDefault(att => att.attribute_value_id == p_attribute_Value.attribute_value_id);
            try
            {
                attribute_Value.attribute_value_id = p_attribute_Value.attribute_value_id;
                attribute_Value.attribute_id = p_attribute_Value.attribute_id;
                attribute_Value.value = p_attribute_Value.value;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteAttributeValue(string attribute_value_id)
        {
            attribute_value attribute_Value = _context.attribute_values.FirstOrDefault(att => att.attribute_value_id == attribute_value_id);

            try
            {
                attribute_Value.status = false;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<attribute_value> GetRecycleAttributeValues()
        {
            return _context.attribute_values.Where(att => att.status == false).ToList();
        }

        public bool RecycleAttributeValue(string attribute_value_id)
        {
            attribute_value attribute_Value = _context.attribute_values.FirstOrDefault(att => att.attribute_value_id == attribute_value_id);
            try
            {
                attribute_Value.status = true;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public attribute_value GetRecycleAttributeValue(string value)
        {
            return _context.attribute_values.FirstOrDefault(att => att.value == value && att.status == false);
        }

        public string GenerateAttributeValueId()
        {
            attribute_value attribute_Value = _context.attribute_values.ToList().LastOrDefault();
            int num = int.Parse(attribute_Value.attribute_value_id.Substring(3)) + 1;
            string attribute_value_id = "VAL";
            if (num < 10)
                attribute_value_id = "VAL00";
            else if (num < 100)
                attribute_value_id = "VAL0";
            attribute_value_id += num;
            return attribute_value_id;
        }

        public bool AddProductAttributeValue(product_attribute_value product_Attribute_Value)
        {
            try
            {
                _context.product_attribute_values.InsertOnSubmit(product_Attribute_Value);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<product_attribute_value> GetProductAttributeValueByProductId(string productId)
        {
            return _context.product_attribute_values.Where(att => att.product_id == productId && att.attribute_value.status == true).ToList();
        }

        public bool UpdateProductAttributeValue(string productId, List<string> newAttributeValueIds)
        {
            try
            {
                var existingAttributeValues = GetProductAttributeValueByProductId(productId);

                foreach (var attributeValue in existingAttributeValues)
                {
                    if (!newAttributeValueIds.Contains(attributeValue.attribute_value_id))
                    {
                        _context.product_attribute_values.DeleteOnSubmit(attributeValue);
                    }
                }

                foreach (var newAttributeValueId in newAttributeValueIds)
                {
                    if (!existingAttributeValues.Any(att => att.attribute_value_id == newAttributeValueId))
                    {
                        _context.product_attribute_values.InsertOnSubmit(
                            new product_attribute_value
                            {
                                product_id = productId,
                                attribute_value_id = newAttributeValueId
                            });
                    }
                }
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}