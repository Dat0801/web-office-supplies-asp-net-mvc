using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class ProductReviewViewModel
    {
        public int ReviewId { get; set; }
        public string UserName { get; set; }
        public string AvtUrl { get; set; }
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public int Rating { get; set; }
        public string ReviewContent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}