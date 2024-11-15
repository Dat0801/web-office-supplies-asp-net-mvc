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
                .Where(c => c.status == true)
                .Select(c => new CategoryViewModel
                {
                    CategoryId = c.category_id,
                    CategoryName = c.category_name
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