using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class PurchaseData
    {
        public uint UserId { get; set; }
        public uint ProductId { get; set; }
        public float Label { get; set; }
    }
}