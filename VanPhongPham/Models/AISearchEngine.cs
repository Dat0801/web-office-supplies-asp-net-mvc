using FuzzySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services.Description;


namespace VanPhongPham.Models
{
    public class AISearchEngine
    {
        //private readonly TrainModelAI _trainModelAI;
        //private readonly Dictionary<string, int> _userMapping;
        //private readonly Dictionary<string, int> _productMapping;
        //private readonly List<string> _allProductIds;

        //public AISearchEngine(List<(string UserId, string ProductId, int? ViewCount, int? AddToCartCount, int? PurchaseCount)> rawData, List<ProductViewModel> products)
        //{
        //    _trainModelAI = new TrainModelAI();
        //    _trainModelAI.TrainModel(rawData); // Huấn luyện mô hình từ dữ liệu thô
        //    _trainModelAI.LoadModel(); // Tải mô hình đã huấn luyện

        //    // Tạo mappings cho người dùng và sản phẩm
        //    _userMapping = rawData.Select(x => x.UserId).Distinct()
        //                          .Select((user, index) => new { user, index })
        //                          .ToDictionary(x => x.user, x => x.index);

        //    _productMapping = rawData.Select(x => x.ProductId).Distinct()
        //                             .Select((prod, index) => new { prod, index })
        //                             .ToDictionary(x => x.prod, x => x.index);

        //    _allProductIds = products.Select(p => p.ProductId).ToList(); // Danh sách tất cả ID sản phẩm
        //}
        // Tìm sản phẩm tương tác nhiều nhất cho người dùng
        //public List<ProductViewModel> RecommendProductsForUser(string userId, List<ProductViewModel> products)
        //{
        //    var userIndex = _userMapping.ContainsKey(userId) ? _userMapping[userId] : -1;

        //    if (userIndex == -1)
        //    {
        //        return new List<ProductViewModel>(); // Nếu không tìm thấy người dùng
        //    }

        //    var predictionEngine = _trainModelAI.GetPredictionEngine(); // Tạo prediction engine từ mô hình đã tải

        //    // Dự đoán điểm số cho từng sản phẩm và sắp xếp theo điểm số
        //    var recommendedProducts = _allProductIds
        //        .Select(productId => new
        //        {
        //            ProductId = productId,
        //            Score = predictionEngine.Predict(new PurchaseData
        //            {
        //                UserId = userIndex,
        //                ProductId = _productMapping.ContainsKey(productId) ? _productMapping[productId] : -1
        //            }).Score
        //        })
        //        .OrderByDescending(x => x.Score) // Sắp xếp sản phẩm theo điểm số dự đoán
        //        .Select(x => x.ProductId)
        //        .ToList();

        //    // Trả về danh sách sản phẩm đã được sắp xếp
        //    return products
        //        .Where(p => recommendedProducts.Contains(p.ProductId))
        //        .OrderBy(p => recommendedProducts.IndexOf(p.ProductId)) // Đảm bảo sắp xếp theo điểm số
        //        .Take(5)
        //        .ToList();
        //}
        public List<ProductViewModel> FindRelevantProducts(string searchQuery, List<ProductViewModel> products)
        {
            //// Triển khai thuật toán AI hoặc kết nối đến API (ví dụ OpenAI hoặc ML.NET)
            //var lowerQuery = searchQuery.ToLower();

            //// Tìm kiếm tương đối dựa trên TF-IDF hoặc Cosine Similarity
            //var filteredProducts = products
            //    .Where(p => ComputeRelevanceScore(lowerQuery, p) > 0) // Chỉ giữ sản phẩm liên quan
            //    .OrderByDescending(p => ComputeRelevanceScore(lowerQuery, p))
            //    .ToList();


            //return filteredProducts;
            var filteredProducts = products
            .Where(p =>
                Fuzz.PartialRatio(searchQuery.ToLower(), p.ProductName.ToLower()) > 70 || // Tìm trong tên sản phẩm
                (!string.IsNullOrWhiteSpace(p.Description) && Fuzz.PartialRatio(searchQuery.ToLower(), p.Description.ToLower()) > 70) ||  // Tìm trong mô tả sản phẩm
                Fuzz.PartialRatio(searchQuery.ToLower(), p.Categories.category_name.ToLower()) > 70 || // Tìm trong tên danh mục
                p.Attributes.Any(a => Fuzz.PartialRatio(searchQuery.ToLower(), a.AttributeName.ToLower()) > 70) // Tìm trong thuộc tính
            )
            .OrderByDescending(p => Fuzz.PartialRatio(searchQuery.ToLower(), p.ProductName.ToLower())) // Sắp xếp theo điểm tương đồng
            .ToList();

            return filteredProducts;
        }
        public List<ProductViewModel> SuggestProducts(List<ProductViewModel> products, List<string> searchQueries)
        {
            var suggestedProducts = new List<ProductViewModel>();
            foreach (var query in searchQueries)
            {
                suggestedProducts.AddRange(FindRelevantProducts(query, products));
            }

            return suggestedProducts.Distinct().Take(5).ToList();
        }
        //private double ComputeRelevanceScore(string query, ProductViewModel product)
        //{
        //    var cleanDescription = Regex.Replace(product.Description ?? "", "<.•*?>", "").ToLower();
        //    //loại bỏ các ký tự đặc biệt
        //    cleanDescription = Regex.Replace(cleanDescription, @"[^\w\s]", "");

        //    var descriptionWords = cleanDescription.Split(new[] { ' ', '.', ',', ';', ':', '!', '•' }, StringSplitOptions.RemoveEmptyEntries);

        //    //var descriptionWords = product.Description?.ToLower().Split(' ') ?? Array.Empty<string>();
        //    var queryWords = query.ToLower().Split(' ');

        //    // Tạo vector từ
        //    var allWords = descriptionWords.Union(queryWords).Distinct().ToList();
        //    var queryVector = allWords.Select(w => queryWords.Count(q => q == w)).ToArray();
        //    var descriptionVector = allWords.Select(w => descriptionWords.Count(d => d == w)).ToArray();

        //    // Tính Cosine Similarity
        //    var dotProduct = queryVector.Zip(descriptionVector, (q, d) => q * d).Sum();
        //    var queryMagnitude = Math.Sqrt(queryVector.Sum(q => q * q));
        //    var descriptionMagnitude = Math.Sqrt(descriptionVector.Sum(d => d * d));

        //    var rs = dotProduct / (queryMagnitude * descriptionMagnitude + 1e-10); // Thêm epsilon để tránh chia cho 0
        //    if(rs ==0)
        //        Console.WriteLine(" " + product);            
        //    return rs;
        //}


    }
}