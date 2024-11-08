using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
namespace VanPhongPham.Areas.Admin.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private readonly PurchaseOrderRepository purchaseOrderRepository;
        private readonly ProductRepository productRepository;
        private readonly SupplierRepository supplierRepository;


        public PurchaseOrderController()
        {
            purchaseOrderRepository = new PurchaseOrderRepository();
            productRepository = new ProductRepository();
            supplierRepository = new SupplierRepository();
        }

        // GET: Admin/PurchaseOrder
        public ActionResult Index(int? page, string search_str)
        {
            int pageSize = 5;
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
            int pageSize = 5;
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
            purchaseOrderRepository.AddPurchaseOrder(order);
            foreach(var orderDetail in orderDetails)
            {
                orderDetail.purchase_order_id = order.purchase_order_id;
                purchaseOrderRepository.AddPurchaseOrderDetail(orderDetail);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Receipt(int? page, string search_str)
        {
            int pageSize = 5;
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
            int pageSize = 5;
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
            purchaseOrderRepository.AddReceipt(receipt);
            foreach (var receiptDetail in receiptDetails)
            {
                receiptDetail.receipt_id = receipt.receipt_id;
                receiptDetail.purchase_order_id = receipt.purchase_order_id;
                purchaseOrderRepository.AddReceiptDetail(receiptDetail);
            }
            return RedirectToAction("Index");
        }

    }
}