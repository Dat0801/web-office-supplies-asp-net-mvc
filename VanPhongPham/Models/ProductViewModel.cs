using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class ProductViewModel
    {
        public string ProductId { get; set; }        
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double? PurchasePrice { get; set; }        
        public double? Price { get; set; }
        public double? PromotionPrice { get; set; }
        public int? StockQuantity { get; set; }
        public int? SoldQuantity { get; set; }
        public double? AvgRating { get; set; }
        public int? VisitCount { get; set; }        
        public bool? Status { get; set; }
        public List<ImageViewModel> Images { get; set; }
        public category Categories { get; set; }
        public List<AttributeViewModel> Attributes { get; set; }
    }
}