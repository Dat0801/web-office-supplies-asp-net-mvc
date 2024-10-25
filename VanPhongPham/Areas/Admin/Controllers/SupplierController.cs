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
        public ActionResult Index()
        {
            List<supplier> listSupplier = supplierRepository.GetAllSuppliers();
            return View(listSupplier);
        }

        [HttpGet]
        public ActionResult AddSupplier()
        {            
            return View();
        }
        [HttpPost]
        public ActionResult AddSupplier(supplier supplier)
        {
            supplierRepository.AddSupplier(supplier);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult UpdateSupplier(string id)
        {
            supplier supplier = supplierRepository.GetSupplierById(id);
            return View(supplier);
        }
        [HttpPost]
        public ActionResult UpdateSupplier(supplier supplier)
        {
            supplierRepository.UpdateSupplier(supplier);
            return View(supplier);
        }
        [HttpPost]
        public ActionResult DeleteSupplier(string id)
        {
            supplierRepository.DeleteSupplier(id);
            return RedirectToAction("Index");
        }
    }
}