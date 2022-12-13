using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarkShop.Controllers
{
    public class ThongKeController : Controller
    {
        QLBanQuanAoDataContext db = new QLBanQuanAoDataContext();
        // GET: ThongKe
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SanPhamTon()
        {
            List<SanPham> listsp = (from s in db.SanPhams where (s.SoLuongTon > 0) select s).ToList();
            return View(listsp);
        }

        public ActionResult DoanhThuThangNam(string NgayBD, string NgayKT)
        {
            List<LoaiSanPham> listlsp = (from s in db.LoaiSanPhams   select s).ToList();
            DateTime bd = Convert.ToDateTime(NgayBD);
            DateTime kt = Convert.ToDateTime(NgayKT);
            //NgayBD = "01/06/2022";
            //NgayKT = "30/06/2022";
            if (NgayBD == null && NgayKT == null)
            {
                NgayBD = "01/01/2022";
                NgayKT = "31/12/2022";
            }
            decimal TongDoanhThu = 0;
            decimal TDTNam = 0;
            decimal TDTNu = 0;
            decimal TDTTat = 0;
            int SoLuong = 0;
            int SoLuongDonGiayNam = 0;
            int SoLuongDonGiayNu = 0;
            int SoLuongDonTat = 0;
            if (!string.IsNullOrEmpty(NgayBD) && !string.IsNullOrEmpty(NgayKT))
            {
                List<HoaDon> listhd = (from s in db.HoaDons where (s.NgayDat >= bd && s.NgayDat <= kt) select s).ToList();
                foreach (var item in listhd)
                {
                    foreach (var items in item.ChiTietHoaDons)
                    {
                        if (items.SanPham.MaLoaiSP == 1)
                        {
                            SoLuongDonGiayNam += 1;
                            TDTNam += (decimal)(items.SoLuong * items.DonGia);
                        }
                        else if (items.SanPham.MaLoaiSP == 2)
                        {
                            SoLuongDonGiayNu += 1;
                            TDTNu += (decimal)(items.SoLuong * items.DonGia);
                        }
                        else
                        {
                            SoLuongDonTat += 1;
                            TDTTat += (decimal)(items.SoLuong * items.DonGia);
                        }
                        SoLuong += (int)items.SoLuong;
                        TongDoanhThu += (decimal)(items.SoLuong * items.DonGia);
                    }
                }
                ViewBag.SLDGiayNam = SoLuongDonGiayNam;
                ViewBag.SLDGiayNu = SoLuongDonGiayNu;
                ViewBag.SLDTat = SoLuongDonTat;
                ViewBag.TongDoanhThu = TongDoanhThu;
                ViewBag.TDTNam = TDTNam;
                ViewBag.TDTNu = TDTNu;
                ViewBag.TDTTat = TDTTat;
                ViewBag.SoLuong = SoLuong;
                return View(listlsp);
            }
            return View();
        }
        private decimal ThongKeTongDoanhThu()
        {
            decimal TongDoanhThu = db.ChiTietHoaDons.Sum(n => n.SoLuong * n.DonGia).Value;
            return TongDoanhThu;
        }
        private double ThongKeDonHang()
        {
            double slhd = db.HoaDons.Count();
            return slhd;
        }
        private double ThongKeKhachHang()
        {
            double slkh = db.KhachHangs.Count();
            return slkh;
        }
        private decimal ThongKeDoanhThuThang(int thang, int nam)
        {
            var lstHD = db.HoaDons.Where(n => n.NgayDat.Value.Month == thang && n.NgayDat.Value.Year == nam);
            decimal TongTien = 0;
            foreach (var item in lstHD)
            {
                TongTien = decimal.Parse(item.ChiTietHoaDons.Sum(n => n.SoLuong * n.DonGia).Value.ToString());
            }
            return TongTien;
        }
        private int TongSL()
        {
            int TongSL = 0;
            var check = db.ChiTietHoaDons.Count();
            if (check == 0)
            {
                return TongSL;
            }
            var cthh = db.ChiTietHoaDons.ToList();
            foreach (var item in cthh)
            {
                TongSL += (int)(item.SoLuong);
            }
            return TongSL;
        }
        private decimal tongnhaphang()
        {
            decimal tongdoanhthu = 0;
            var check = db.ChiTietHoaDons.Count();
            if (check == 0)
            {
                return tongdoanhthu;
            }
            var sp = db.ChiTietHoaDons.ToList();
            foreach (var item in sp)
            {        
                tongdoanhthu += (decimal)(item.SanPham.GiaNhap);            
            }
            return tongdoanhthu;
        }


        //private decimal TongNhapHangS()
        //{
        //    decimal TongDoanhThu = 0;
        //    var check = db.SanPhams.Count();
        //    if (check == 0)
        //    {
        //        return TongDoanhThu;
        //    }
        //    var cthh = db.ChiTietHoaDons.ToList();
        //    var sp = db.SanPhams.ToList();
        //    foreach (var item in sp)
        //    {
        //        foreach (var itemS in cthh)
        //        {
        //            TongDoanhThu = ((decimal)(item.GiaBan)*(int)(itemS.SoLuong) - (decimal)(item.GiaNhap) * (int)(itemS.SoLuong));
        //        }
        //    }
        //    return TongDoanhThu;
        //}


        public ActionResult LoiNhuan()
        {
            ViewBag.SoNgTruyCap = HttpContext.Application["SoNgTruyCap"].ToString();
            ViewBag.SoNgDangTruyCap = HttpContext.Application["SoNgDangTruyCap"].ToString();
            ViewBag.TongDoanhThu = ThongKeTongDoanhThu();
            ViewBag.SLDH = ThongKeDonHang();
            ViewBag.SLKH = ThongKeKhachHang();
            ViewBag.TongSL = TongSL();
            ViewBag.TongDoanhThu = ThongKeTongDoanhThu();
            ViewBag.TongNhapHang = tongnhaphang();
            
            return View();
        }
    }
}