using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class CouponViewModel
    {
        public string CouponId { get; set; }
        public string CouponCode { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DiscountPercent { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Status { get; set; }
        public HttpPostedFileBase CouponFile { get; set; }
    }
}