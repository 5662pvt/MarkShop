using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using MarkShop.Models;

namespace MarkShop.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: SanPham
        QLBanQuanAoDataContext db = new QLBanQuanAoDataContext();
        public ActionResult SanPham()
        {
            var listSanPham = db.SanPhams.OrderBy(sp => sp.TenSP).ToList();
            return View(listSanPham);
        }

        public ActionResult SanPhamPartial(int page = 1, int pageSize = 12)
        {
            ViewBag.loaiSP = db.LoaiSanPhams.ToList();
            var dsSanPham = new Product();
            var model = dsSanPham.ListAll(page, pageSize);
            return View(model);
        }

        public ActionResult SanPhamTheoLoai(int maLoaiSP)
        {
            ViewBag.loaiSP = db.LoaiSanPhams.OrderBy(sp => sp.MaLoaiSP);
            var dsSPTheoLoai = db.SanPhams.Where(sp => sp.MaLoaiSP == maLoaiSP).OrderBy(sp => sp.GiaBan).ToList();
            if (dsSPTheoLoai.Count == 0)
            {
                ViewBag.thongBao = "Sản phẩm đã hết. Xin quý khách thông cảm";
            }
            return View(dsSPTheoLoai);
        }

        public ActionResult SanPhamTuongTu()
        {
            var listSanPham = db.SanPhams.OrderBy(sp => sp.TenSP).ToList();
            return View(listSanPham);
        }

        public ActionResult XemChiTiet(int masp)
        {
            ViewBag.loaiSP = db.LoaiSanPhams.OrderBy(sp => sp.MaLoaiSP);
            SanPham sanPham = db.SanPhams.Single(s => s.MaSP == masp);

            //comment
            if ((KhachHang)Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap", "User");
            }
            var sessionUser = (KhachHang)Session["TaiKhoan"];
            ViewBag.MAKH = sessionUser.MaKH;
            ViewBag.ListComment = new CommentsController().ListCommentViewModel(0, masp);

            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }

        public ActionResult timKiemSanPham(string tenSP)
        {
            ViewBag.loaiSP = db.LoaiSanPhams.OrderBy(sp => sp.MaLoaiSP);
            if (!string.IsNullOrEmpty(tenSP))
            {
                var query = from sp in db.SanPhams where sp.TenSP.Contains(tenSP) || sp.LoaiSanPham.TenLoaiSP.Contains(tenSP) || sp.NhaCungCap.TenNCC.Contains(tenSP) select sp;
                if (query.Count() == 0)
                {
                    return RedirectToAction("thongBaoRong", "SanPham");
                }
                return View(query);
            }
            return View();
        }

        public ActionResult thongBaoRong()
        {
            ViewBag.loaiSP = db.LoaiSanPhams.OrderBy(sp => sp.MaLoaiSP);
            ViewBag.stringEmpty = "Không tìm thấy sản phẩm";
            return View();
        }

        //comment
        [ChildActionOnly]
        public ActionResult _ChildComment(int parentid, int productid)
        {
            var data = new CommentsController().ListCommentViewModel(parentid, productid);
            var sessionuser = (KhachHang)Session["TaiKhoan"];
            for (int k = 0; k < data.Count; k++)
            {
                data[k].MaKH = sessionuser.MaKH;
            }
            return PartialView("~/Views/Shared/_ChildComment.cshtml", data);
        }

        [HttpPost]
        public JsonResult AddNewComment(int productid, int userid, int parentid, string commentmsg, string rate)
        {
            try
            {
                var dao = new CommentsController();
                tbComMent comment = new tbComMent();

                comment.ComentMsg = commentmsg;
                comment.MaSP = productid;
                comment.MaKH = userid;
                comment.ParentID = parentid;
                comment.Rate = Convert.ToInt16(rate);
                comment.ComentDate = DateTime.Now;

                bool addcomment = dao.Insert(comment);
                if (addcomment == true)
                {
                    return Json(new{ status = true});
                }
                else
                {
                    return Json(new{status = false});
                }
            }
            catch
            {
                return Json(new{status = false});
            }
        }

        public ActionResult GetComment(int productid)
        {
            var data = new CommentsController().ListCommentViewModel(0, productid);
            return PartialView("~/Views/Shared/_ChildComment.cshtml", data);

        }
    }
}