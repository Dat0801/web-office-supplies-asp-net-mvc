using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Areas.Admin.Filter;
using VanPhongPham.Models;
using VanPhongPham.Services;

namespace VanPhongPham.Areas.Admin.Controllers
{
    [ImportStaff]
    public class PurchaseOrderController : Controller
    {
        private readonly PurchaseOrderRepository purchaseOrderRepository;
        private readonly ProductRepository productRepository;
        private readonly SupplierRepository supplierRepository;
        private readonly ExcelReportService _excelReportService;

        public PurchaseOrderController()
        {
            purchaseOrderRepository = new PurchaseOrderRepository();
            productRepository = new ProductRepository();
            supplierRepository = new SupplierRepository();
            _excelReportService = new ExcelReportService();
        }

        // GET: Admin/PurchaseOrder
        public ActionResult Index(int? page, string search_str)
        {
            var message = TempData["Message"];
            var messageType = TempData["MessageType"];
            if (message != null)
            {
                ViewBag.Message = message;
                ViewBag.MessageType = messageType;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            List<purchase_order> purchase_Orders;
            if (search_str != null)
            {
                purchase_Orders = purchaseOrderRepository.SearchPurchaseOrders(search_str);
                ViewBag.search_str = search_str;
            }
            else
            {
                purchase_Orders = purchaseOrderRepository.GetPurchaseOrders();
            }
            return View(purchase_Orders.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Detail(int? page, string purchase_order_id)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            List<purchase_order_detail> purchase_Order_Details = purchaseOrderRepository.GetPurchaseOrderDetails(purchase_order_id);
            return View(purchase_Order_Details.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            var products = productRepository.GetProducts();
            ViewBag.suppliers = supplierRepository.GetAllSuppliers();
            ViewBag.products = new SelectList(products, "product_id", "product_name");
            ViewBag.user = Session["Admin"];
            ViewBag.purchase_order_id = purchaseOrderRepository.GeneratePurchaseOrderId();
            return View();
        }

        [HttpPost]
        public ActionResult Create(purchase_order order, List<purchase_order_detail> orderDetails)
        {
            if (orderDetails != null && orderDetails.Count > 0)
            {
                var result = purchaseOrderRepository.AddPurchaseOrder(order);
                if (result)
                {
                    foreach (var orderDetail in orderDetails)
                    {
                        orderDetail.purchase_order_id = order.purchase_order_id;
                        purchaseOrderRepository.AddPurchaseOrderDetail(orderDetail);
                    }
                    TempData["Message"] = "Tạo phiếu đặt thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Tạo phiếu đặt thất bại!";
                    TempData["MessageType"] = "danger";
                }
            }
            else
            {
                TempData["Message"] = "Không có chi tiết đơn hàng!";
                TempData["MessageType"] = "danger";
            }

            return RedirectToAction("Index");
        }

        public ActionResult PurchaseOrderExportToExcel()
        {
            var data = purchaseOrderRepository.GetPurchaseOrders();
            var userName = ((user)Session["Admin"]).full_name;
            var fileContent = _excelReportService.GeneratePurchaseOrderReport(data, userName);
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "danh_sach_phieu_dat_hang.xlsx");

        }

        public ActionResult Receipt(int? page, string search_str)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            List<receipt> receipts;
            if (search_str != null)
            {
                receipts = purchaseOrderRepository.SearchReceipt(search_str);
                ViewBag.search_str = search_str;
            }
            else
            {
                receipts = purchaseOrderRepository.GetReceipts();
            }
            return View(receipts.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ReceiptDetail(int? page, string receipt_id)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            List<receipt_detail> receipt_Details = purchaseOrderRepository.GetReceiptDetails(receipt_id);
            return View(receipt_Details.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CreateReceipt(string purchase_order_id)
        {
            string receipt_id = purchaseOrderRepository.GenerateReceiptId();
            ViewBag.receipt_id = receipt_id;
            ViewBag.entry_count = purchaseOrderRepository.GenerateEntryCount(purchase_order_id);
            List<purchase_order_detail> purchase_order_details = purchaseOrderRepository.GetPurchaseOrderDetails(purchase_order_id);
            return View(purchase_order_details);
        }

        [HttpPost]
        public ActionResult CreateReceipt(receipt receipt, List<receipt_detail> receiptDetails)
        {
            var result = purchaseOrderRepository.AddReceipt(receipt);
            if (result)
            {
                foreach (var receiptDetail in receiptDetails)
                {
                    receiptDetail.receipt_id = receipt.receipt_id;
                    receiptDetail.purchase_order_id = receipt.purchase_order_id;
                    purchaseOrderRepository.AddReceiptDetail(receiptDetail);
                }
                TempData["Message"] = "Tạo phiếu nhập hàng thành công!";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "Tạo phiếu nhập hàng thất bại!";
                TempData["MessageType"] = "danger";
            }

            return RedirectToAction("Index");
        }

    }
}