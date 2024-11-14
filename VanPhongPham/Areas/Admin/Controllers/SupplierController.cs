using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
using PagedList;
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

        public ActionResult Index(int? page, string supplier_id, string search_str)
        {
            var message = TempData["Message"];
            var messageType = TempData["MessageType"];
            if (message != null)
            {
                ViewBag.Message = message;
                ViewBag.MessageType = messageType;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<supplier> listSupplier;
            if (supplier_id != null)
            {
                supplier supplier = supplierRepository.GetSupplierById(supplier_id);
                ViewBag.supplier = supplier;
            }
            if (search_str != null)
            {
                listSupplier = supplierRepository.SearchSupplier(search_str);
                ViewBag.search_str = search_str;
            }
            else
            {
                listSupplier = supplierRepository.GetAllSuppliers();
            }
            ViewBag.supplier_id = supplierRepository.GenerateSupplierId();
            return View(listSupplier.ToPagedList(pageNumber, pageSize));
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