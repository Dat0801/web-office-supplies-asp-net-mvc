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

        public supplier GetSupplierById(string id)
        {
            return _context.suppliers.FirstOrDefault(x => x.supplier_id == id);
        }

        public supplier GetSupplierByName(string supplier_name)
        {
            return _context.suppliers.FirstOrDefault(x => x.supplier_name == supplier_name);
        }

        public List<supplier> SearchSupplier(string search_str)
        {
            return _context.suppliers
                .Where(p => p.supplier_id.Contains(search_str) ||
                            p.supplier_name.Contains(search_str) ||
                            p.email.Contains(search_str) ||
                            p.phone_number.Contains(search_str))
                .ToList();
        }

        public bool AddSupplier(supplier supplier)
        {
            try
            {
                _context.suppliers.InsertOnSubmit(supplier);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
        public bool UpdateSupplier(supplier supplier)
        {
            supplier supplierToUpdate = GetSupplierById(supplier.supplier_id);
            try
            {
                supplierToUpdate.supplier_name = supplier.supplier_name;
                supplierToUpdate.phone_number = supplier.phone_number;
                supplierToUpdate.email = supplier.email;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public bool DeleteSupplier(string id)
        {
            supplier supplierToDelete = GetSupplierById(id);
            try
            {
                _context.suppliers.DeleteOnSubmit(supplierToDelete);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public string GenerateSupplierId()
        {
            supplier supplier = _context.suppliers.ToList().LastOrDefault();
            int num = int.Parse(supplier.supplier_id.Substring(3)) + 1;
            string supplier_id = "SUP";
            if (num < 10)
                supplier_id = "SUP00";
            else if (num < 100)
                supplier_id = "SUP0";
            supplier_id += num;
            return supplier_id;
        }
    }
}