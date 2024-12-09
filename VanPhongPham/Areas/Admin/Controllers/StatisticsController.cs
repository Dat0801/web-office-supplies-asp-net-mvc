using System.Collections.Generic;
using System.Web.Mvc;
using System;
using VanPhongPham.Models;
using System.Linq;
using VanPhongPham.Areas.Admin.Filter;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
namespace VanPhongPham.Areas.Admin.Controllers
{
    [Admin]
    public class StatisticsController : Controller
    {
        private DB_VanPhongPhamDataContext db = new DB_VanPhongPhamDataContext();

        
        public ActionResult Index(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                startDate = DateTime.Now.AddDays(-7);
                endDate = DateTime.Now;
            }
            endDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);

            var statistics = new StatisticsViewModel
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                TotalRevenue = CalculateRevenue(startDate.Value, endDate.Value),
                TotalProfit = CalculateProfit(startDate.Value, endDate.Value),
                BestSellingProducts = GetBestSellingProducts(5, startDate.Value, endDate.Value),
                SlowSellingProducts = GetSlowSellingProducts(5, startDate.Value, endDate.Value),
                HighestRatedProducts = GetHighestRatedProducts(5, startDate.Value, endDate.Value),
                LowestRatedProducts = GetLowestRatedProducts(5, startDate.Value, endDate.Value)
            };
            
            ViewBag.TotalRevenue = statistics.TotalRevenue;
            ViewBag.TotalProfit = statistics.TotalProfit;
            ViewBag.BestSellingProducts = statistics.BestSellingProducts;
            ViewBag.SlowSellingProducts = statistics.SlowSellingProducts;

            return View(statistics);
        }
        public ActionResult Revenue(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                startDate = DateTime.Now.AddDays(-7);
                endDate = DateTime.Now;
            }
            endDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);

            var statistics = new StatisticsViewModel
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                TotalRevenue = CalculateRevenue(startDate.Value, endDate.Value),
                TotalProfit = CalculateProfit(startDate.Value, endDate.Value),
                BestSellingProducts = GetBestSellingProducts(5, startDate.Value, endDate.Value),
                SlowSellingProducts = GetSlowSellingProducts(5, startDate.Value, endDate.Value),
                HighestRatedProducts = GetHighestRatedProducts(5, startDate.Value, endDate.Value),
                LowestRatedProducts = GetLowestRatedProducts(5, startDate.Value, endDate.Value)
            };

            ViewBag.TotalRevenue = statistics.TotalRevenue;
            ViewBag.TotalProfit = statistics.TotalProfit;
            ViewBag.BestSellingProducts = statistics.BestSellingProducts;
            ViewBag.SlowSellingProducts = statistics.SlowSellingProducts;

            return View(statistics);
        }
        public ActionResult Products(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                startDate = DateTime.Now.AddDays(-7);
                endDate = DateTime.Now;
            }
            endDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);

            var statistics = new StatisticsViewModel
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                TotalRevenue = CalculateRevenue(startDate.Value, endDate.Value),
                TotalProfit = CalculateProfit(startDate.Value, endDate.Value),
                BestSellingProducts = GetBestSellingProducts(5, startDate.Value, endDate.Value),
                SlowSellingProducts = GetSlowSellingProducts(5, startDate.Value, endDate.Value),
                HighestRatedProducts = GetHighestRatedProducts(5, startDate.Value, endDate.Value),
                LowestRatedProducts = GetLowestRatedProducts(5, startDate.Value, endDate.Value)
            };

            

            return View(statistics);
        }
        public ActionResult Rates(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                startDate = DateTime.Now.AddDays(-7);
                endDate = DateTime.Now;
            }
            endDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);

            var statistics = new StatisticsViewModel
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                TotalRevenue = CalculateRevenue(startDate.Value, endDate.Value),
                TotalProfit = CalculateProfit(startDate.Value, endDate.Value),
                BestSellingProducts = GetBestSellingProducts(5, startDate.Value, endDate.Value),
                SlowSellingProducts = GetSlowSellingProducts(5, startDate.Value, endDate.Value),
                HighestRatedProducts = GetHighestRatedProducts(5, startDate.Value, endDate.Value),
                LowestRatedProducts = GetLowestRatedProducts(5, startDate.Value, endDate.Value)
            };

            ViewBag.TotalRevenue = statistics.TotalRevenue;
            ViewBag.TotalProfit = statistics.TotalProfit;
            ViewBag.BestSellingProducts = statistics.BestSellingProducts;
            ViewBag.SlowSellingProducts = statistics.SlowSellingProducts;

            return View(statistics);
        }

        private double CalculateRevenue(DateTime startDate, DateTime endDate)
        {
            var revenue = db.order_details
                .Where(od => od.order.created_at >= startDate && od.order.created_at <= endDate)
                .Select(od => od.price * od.quantity)
                .Sum();
            return revenue ?? 0;
        }

        private double CalculateProfit(DateTime startDate, DateTime endDate)
        {
            var profit = db.order_details
                .Where(od => od.order.created_at >= startDate && od.order.created_at <= endDate)
                .GroupBy(od => od.product_id)
                .Select(g => new
                {
                    SoldAmount = g.Sum(x => x.price * x.quantity),
                    CostAmount = db.products
                                  .Where(p => p.product_id == g.Key)
                                  .Select(p => p.purchase_price * g.Sum(x => x.quantity))
                                  .FirstOrDefault()
                })
                .Sum(x => x.SoldAmount - x.CostAmount);
            if (profit == null)
            {
                return 0;
            }
            else
            {
                return (double)profit;
            }
        }


        private List<ProductStatisticsViewModel> GetBestSellingProducts(int count, DateTime startDate, DateTime endDate)
        {
            var query = db.order_details
                .Where(od => od.order.created_at >= startDate && od.order.created_at <= endDate)
                .GroupBy(od => new { od.product_id, od.product.product_name, od.product.purchase_price })
                .Select(g => new ProductStatisticsViewModel
                {
                    ProductId = g.Key.product_id,
                    ProductName = g.Key.product_name,
                    TotalQuantitySold = (int)g.Sum(x => x.quantity),
                    TotalRevenue = (double)g.Sum(x => x.price * x.quantity),
                    TotalProfit = (double)((g.Sum(x => x.price * x.quantity)) -
                                  (g.Key.purchase_price * g.Sum(x => x.quantity)))
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(count)
                .ToList();

            return query;
        }



        private List<ProductStatisticsViewModel> GetSlowSellingProducts(int count, DateTime startDate, DateTime endDate)
        {
            var query = db.order_details
                .Where(od => od.order.created_at >= startDate && od.order.created_at <= endDate)
                .GroupBy(od => new { od.product_id, od.product.product_name, od.product.purchase_price })
                .Select(g => new ProductStatisticsViewModel
                {
                    ProductId = g.Key.product_id,
                    ProductName = g.Key.product_name,
                    TotalQuantitySold = (int)g.Sum(x => x.quantity),
                    TotalRevenue = (double)g.Sum(x => x.price * x.quantity),
                    TotalProfit = (double)((g.Sum(x => x.price * x.quantity)) -
                                  (g.Key.purchase_price * g.Sum(x => x.quantity)))
                })
                .OrderBy(x => x.TotalQuantitySold)
                .Take(count)
                .ToList();

            return query;
        }



        private List<ProductStatisticsViewModel> GetHighestRatedProducts(int count, DateTime startDate, DateTime endDate)
        {
            var query = db.product_reviews
                .Where(r => r.created_at >= startDate && r.created_at <= endDate)
                .GroupBy(r => new { r.product_id, r.product.product_name })
                .Select(g => new ProductStatisticsViewModel
                {
                    ProductId = g.Key.product_id,
                    ProductName = g.Key.product_name,
                    AverageRating = g.Average(r => r.rating),
                    ReviewCount = g.Count()
                })
                .OrderByDescending(x => x.AverageRating)
                .Take(count)
                .ToList();

            return query;
        }



        private List<ProductStatisticsViewModel> GetLowestRatedProducts(int count, DateTime startDate, DateTime endDate)
        {
            var query = db.product_reviews
                .Where(r => r.created_at >= startDate && r.created_at <= endDate)
                .GroupBy(r => new { r.product_id, r.product.product_name })
                .Select(g => new ProductStatisticsViewModel
                {
                    ProductId = g.Key.product_id,
                    ProductName = g.Key.product_name,
                    AverageRating = g.Average(r => r.rating),
                    ReviewCount = g.Count()
                })
                .OrderBy(x => x.AverageRating)
                .Take(count)
                .ToList();

            return query;
        }
        public ActionResult ExportToExcel(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                startDate = DateTime.Now.AddDays(-7);
                endDate = DateTime.Now;
            }
            endDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);

            // Lấy dữ liệu thống kê
            var bestSellingProducts = GetBestSellingProducts(10, startDate.Value, endDate.Value);
            var slowSellingProducts = GetSlowSellingProducts(10, startDate.Value, endDate.Value);
            var highestRatedProducts = GetHighestRatedProducts(10, startDate.Value, endDate.Value);
            var lowestRatedProducts = GetLowestRatedProducts(10, startDate.Value, endDate.Value);

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                // Tạo worksheet cho Best Selling Products
                var bestSellingSheet = package.Workbook.Worksheets.Add("Sản phẩm bán chạy");
                bestSellingSheet.Cells[1, 1].Value = "Mã sản phẩm";
                bestSellingSheet.Cells[1, 2].Value = "Tên sản phẩm";
                bestSellingSheet.Cells[1, 3].Value = "Tổng số lượng bán";
                bestSellingSheet.Cells[1, 4].Value = "Tổng doanh thu";
                bestSellingSheet.Cells[1, 5].Value = "Tổng lợi nhuận";

                int row = 2;
                foreach (var product in bestSellingProducts)
                {
                    bestSellingSheet.Cells[row, 1].Value = product.ProductId;
                    bestSellingSheet.Cells[row, 2].Value = product.ProductName;
                    bestSellingSheet.Cells[row, 3].Value = product.TotalQuantitySold;
                    bestSellingSheet.Cells[row, 4].Value = product.TotalRevenue;
                    bestSellingSheet.Cells[row, 5].Value = product.TotalProfit;
                    row++;
                }

                // Tạo worksheet cho Slow Selling Products
                var slowSellingSheet = package.Workbook.Worksheets.Add("Sản phẩm bán chậm");
                slowSellingSheet.Cells[1, 1].Value = "Mã sản phẩm";
                slowSellingSheet.Cells[1, 2].Value = "Tên sản phẩm";
                slowSellingSheet.Cells[1, 3].Value = "Tổng số lượng bán";
                slowSellingSheet.Cells[1, 4].Value = "Tổng doanh thu";
                slowSellingSheet.Cells[1, 5].Value = "Tổng lợi nhuận";

                row = 2;
                foreach (var product in slowSellingProducts)
                {
                    slowSellingSheet.Cells[row, 1].Value = product.ProductId;
                    slowSellingSheet.Cells[row, 2].Value = product.ProductName;
                    slowSellingSheet.Cells[row, 3].Value = product.TotalQuantitySold;
                    slowSellingSheet.Cells[row, 4].Value = product.TotalRevenue;
                    slowSellingSheet.Cells[row, 5].Value = product.TotalProfit;
                    row++;
                }

                // Tạo worksheet cho Highest Rated Products
                var highestRatedSheet = package.Workbook.Worksheets.Add("Sản phẩm được đánh giá cao");
                highestRatedSheet.Cells[1, 1].Value = "Mã sản phẩm";
                highestRatedSheet.Cells[1, 2].Value = "Tên sản phẩm";
                highestRatedSheet.Cells[1, 3].Value = "Đánh giá trung bình";
                highestRatedSheet.Cells[1, 4].Value = "Số lượt đánh giá";

                row = 2;
                foreach (var product in highestRatedProducts)
                {
                    highestRatedSheet.Cells[row, 1].Value = product.ProductId;
                    highestRatedSheet.Cells[row, 2].Value = product.ProductName;
                    highestRatedSheet.Cells[row, 3].Value = product.AverageRating;
                    highestRatedSheet.Cells[row, 4].Value = product.ReviewCount;
                    row++;
                }

                // Tạo worksheet cho Lowest Rated Products
                var lowestRatedSheet = package.Workbook.Worksheets.Add("Sản phẩm bị đánh giá thấp");
                lowestRatedSheet.Cells[1, 1].Value = "Mã sản phẩm";
                lowestRatedSheet.Cells[1, 2].Value = "Tên sản phẩm";
                lowestRatedSheet.Cells[1, 3].Value = "Đánh giá trung bình";
                lowestRatedSheet.Cells[1, 4].Value = "Số lượt đánh giá";

                row = 2;
                foreach (var product in lowestRatedProducts)
                {
                    lowestRatedSheet.Cells[row, 1].Value = product.ProductId;
                    lowestRatedSheet.Cells[row, 2].Value = product.ProductName;
                    lowestRatedSheet.Cells[row, 3].Value = product.AverageRating;
                    lowestRatedSheet.Cells[row, 4].Value = product.ReviewCount;
                    row++;
                }

                // Thiết lập style cho tiêu đề
                foreach (var sheet in package.Workbook.Worksheets)
                {
                    using (var range = sheet.Cells[1, 1, 1, sheet.Dimension.Columns])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                }

                // Trả file về cho người dùng
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                var fileName = $"Statistics_{startDate.Value:yyyyMMdd}_{endDate.Value:yyyyMMdd}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

    }
}