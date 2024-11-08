using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class PurchaseOrderRepository
    {
        private readonly DB_VanPhongPhamDataContext _context;

        public PurchaseOrderRepository()
        {
            _context = new DB_VanPhongPhamDataContext();
        }

        public List<purchase_order> GetPurchaseOrders()
        {
            return _context.purchase_orders.ToList();
        }

        public purchase_order GetPurchaseOrder(string purchase_order_id)
        {
            return _context.purchase_orders.FirstOrDefault(po => po.purchase_order_id == purchase_order_id);
        }

        public List<purchase_order_detail> GetPurchaseOrderDetails(string purchase_order_id)
        {
            return _context.purchase_order_details.Where(po => po.purchase_order_id == purchase_order_id).ToList();
        }

        public List<purchase_order> SearchPurchaseOrders(string search_str)
        {
            return _context.purchase_orders
                .Where(p => p.purchase_order_id.Contains(search_str) ||
                            p.supplier_id.Contains(search_str) ||
                            p.employee_id.Contains(search_str) ||
                            p.status.Contains(search_str))
                .ToList();
        }

        public string GeneratePurchaseOrderId()
        {
            purchase_order purchase_Order = _context.purchase_orders.ToList().LastOrDefault();
            int num = int.Parse(purchase_Order.purchase_order_id.Substring(3)) + 1;
            string purchase_order_id = "POD";
            if (num < 10)
                purchase_order_id = "POD00";
            else if (num < 100)
                purchase_order_id = "POD0";
            purchase_order_id += num;
            return purchase_order_id;
        }

        public bool AddPurchaseOrder(purchase_order purchase_Order)
        {
            try
            {
                if (string.IsNullOrEmpty(purchase_Order.status))
                {
                    purchase_Order.status = "Đang giao";
                }

                if (purchase_Order.created_at == null)
                {
                    purchase_Order.created_at = DateTime.Now;
                }

                _context.purchase_orders.InsertOnSubmit(purchase_Order);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool AddPurchaseOrderDetail(purchase_order_detail purchase_Order_Detail)
        {
            try
            {
                if (string.IsNullOrEmpty(purchase_Order_Detail.quantity_received.ToString()))
                {
                    purchase_Order_Detail.quantity_received = 0;
                }
                _context.purchase_order_details.InsertOnSubmit(purchase_Order_Detail);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<receipt> GetReceipts()
        {
            return _context.receipts.ToList();
        }

        public receipt GetReceipt(string receipt_id)
        {
            return _context.receipts.FirstOrDefault(po => po.receipt_id == receipt_id);
        }

        public List<receipt_detail> GetReceiptDetails(string receipt_id)
        {
            return _context.receipt_details.Where(po => po.receipt_id == receipt_id).ToList();
        }

        public List<receipt> SearchReceipt(string search_str)
        {
            return _context.receipts
                .Where(p => p.receipt_id.Contains(search_str) ||
                            p.purchase_order_id.Contains(search_str) ||
                            p.entry_count.ToString().Contains(search_str))
                .ToList();
        }

        public string GenerateReceiptId()
        {
            receipt receipt = _context.receipts.ToList().LastOrDefault();
            int num = int.Parse(receipt.receipt_id.Substring(3)) + 1;
            string receipt_id = "REC";
            if (num < 10)
                receipt_id = "REC00";
            else if (num < 100)
                receipt_id = "REC0";
            receipt_id += num;
            return receipt_id;
        }

        public int GenerateEntryCount(string purchase_order_id)
        {
            receipt receipt = _context.receipts
                                .Where(r => r.purchase_order_id == purchase_order_id)
                                .OrderByDescending(r => r.created_at)
                                .FirstOrDefault();
            int entry_count = 1;
            if (receipt != null)
            {
                entry_count = (int)(receipt.entry_count + 1);
            }
            return entry_count;
        }

        public bool AddReceipt(receipt receipt)
        {
            try
            {
                if (receipt.created_at == null)
                {
                    receipt.created_at = DateTime.Now;
                }
                _context.receipts.InsertOnSubmit(receipt);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool AddReceiptDetail(receipt_detail receipt_Detail)
        {
            try
            {
                _context.receipt_details.InsertOnSubmit(receipt_Detail);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}