using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class ProductStatisticsViewModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalQuantitySold { get; set; }
        public double TotalRevenue { get; set; }
        public double TotalProfit { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}