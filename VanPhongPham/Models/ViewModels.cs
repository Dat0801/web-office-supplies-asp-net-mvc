using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class ViewModels
    {
        public List<CategoryViewModel> CategoryViewModel { get; set; }
        public List<PromotionViewModel> PromotionViewModel { get; set; }       
        public List<ProductViewModel> ProductViewModel { get; set; }
        public List<ProductViewModel> RelatedProducts { get; set; }
        public List<ProductReviewViewModel> ReviewViewModel { get; set; }
    }
}