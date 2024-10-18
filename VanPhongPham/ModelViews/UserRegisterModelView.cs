using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.ModelViews
{
    public class UserRegisterModelView
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        
        //public string ShippingAddress { get; set;}
    }
}