using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class AddressViewModel
    {
        public int AddressId { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressLine { get; set; }
        public string Ward { get; set; }
        public int District { get; set; }
        public int Province { get; set; }
        public bool isDefault { get; set; }
    }
}