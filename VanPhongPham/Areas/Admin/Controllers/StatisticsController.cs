using System.Collections.Generic;
using System.Web.Mvc;
using System;
using VanPhongPham.Models;
using System.Linq;

namespace VanPhongPham.Areas.Admin.Controllers
{
    public class StatisticsController : Controller
    {
        private DB_VanPhongPhamDataContext db = new DB_VanPhongPhamDataContext();

        //public ActionResult Index(int? month = null, int? year = null, string period = "custom")
        //{
        //    DateTime startDate;
        //    DateTime endDate;

        //    if (month.HasValue && year.HasValue)
        //    {                
        //        startDate = new DateTime(year.Value, month.Value, 1);
        //        endDate = startDate.AddMonths(1).AddDays(-1); 
        //    }
        //    else
        //    {                
        //        endDate = DateTime.Now;
        //        switch (period.ToLower())
        //        {
        //            case "week":
        //                startDate = DateTime.Now.AddDays(-7);
        //                break;
        //            case "month":
        //                startDate = DateTime.Now.AddMonths(-1);
        //                break;
        //            case "year":
        //                startDate = DateTime.Now.AddYears(-1);
        //                break;
        //            default:
        //                startDate = DateTime.Now.AddDays(-7);
        //                break;
        //        }
        //    }

        //    var statistics = new StatisticsViewModel
        //    {
        //        Period = period,
        //        SelectedMonth = month,
        //        SelectedYear = year,
        //        TotalRevenue = CalculateRevenue(startDate, endDate),
        //        TotalProfit = CalculateProfit(startDate, endDate),
        //        BestSellingProducts = GetBestSellingProducts(5, startDate, endDate),
        //        SlowSellingProducts = GetSlowSellingProducts(5, startDate, endDate),
        //        HighestRatedProducts = GetHighestRatedProducts(5, startDate, endDate),
        //        LowestRatedProducts = GetLowestRatedProducts(5, startDate, endDate)
        //    };

        //    return View(statistics);
        //}
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


    }
}