using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;

namespace VanPhongPham.Controllers
{
    public class ProfileController : Controller
    {
        DB_VanPhongPhamDataContext db = new DB_VanPhongPhamDataContext();
        // GET: Profile
        public ActionResult Index(string view, string MaTaiKhoan)
        {
            ViewBag.PartialView = string.IsNullOrEmpty(view) ? "ProfilePartial" : view;
            ViewBag.MaTaiKhoan = MaTaiKhoan;

            if (!string.IsNullOrEmpty(MaTaiKhoan))
            {
                var dbUser = db.users.FirstOrDefault(u => u.user_id == MaTaiKhoan);
                if (dbUser != null)
                {
                    return View(dbUser); // Trả về view với đối tượng người dùng
                }
                else
                {
                    // Trường hợp không tìm thấy tài khoản
                    return View("Error", new { message = "Không tìm thấy tài khoản" });
                }
            }

            return View();
        }

        public ActionResult ProfilePartial(string MaTaiKhoan)
        {
            if (!string.IsNullOrEmpty(MaTaiKhoan))
            {
                var dbUser = db.users.FirstOrDefault(u => u.user_id == MaTaiKhoan);

                if (dbUser != null)
                {
                    return PartialView(dbUser); // Truyền thông tin user từ database vào view
                }
            }

            return PartialView();
        }

        public ActionResult AddressPartial(string MaTaiKhoan) // Thêm tham số MaTaiKhoan
        {
            if (!string.IsNullOrEmpty(MaTaiKhoan))
            {
                var dbUser = db.users.Include("addresses").FirstOrDefault(u => u.user_id == MaTaiKhoan);
                var adrs = db.addresses.Where(a => a.user_id == MaTaiKhoan).ToList();

                if (dbUser != null)
                {
                    return PartialView(adrs); // Truyền thông tin user từ database vào view
                }
            }

            return PartialView();
        }

        public ActionResult ChangePasswordPartial(string MaTaiKhoan) // Thêm tham số MaTaiKhoan
        {
            if (!string.IsNullOrEmpty(MaTaiKhoan))
            {
                var dbUser = db.users.FirstOrDefault(u => u.user_id == MaTaiKhoan);

                if (dbUser != null)
                {
                    return PartialView(dbUser); // Truyền thông tin user từ database vào view
                }
            }

            return PartialView();
        }

        public ActionResult ChangeEmailPartial(string MaTaiKhoan) // Thêm tham số MaTaiKhoan
        {
            if (!string.IsNullOrEmpty(MaTaiKhoan))
            {
                var dbUser = db.users.FirstOrDefault(u => u.user_id == MaTaiKhoan);

                if (dbUser != null)
                {
                    return PartialView(dbUser); // Truyền thông tin user từ database vào view
                }
            }

            return PartialView();
        }

        public ActionResult OrderPartial(string MaTaiKhoan) // Thêm tham số MaTaiKhoan
        {
            if (!string.IsNullOrEmpty(MaTaiKhoan))
            {
                var dbUser = db.users.FirstOrDefault(u => u.user_id == MaTaiKhoan);

                if (dbUser != null)
                {
                    return PartialView(dbUser); // Truyền thông tin user từ database vào view
                }
            }

            return PartialView();
        }

        [HttpPost]
        public ActionResult UpdateProfile(user updateUser)
        {
            try
            {
                var currentUser = db.users.FirstOrDefault(u => u.user_id == updateUser.user_id);
                if (currentUser != null)
                {
                    currentUser.full_name = updateUser.full_name;
                    currentUser.gender = updateUser.gender;
                    currentUser.dob = updateUser.dob;
                    currentUser.avt_url = updateUser.avt_url;

                    db.SubmitChanges();
                }
                return RedirectToAction("Index", new { view = "ProfilePartial", MaTaiKhoan = updateUser.user_id });
            }
            catch (Exception ex)
            {
                return View("Error", new { message = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult UpdateEmail(string user_id, string email)
        {
            try
            {
                var currentUser = db.users.FirstOrDefault(u => u.user_id == user_id);
                if (currentUser != null)
                {
                    currentUser.email = email;
                    db.SubmitChanges();
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}