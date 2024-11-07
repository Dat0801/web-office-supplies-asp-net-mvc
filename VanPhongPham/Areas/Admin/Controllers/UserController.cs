using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Net;
namespace VanPhongPham.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        // GET: Admin/User
        private readonly UserRepository userRepository;
        private readonly Account _cloudinaryAccount = new Account("dgvcrawly", "173992459599594", "F0N2oghE7dRuopEEVqmtRUf9mIQ");
        private readonly Cloudinary _cloudinary;
        public UserController()
        {
            userRepository = new UserRepository();
            _cloudinary = new Cloudinary(_cloudinaryAccount);
        }
        public ActionResult Index(string search_str, string user_id)
        {
            List<user> listSup;
            if (search_str != null)
            {
                listSup = userRepository.SearchUser(search_str);
                ViewBag.searchStr = search_str;
            }
            else
            {
                listSup = userRepository.GetAllUsers();
            }
            ViewBag.user_id = userRepository.GenerateUserId();
            user supp = userRepository.GetUserById(user_id);
            ViewBag.user = supp;
            return View(listSup);
        }
        
        public ActionResult RecoverUser(string search_str)
        {
            List<user> sup;
            if (search_str != null)
            {
                sup = userRepository.SearchDeletedUser(search_str);
                ViewBag.searchStr = search_str;
            }
            else
                sup = userRepository.GetDeletedUsers();
            return View(sup);
        }
        [HttpGet]
        public ActionResult RecoverSingleUser(string user_id)
        {
            if (!string.IsNullOrEmpty(user_id))
            {
                userRepository.RecoverUsers(new List<string> { user_id });
            }
            return RedirectToAction("Recoveruser", "user", new { area = "Admin" });
        }
        [HttpPost]
        public ActionResult RecoverUser(List<string> selectedUsers)
        {
            if (selectedUsers != null && selectedUsers.Any())
            {
                userRepository.RecoverUsers(selectedUsers);
            }
            return RedirectToAction("Recoveruser", "user", new { area = "Admin" });
        }
        [HttpPost]
        public ActionResult ManageUser(string action, UserViewModel model)
        {            
            if (model.AvatarFile != null && model.AvatarFile.ContentLength > 0)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(model.AvatarFile.FileName, model.AvatarFile.InputStream),
                    Folder = "user_avatars"
                };
                var uploadResult = _cloudinary.Upload(uploadParams);

                if (uploadResult.StatusCode == HttpStatusCode.OK)
                {
                    model.AvatarUrl = uploadResult.SecureUrl.ToString();
                }
            }
            else
            {
                user user = userRepository.GetUserById(model.UserId);
                if (user != null)
                {
                    model.AvatarUrl = user.avt_url;
                }
                else
                {
                    model.AvatarUrl = "https://res.cloudinary.com/dgvcrawly/image/upload/v1730879614/user_avatars/ucfbpazojzj4kaluz3oo.jpg";
                }
            }

            user newUser = new user
            {
                user_id = model.UserId,
                full_name = model.FullName,
                username = model.Username,
                password = model.Password,
                email = model.Email,
                gender = model.Gender,
                dob = model.Dob,
                avt_url = model.AvatarUrl,
            };
            if (action == "add")
            {
                bool rs = userRepository.AddUser(newUser);
                if (!rs)
                {
                    TempData["Msg"] = "Người dùng đã tồn tại";
                }
                else
                { 
                    TempData["Msg"] = "Thêm người dùng thành công";
                }
            }
            if (action == "edit")
            {
                bool rs = userRepository.UpdateUser(newUser);
                if(!rs)
                {
                    TempData["Msg"] = "Người dùng không tồn tại";
                }
                else
                {
                    TempData["Msg"] = "Cập nhật người dùng thành công";
                }
            }
            return RedirectToAction("Index", "user", new { area = "Admin" });
        }
        [HttpGet]
        public ActionResult DeleteUser(string user_id)
        {
            if (!string.IsNullOrEmpty(user_id))
            {
                userRepository.DeleteUsers(new List<string> { user_id });
            }
            return RedirectToAction("Index", "user", new { area = "Admin" });
        }
        [HttpPost]
        public ActionResult DeleteUser(List<string> selectedUsers)
        {
            if (selectedUsers != null && selectedUsers.Any())
            {
                userRepository.DeleteUsers(selectedUsers);
            }
            return RedirectToAction("Index", "user", new { area = "Admin" });
        }
    }
}