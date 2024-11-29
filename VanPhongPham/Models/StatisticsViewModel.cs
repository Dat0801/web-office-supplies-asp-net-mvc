using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VanPhongPham.Areas.Admin.Controllers;

namespace VanPhongPham.Models
{
    public class StatisticsViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalRevenue { get; set; }
        public double TotalProfit { get; set; }
        public List<ProductStatisticsViewModel> BestSellingProducts { get; set; }
        public List<ProductStatisticsViewModel> SlowSellingProducts { get; set; }
        public List<ProductStatisticsViewModel> HighestRatedProducts { get; set; }
        public List<ProductStatisticsViewModel> LowestRatedProducts { get; set; }
    }


}