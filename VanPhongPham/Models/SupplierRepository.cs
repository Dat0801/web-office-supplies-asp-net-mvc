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
        public void AddSupplier(supplier supplier)
        {
            _context.suppliers.InsertOnSubmit(supplier);
            _context.SubmitChanges();
        }
        public void UpdateSupplier(supplier supplier)
        {
            supplier supplierToUpdate = GetSupplierById(supplier.supplier_id);
            supplierToUpdate.supplier_name = supplier.supplier_name;            
            supplierToUpdate.phone_number = supplier.phone_number;
            supplierToUpdate.email = supplier.email;            
            _context.SubmitChanges();
        }
        public void DeleteSupplier(string id)
        {
            supplier supplierToDelete = GetSupplierById(id);
            _context.suppliers.DeleteOnSubmit(supplierToDelete);
            _context.SubmitChanges();
        }

        public string generateSupplierId()
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