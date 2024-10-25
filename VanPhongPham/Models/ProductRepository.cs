using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class ProductRepository
    {
        private readonly DB_VanPhongPhamDataContext _context;

        public ProductRepository()
        {
            _context = new DB_VanPhongPhamDataContext();
        }

        public List<product> GetAllProducts()
        {
            return _context.products.ToList();
        }
    }
}