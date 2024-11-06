using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VanPhongPham.Models;
using VanPhongPham.Services;

namespace VanPhongPham.Controllers
{
    public class AuthController : Controller
    {
        DB_VanPhongPhamDataContext db = new DB_VanPhongPhamDataContext();

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public async Task<ActionResult> GoogleSignIn(string token)
        {
            try
            {
                FirebaseToken decodedToken = await FirebaseService.VerifyTokenAsync(token);
                string uid = decodedToken.Uid; // UID từ Firebase
                string email = decodedToken.Claims["email"].ToString();
                string displayName = decodedToken.Claims["name"].ToString();
                string userName = email.Substring(0, email.IndexOf('@'));
                // Kiểm tra hoặc tạo người dùng mới dựa trên email
                var user = db.users.SingleOrDefault(u => u.email == email);
                if (user == null)
                {
                    user = new user
                    {
                        user_id = uid,
                        email = email,
                        full_name = displayName,
                        username = userName
                    };

                    db.users.InsertOnSubmit(user);
                    db.SubmitChanges();

                    user_role usrrole = new user_role
                    {
                        user_id = uid,
                        role_id = 1 
                    };

                    db.user_roles.InsertOnSubmit(usrrole);
                    db.SubmitChanges();

                    cart_section cart = new cart_section
                    {
                        user_id = uid
                    };
                    db.cart_sections.InsertOnSubmit(cart);
                    db.SubmitChanges();
                }

                return Json(new { success = true, token = token });
            }
            catch (FirebaseAuthException authEx)
            {
                return Json(new { success = false, message = "Lỗi xác thực: " + authEx.Message });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi chung
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        public ActionResult ResetPassword(string email)
        {
            ViewBag.Email = email; // Lưu email vào ViewBag
            return View();
        }
    }
}