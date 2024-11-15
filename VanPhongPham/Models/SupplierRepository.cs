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
            return _context.suppliers.Where(s => s.status == true).ToList();
        }
        public List<supplier> GetDeletedSuppliers()
        {
            return _context.suppliers.Where(s => s.status == false).ToList();
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
                .Where(p => p.status == true &&
                           (p.supplier_name.Contains(search_str) ||
                            p.supplier_id.Contains(search_str) ||
                            p.email.Contains(search_str)))
                .ToList();
        }
        public List<supplier> SearchDeletedSupplier(string search_str)
        {
            return _context.suppliers
                .Where(p => p.status == false &&
                           (p.supplier_name.Contains(search_str) ||
                            p.supplier_id.Contains(search_str) ||
                            p.email.Contains(search_str)))
                .ToList();
        }

        public bool AddSupplier(supplier sup)
        {
            try
            {
                sup.status = true;
                _context.suppliers.InsertOnSubmit(sup);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
        public bool UpdateSupplier(supplier sup)
        {
            supplier supplierToUpdate = GetSupplierById(sup.supplier_id);
            try
            {
                supplierToUpdate.supplier_name = sup.supplier_name;
                supplierToUpdate.phone_number = sup.phone_number;
                supplierToUpdate.email = sup.email;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public bool DeleteSuppliers(List<string> ids)
        {
            try
            {
                var suppliersToDelete = _context.suppliers.Where(s => ids.Contains(s.supplier_id)).ToList();
                suppliersToDelete.ForEach(s => s.status = false);

                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool RecoverSuppliers(List<string> ids)
        {
            try
            {
                var suppliersToRecover = _context.suppliers.Where(s => ids.Contains(s.supplier_id)).ToList();
                suppliersToRecover.ForEach(s => s.status = true);

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
            supplier sup = _context.suppliers.ToList().LastOrDefault();
            int num = int.Parse(sup.supplier_id.Substring(3)) + 1;
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