using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class PromotionViewModel
    {
        public string PromotionId { get; set; }
        public string PromotionName { get; set; }
        public double DiscountPercent { get; set; }       
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public List<string> ProductIds { get; set; }
    }
}