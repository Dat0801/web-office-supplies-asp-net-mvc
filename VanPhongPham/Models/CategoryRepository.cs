using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class CategoryRepository
    {
        private readonly DB_VanPhongPhamDataContext _context;

        public CategoryRepository()
        {
            _context = new DB_VanPhongPhamDataContext();
        }
        public ViewModels GetCategories()
        {
            var categories = _context.categories
                .Where(c => c.parent_category_id == null && c.status == true)
                .Select(c => new CategoryViewModel
                {
                    CategoryId = c.category_id,
                    CategoryName = c.category_name,
                    SubCategories = _context.categories
                        .Where(sub => sub.parent_category_id == c.category_id)
                        .Select(sub => new CategoryViewModel
                        {
                            CategoryId = sub.category_id,
                            CategoryName = sub.category_name
                        })
                        .ToList()
                })
                .ToList();
            var viewModels = new ViewModels
            {
                CategoryViewModel = categories
            };
            return viewModels;
        }
    }
}