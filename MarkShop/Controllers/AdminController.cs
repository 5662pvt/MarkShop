using MarkShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarkShop.Controllers
{
    public class AdminController : Controller
    {
        //test
        // GET: Admin
        QLBanQuanAoDataContext db = new QLBanQuanAoDataContext();
        public ActionResult LayoutAdmin()
        {
            return View();
        }

        public ActionResult TrangPhucNam()
        {
            if (Session["Admin"] == null)                                                        //vùng nhớ mà server cấp phát cho user để lưu dữ liệu riêng của mình   
            {
                return RedirectToAction("DangNhap", "DangNhap");                                 //chuyển hướng sang trang đăng nhập
            }
            var listTrangPhucNam = db.SanPhams.OrderBy(sp => sp.MaSP).ToList();
            return View(listTrangPhucNam);
        }

        public ActionResult TrangPhucNu()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }
            var listTrangPhucNu = db.SanPhams.OrderBy(sp => sp.MaSP).ToList();
            return View(listTrangPhucNu);
        }

        public ActionResult DanhMucCacSanPham(int page = 1, int pageSize = 12)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }
            var sanPham = new Product();
            var model = sanPham.ListAll(page, pageSize);
            return View(model);
        }

        public ActionResult DangXuat()
        {
            Session["Admin"] = null;
            return RedirectToAction("Home", "Home");
        }

    }
}