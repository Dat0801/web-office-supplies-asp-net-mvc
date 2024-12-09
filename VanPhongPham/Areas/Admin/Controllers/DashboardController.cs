using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
using VanPhongPham.Services;

namespace VanPhongPham.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserRepository userRepository;
        public DashboardController()
        {
            userRepository = new UserRepository();
        }
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Dashboard");
        }

        [HttpPost]
        public ActionResult CheckLoginAdmin(string username, string password)
        {
            var usr = userRepository.CheckLoginAdmin(username, password);
            if (usr != null)
            {
                Session["Admin"] = usr;
                var userRoles = usr.user_roles.FirstOrDefault();
                Session["Role"] = userRoles.role.role_name;
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }    
        }
    }
}