using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
namespace VanPhongPham.Areas.Admin.Controllers
{
    public class SupplierController : Controller
    {
        private readonly SupplierRepository supplierRepository;
        public SupplierController()
        {
            supplierRepository = new SupplierRepository();
        }
        // GET: Admin/Supplier

        public ActionResult Index(string supplier_id)
        {
            if (supplier_id != null)
            {
                supplier supplier = supplierRepository.GetSupplierById(supplier_id);
                ViewBag.supplier = supplier;
            }
            List<supplier> listSupplier = supplierRepository.GetAllSuppliers();
            ViewBag.supplier_id = supplierRepository.GenerateSupplierId();
            return View(listSupplier);
        }

        [HttpPost]
        public ActionResult AddSupplier(string action, supplier supplier)
        {
            if (action == "add")
            {
                supplierRepository.AddSupplier(supplier);
            }
            else
            {
                supplierRepository.UpdateSupplier(supplier);
            }
            return RedirectToAction("Index", "Admin/Supplier");
        }

        public ActionResult DeleteSupplier(string supplier_id)
        {
            supplierRepository.DeleteSupplier(supplier_id);
            return RedirectToAction("Index", "Supplier");
        }
    }
}