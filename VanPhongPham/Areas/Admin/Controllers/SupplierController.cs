using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
using PagedList;
using VanPhongPham.Areas.Admin.Filter;
namespace VanPhongPham.Areas.Admin.Controllers
{
    [Admin]
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
                supplier supp = supplierRepository.GetSupplierById(supplier_id);
                ViewBag.supplier = supp;
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
                var result = supplierRepository.RecoverSuppliers(new List<string> { supplier_id });
                if (result)
                {
                    TempData["Message"] = "Khôi phục nhà cung cấp thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Khôi phục nhà cung cấp thất bại!";
                    TempData["MessageType"] = "danger";
                }
            }
            return RedirectToAction("Index", "Supplier", new { area = "Admin" });
        }
        [HttpPost]
        public ActionResult RecoverSupplier(List<string> selectedSuppliers)
        {
            if(selectedSuppliers != null && selectedSuppliers.Any())
            {
                var result = supplierRepository.RecoverSuppliers(selectedSuppliers);
                if (result)
                {
                    TempData["Message"] = "Khôi phục nhà cung cấp thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Khôi phục nhà cung cấp thất bại!";
                    TempData["MessageType"] = "danger";
                }
            }
            return RedirectToAction("Index", "Supplier", new {area = "Admin"});
        }
        [HttpPost]
        public ActionResult ManageSupplier(string action, supplier supplier)
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
            if(action == "edit")
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
            return RedirectToAction("Index", "Supplier", new { area = "Admin" });
        }
        [HttpGet]
        public ActionResult DeleteSupplier(string supplier_id)
        {
            if (!string.IsNullOrEmpty(supplier_id))
            {
                var result = supplierRepository.DeleteSuppliers(new List<string> { supplier_id });
                if (result)
                {
                    TempData["Message"] = "Xóa nhà cung cấp thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Xóa nhà cung cấp thất bại!";
                    TempData["MessageType"] = "success";
                }
            }
            return RedirectToAction("Index", "Supplier", new { area = "Admin" });
        }

        [HttpPost]
        public ActionResult DeleteSupplier(List<string> selectedSuppliers)
        {
            if(selectedSuppliers != null && selectedSuppliers.Any())
            {
               var result = supplierRepository.DeleteSuppliers(selectedSuppliers);
                if (result)
                {
                    TempData["Message"] = "Xóa nhà cung cấp thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Xóa nhà cung cấp thất bại!";
                    TempData["MessageType"] = "success";
                }
            }
            return RedirectToAction("Index", "Supplier", new {area = "Admin"});
        }
    }
}