using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
using PagedList;

namespace VanPhongPham.Areas.Admin.Controllers
{
    public class UserOrderController : Controller
    {
        private readonly DB_VanPhongPhamDataContext db = new DB_VanPhongPhamDataContext();
        // GET: Admin/UserOrder
        public ActionResult Index(int? page, string search_str)
        {
            var orders = db.orders.ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            
            if (search_str != null)
            {
                ViewBag.search_str = search_str;
            }
            else
            {
            }
            return View(orders.ToPagedList(pageNumber, pageSize));
        }
    }
}