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
    }
}