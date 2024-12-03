using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class OrderViewModel
    {
        public string OrderId { get; set; }
        public string EmployeeId { get; set; }
        public string FullNameEmployee { get; set; }
        public string CustomerId { get; set; }
        public string FullNameCustomer { get; set; }
        public string InfoAddress { get; set; }
        public string OrderNote { get; set; }
        public string OrderCode { get; set; }
        public string MethodId { get; set; }
        public string MethodName { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public double? ShippingFee { get; set; }
        public double? DiscountAmount { get; set; }
        public string CounponApplied { get; set; }
        public double? TotalAmount { get; set; }
        public int OrderStatusID { get; set; }
        public string OrderStatusName { get; set; }
        public int CancellationRequested { get; set; }
        public string CancellationReason { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }
}