using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VanPhongPham.Models;
namespace VanPhongPham.Models
{
    public class SupplierRepository
    {
        private readonly DB_VanPhongPhamDataContext _context;

        public SupplierRepository ()
        {
            _context = new DB_VanPhongPhamDataContext();
        }

        public List<supplier> GetAllSuppliers()
        {
            return _context.suppliers.ToList();
        }
    }
}