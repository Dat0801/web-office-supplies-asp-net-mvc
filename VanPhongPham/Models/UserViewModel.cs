using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime? Dob { get; set; }
        public bool Status { get; set; }
        public string AvatarUrl { get; set; } 
        public HttpPostedFileBase AvatarFile { get; set; } 
        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
        public List<string> Roles { get; set; }
    }

}