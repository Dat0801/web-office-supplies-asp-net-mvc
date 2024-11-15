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
                var existSupplier = supplierRepository.GetSupplierByName(supplier.supplier_name);
                if (existSupplier != null)
                {
                    TempData["Message"] = "Tên nhà cung cấp đã tồn tại! Vui lòng thêm tên mới hoặc kiểm tra phần khôi phục!";
                    TempData["MessageType"] = "danger";
                }
                else
                {
                    var result = supplierRepository.AddSupplier(supplier);
                    if (result)
                    {
                        TempData["Message"] = "Thêm nhà cung cấp thành công!";
                        TempData["MessageType"] = "success";
                    }
                    else
                    {
                        TempData["Message"] = "Thêm nhà cung cấp thất bại!";
                        TempData["MessageType"] = "danger";
                    }
                }
            }
            else
            {
                var result = supplierRepository.UpdateSupplier(supplier);
                if (result)
                {
                    TempData["Message"] = "Cập nhật nhà cung cấp thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Cập nhật nhà cung cấp thất bại!";
                    TempData["MessageType"] = "success";
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteSupplier(string supplier_id)
        {
            supplierRepository.DeleteSupplier(supplier_id);
            return RedirectToAction("Index", "Supplier");
        }
    }
}