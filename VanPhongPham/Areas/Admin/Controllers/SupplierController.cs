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

        public ActionResult Index(string search_str, string supplier_id)
        {
            List<supplier> listSup;
            if (search_str != null)
            {
                listSup = supplierRepository.SearchSupplier(search_str);
                ViewBag.searchStr = search_str;
            }
            else
            {
                listSup = supplierRepository.GetAllSuppliers();
            }            
            ViewBag.supplier_id = supplierRepository.GenerateSupplierId();
            supplier supp = supplierRepository.GetSupplierById(supplier_id);
            ViewBag.supplier = supp;
            return View(listSup);
        }
        public ActionResult RecoverSupplier(string search_str)
        {
            List<supplier> sup;
            if(search_str != null)
            {
                sup = supplierRepository.SearchDeletedSupplier(search_str);
                ViewBag.searchStr = search_str;
            }
            else
                sup = supplierRepository.GetDeletedSuppliers();
            return View(sup);
        }
        [HttpGet]
        public ActionResult RecoverSingleSupplier(string supplier_id)
        {
            if (!string.IsNullOrEmpty(supplier_id))
            {
                supplierRepository.RecoverSuppliers(new List<string> { supplier_id });
            }
            return RedirectToAction("RecoverSupplier", "Supplier", new { area = "Admin" });
        }
        [HttpPost]
        public ActionResult RecoverSupplier(List<string> selectedSuppliers)
        {
            if(selectedSuppliers != null && selectedSuppliers.Any())
            {
                supplierRepository.RecoverSuppliers(selectedSuppliers);
            }
            return RedirectToAction("RecoverSupplier", "Supplier", new {area = "Admin"});
        }
        [HttpPost]
        public ActionResult ManageSupplier(string action, supplier supplier)
        {
            if (action == "add")
            {
                supplierRepository.AddSupplier(supplier);
            }
            if(action == "edit")
            {
                supplierRepository.UpdateSupplier(supplier);
            }           
            return RedirectToAction("Index","Supplier", new {area = "Admin"});
        }
        [HttpGet]
        public ActionResult DeleteSupplier(string supplier_id)
        {
            if (!string.IsNullOrEmpty(supplier_id))
            {
                supplierRepository.DeleteSuppliers(new List<string> { supplier_id });
            }
            return RedirectToAction("Index", "Supplier", new { area = "Admin" });
        }
        [HttpPost]
        public ActionResult DeleteSupplier(List<string> selectedSuppliers)
        {
            if(selectedSuppliers != null && selectedSuppliers.Any())
            {
                supplierRepository.DeleteSuppliers(selectedSuppliers);
            }
            return RedirectToAction("Index", "Supplier", new {area = "Admin"});
        }
    }
}