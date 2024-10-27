using VanPhongPham.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VanPhongPham.Controllers
{
    public class AddressController : Controller
    {
        DB_VanPhongPhamDataContext db = new DB_VanPhongPhamDataContext();
        // GET: Address
        [HttpPost]
        public ActionResult SaveUpdateUserAddress(address useraddress, int address_id)
        {
            try
            {
                var checkadrs = db.addresses.FirstOrDefault(u => u.user_id == useraddress.user_id && u.address_id == address_id);

                // Nếu là địa chỉ mặc định, cập nhật các địa chỉ khác
                if (useraddress.isDefault == true)
                {
                    var lstadrs = db.addresses.Where(u => u.user_id == useraddress.user_id).ToList();
                    foreach (var adrs in lstadrs)
                    {
                        if (adrs.address_id != useraddress.address_id)
                        {
                            adrs.isDefault = false;
                        }
                    }
                }

                if (checkadrs == null)
                {
                    db.addresses.InsertOnSubmit(useraddress);
                }
                else
                {
                    checkadrs.full_name = useraddress.full_name;
                    checkadrs.phone_number = useraddress.phone_number;
                    checkadrs.address_line = useraddress.address_line;
                    checkadrs.ward = useraddress.ward;
                    checkadrs.district = useraddress.district;
                    checkadrs.province = useraddress.province;
                    checkadrs.isDefault = useraddress.isDefault;
                }

                db.SubmitChanges();

                return Json(new { success = true, message = "Dữ liệu đã được lưu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult SaveSetDefault(int address_id)
        {
            try
            {
                var address = db.addresses.FirstOrDefault(u => u.address_id == address_id);
                if (address.isDefault == false)
                {
                    var lstadrs = db.addresses.Where(u => u.user_id == address.user_id).ToList();

                    foreach (var adrs in lstadrs)
                    {
                        if (adrs.address_id != address.address_id)
                        {
                            adrs.isDefault = false;
                        }
                    }

                    address.isDefault = true;
                    db.SubmitChanges();
                }
                return Json(new { success = true, message = "Dữ liệu đã được lưu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult DeleteUserAddress(int address_id)
        {
            try
            {
                var address = db.addresses.FirstOrDefault(u => u.address_id == address_id);
                if (address != null)
                {
                    db.addresses.DeleteOnSubmit(address);
                    db.SubmitChanges();
                }
                return Json(new { success = true, message = "Xóa dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}