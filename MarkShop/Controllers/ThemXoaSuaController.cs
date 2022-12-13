using MarkShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MarkShop.Controllers
{
    public class ThemXoaSuaController : Controller
    {
        QLBanQuanAoDataContext db = new QLBanQuanAoDataContext();

        [HttpGet]
        public ActionResult ThemSanPham()
        {
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoaiSP");
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.MaNCC), "MaNCC", "TenNCC");
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ThemSanPham(SanPham sp, HttpPostedFileBase fUpload)                                
        {
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoaiSP");
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.MaNCC), "MaNCC", "TenNCC");
            if (fUpload != null) 
            {
                if (fUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fUpload.FileName); 
                    var path = Path.Combine(Server.MapPath("/Images"), fileName); 
                    if (System.IO.File.Exists(path)) 
                    {
                        TempData["UploadFail"] = "Hình ảnh này đã tồn tại!";
                        return View();
                    }
                    else 
                    {
                        fUpload.SaveAs(path); 
                        sp.Anh = fUpload.FileName; 
                    }
                }
            }
            else
            { 
                TempData["UploadFail"] = "Vui lòng chọn hình ảnh!";
                return View();
            }
            db.SanPhams.InsertOnSubmit(sp);
            db.SubmitChanges();
            ModelState.Clear();
            TempData["Added"] = "Thêm sản phẩm thành công";
            return RedirectToAction("DanhMucCacSanPham", "Admin");
        }

        [HttpGet]
        public ActionResult SuaSanPham(int id)
        {
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoaiSP");
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.MaNCC), "MaNCC", "TenNCC");
            SanPham product = db.SanPhams.SingleOrDefault(n => n.MaSP.Equals(id));
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
       
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SuaSanPham(SanPham sp, HttpPostedFileBase fUpload)
        {
            SanPham product = db.SanPhams.SingleOrDefault(n => n.MaSP.Equals(sp.MaSP));
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoaiSP");
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.MaNCC), "MaNCC", "TenNCC");
            if (product == null)
            {
                return HttpNotFound();
            }
            product.TenSP = sp.TenSP;
            product.MoTa = sp.MoTa;
            product.GioiTinh = sp.GioiTinh;
            product.GiaBan = sp.GiaBan;
            product.GiaNhap = sp.GiaNhap;
            if (fUpload != null)
            {
                if (fUpload.ContentLength > 0)
                {
                    var filename = Path.GetFileName(fUpload.FileName);
                    var path = Path.Combine(Server.MapPath("/Images"), filename);
                    fUpload.SaveAs(path);
                    product.Anh = fUpload.FileName;
                }
            }
            else
            {
                TempData["UploadFail"] = "Vui lòng chọn hình ảnh!";
                return View();
            }
            product.MaLoaiSP = sp.MaLoaiSP;
            product.MaNCC = sp.MaNCC;
            product.SoLuongTon = sp.SoLuongTon;
            db.SubmitChanges();
            TempData["Edited"] = "Sửa thông tin sản phẩm thành công";
            return RedirectToAction("DanhMucCacSanPham", "Admin");
        }

        public ActionResult XoaSanPham(int maSP)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }
            SanPham sanPham = db.SanPhams.Single(ma => ma.MaSP == maSP);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            db.SanPhams.DeleteOnSubmit(sanPham);
            db.SubmitChanges();
            return RedirectToAction("DanhMucCacSanPham", "Admin");
        }

        public ActionResult ChiTietSanPham(int maSP)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }
            SanPham sanPham = db.SanPhams.Single(ma => ma.MaSP == maSP);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(sanPham);
            }
        }

        public ActionResult timKiemSanPham(string tenSP)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }
            if (!string.IsNullOrEmpty(tenSP))
            {
                var query = from sp in db.SanPhams where sp.TenSP.Contains(tenSP) || sp.LoaiSanPham.TenLoaiSP.Contains(tenSP) || sp.NhaCungCap.TenNCC.Contains(tenSP) select sp;
                if (query.Count() == 0)
                {
                    return RedirectToAction("thongBaoRong", "ThemXoaSua");
                }
                return View(query);
            }
            return View();
        }

        public ActionResult thongBaoRong()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }
            ViewBag.stringEmpty = "Không tìm thấy sản phẩm";
            return View();
        }

        private decimal TongDoanhThu() {
            decimal TongDoanhThu = 0;
            var check = db.ChiTietHoaDons.Count();
            if (check == 0)
            {
                return TongDoanhThu;
            }
            var cthh = db.ChiTietHoaDons.ToList();
            foreach (var item in cthh)
            {
                TongDoanhThu += (decimal)(item.SoLuong * item.DonGia);
            }
            return TongDoanhThu;
        }

        public ActionResult QuanLiDonHang()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("DangNhap", "User");
            }
            var loadData = db.ChiTietHoaDons;
            var check = db.ChiTietHoaDons.Count();
            ViewBag.TongDoanhThu = TongDoanhThu();
            return View(loadData);
        }
        [HttpPost]
        public ActionResult DuyetDonHang(int maHD)
        {
            HoaDon hd = db.HoaDons.SingleOrDefault(n => n.MaHD.Equals(maHD));
            hd.TinhTrang = true;
            db.SubmitChanges();
            return RedirectToAction("QuanLiDonHang", "ThemXoaSua");
        }

        [HttpPost]
        public ActionResult HuyDH(int maHD)
        {
            HoaDon hd = db.HoaDons.SingleOrDefault(n => n.MaHD.Equals(maHD));
            db.HoaDons.DeleteOnSubmit(hd);
            db.SubmitChanges();
            return RedirectToAction("QuanLiDonHang", "ThemXoaSua");
        }

        public ActionResult QuanLiKhachHang()
        {
            ViewBag.GetList = from a in db.HoaDons
                              join b in db.KhachHangs
                              on a.MaKH equals b.MaKH
                              select new HDKhachHangModel
                              {
                                  MaKH = b.MaKH,
                                  TenKH = b.TenKH,
                                  TaiKhoan = b.TaiKhoan,
                                  MatKhau = b.MatKhau,
                                  SoDienThoai = b.SDT,
                                  MaHD = a.MaHD,
                                  TinhTrang = (bool)a.TinhTrang,
                              };
            return View(ViewBag.GetList);
        }
        [HttpPost]
        public ActionResult XoaTaiKhoan(int maKH)
        {
            KhachHang kh = db.KhachHangs.SingleOrDefault(n => n.MaKH.Equals(maKH));
            db.KhachHangs.DeleteOnSubmit(kh);
            db.SubmitChanges();
            return RedirectToAction("QuanLiKhachHang", "ThemXoaSua");
        }

        public ActionResult listLoai()
        {
            var all_sanpham = from lsp in db.LoaiSanPhams select lsp;
            return View(all_sanpham);
        }

        public ActionResult CreateLoai()
        {
            //SetViewBag();
            return View();
        }
        [HttpPost]
        public ActionResult CreateLoai(System.Web.Mvc.FormCollection collection)
        {
            var E_tenloai = collection["TenLoaiSP"];

            if (string.IsNullOrEmpty(E_tenloai))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                LoaiSanPham sp = new LoaiSanPham();
                sp.TenLoaiSP = E_tenloai;

                db.LoaiSanPhams.InsertOnSubmit(sp);
                db.SubmitChanges();
                return RedirectToAction("listLoai");
            }
            //SetViewBag();
            return this.CreateLoai();
        }

        public ActionResult EditLoai(int id)
        {
            var E_sanpham = db.LoaiSanPhams.First(m => m.MaLoaiSP == id);
            //SetViewBag(id);
            return View(E_sanpham);
        }
        [HttpPost]
        public ActionResult EditLoai(int id, System.Web.Mvc.FormCollection collection)
        {
            var E_sanpham = db.LoaiSanPhams.First(m => m.MaLoaiSP == id);

            var E_tensanpham = collection["TenLoaiSP"];

            if (string.IsNullOrEmpty(E_tensanpham))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_sanpham.TenLoaiSP = E_tensanpham;

                UpdateModel(E_sanpham);
                db.SubmitChanges();
                return RedirectToAction("listLoai");
            }
            //SetViewBag(id);
            return this.SuaSanPham(id);
        }

        public ActionResult DeleteLoai(int id)
        {
            var D_sp = db.LoaiSanPhams.First(m => m.MaLoaiSP == id);
            return View(D_sp);
        }
        [HttpPost]
        public ActionResult DeleteLoai(int id, FormCollection collection)
        {
            var D_sp = db.LoaiSanPhams.Where(m => m.MaLoaiSP == id).First();
            db.LoaiSanPhams.DeleteOnSubmit(D_sp);
            db.SubmitChanges();
            return RedirectToAction("listLoai");
        }


        public ActionResult listTH()
        {
            var all_sanpham = from lth in db.NhaCungCaps select lth;
            return View(all_sanpham);
        }

        public ActionResult CreateTH()
        {
            //SetViewBag();
            return View();
        }
        [HttpPost]
        public ActionResult CreateTH(System.Web.Mvc.FormCollection collection)
        {
            var E_tenloai = collection["TenNCC"];

            if (string.IsNullOrEmpty(E_tenloai))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                NhaCungCap sp = new NhaCungCap();
                sp.TenNCC = E_tenloai;

                db.NhaCungCaps.InsertOnSubmit(sp);
                db.SubmitChanges();
                return RedirectToAction("listTH");
            }
            //SetViewBag();
            return this.CreateTH();
        }

        public ActionResult EditTH(int id)
        {
            var E_sanpham = db.NhaCungCaps.First(m => m.MaNCC == id);
            //SetViewBag(id);
            return View(E_sanpham);
        }

        [HttpPost]
        public ActionResult EditTH(int id, System.Web.Mvc.FormCollection collection)
        {
            var E_sanpham = db.NhaCungCaps.First(m => m.MaNCC == id);

            var E_tensanpham = collection["TenNCC"];

            if (string.IsNullOrEmpty(E_tensanpham))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_sanpham.TenNCC = E_tensanpham;

                UpdateModel(E_sanpham);
                db.SubmitChanges();
                return RedirectToAction("listTH");
            }
            //SetViewBag(id);
            return this.SuaSanPham(id);
        }

        public ActionResult DeleteTH(int id)
        {
            var D_sp = db.NhaCungCaps.First(m => m.MaNCC == id);
            return View(D_sp);
        }

        [HttpPost]
        public ActionResult DeleteTH(int id, FormCollection collection)
        {
            var D_sp = db.NhaCungCaps.Where(m => m.MaNCC == id).First();
            db.NhaCungCaps.DeleteOnSubmit(D_sp);
            db.SubmitChanges();
            return RedirectToAction("listTH");
        }

        public void ExportExcel_EmployeeData()
        {
            double thanhtien;
            var sb = new StringBuilder();
            var data = from s1 in db.ChiTietHoaDons
                        select new
                        {
                            s1.HoaDon.MaHD,
                            s1.HoaDon.MaKH,
                            s1.HoaDon.NgayDat,
                            s1.HoaDon.NgayGiao,
                            s1.HoaDon.TinhTrang,
                            s1.DonGia,
                            s1.SoLuong,
                            thanhtien = s1.DonGia * s1.SoLuong
                        }; 
           
            var list = data.ToList();
            var grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = list;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=Emp.xls");
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }   
    }
}