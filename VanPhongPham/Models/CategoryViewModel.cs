using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class CategoryViewModel
    {
        public string CategoryId { get; set; }
        //public string ParentId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }                       
        public bool? Status { get; set; }     
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        //public List<CategoryViewModel> SubCategories { get; set; } = new List<CategoryViewModel>();
        public int ProductCount { get; set; }
        
    }
}