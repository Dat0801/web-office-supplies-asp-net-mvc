using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.APIService;
using VanPhongPham.ModelViews;
namespace VanPhongPham.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        private readonly ApiService _apiService;

        public AuthController()
        {
            _apiService = new ApiService();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // GET: Auth/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(UserLoginModelView user)
        {           
            var rs = await _apiService.LoginAsync(user);
            if (rs)
            {
                TempData["SuccessMessage"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Đăng nhập thất bại!";
                return View();
            }            
        }
        [HttpPost]
        public async Task<ActionResult> Register(UserRegisterModelView model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu form không hợp lệ, trả về view để hiển thị lỗi
                return View(model);
            }

            try
            {
                var result = await _apiService.RegisterAsync(model);
                if (result)
                {
                    return RedirectToAction("Login", "Auth");
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi
                ViewBag.ErrorMessage = ex.Message;
            }

            return View(model);
        }



    }
}