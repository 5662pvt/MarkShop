using MarkShop.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarkShop.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        // Tạo đối tượng db chứa dữ liệu từ Model QLBanQuanAo
        QLBanQuanAoDataContext db = new QLBanQuanAoDataContext();
        public List<GioHang> LayGioHang()
        {
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang == null)
            {
                // Nếu listGioHang chưa tồn tại thì khởi tạo
                listGioHang = new List<GioHang>();
                Session["GioHang"] = listGioHang;
            }
            return listGioHang;
        }

        // Xây dựng phương thức thêm vào giỏ hàng
        public ActionResult ThemGioHang(int msp, string strURL) 
        {
            // Kiểm tra sản phẩm này trong database đã tồn tại chưa (tránh trường hợp user tự get URL). Nếu k tồn tại => Trang 404
            SanPham product = db.SanPhams.SingleOrDefault(pd => pd.MaSP.Equals(msp));
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            // Lấy ra số lượng tồn hiện có hiển thị ra view HetHang
            Session["SoLuongTonHienCo"] = product.SoLuongTon;
            // Lấy ra giỏ hàng
            List<GioHang> listGioHang = LayGioHang();
            // Kiểm tra sản phẩm này có tồn tại trong Session["GioHang"] hay chưa ?
            GioHang item = listGioHang.Find(sp => sp.maSP == msp);
            if (item != null)
            { // Đã có sản phẩm này trong giỏ 
                Session["TenSP"] = item.tenSP;
                item.soLuong++;
                // Kiểm tra tiếp xem số lượng tồn sản phẩm trong database có nhỏ hơn số lượng sản phẩm thêm hay k. 
                // Nếu nhỏ hơn thì báo hết hàng
                if (product.SoLuongTon < item.soLuong)
                {
                    item.soLuong = 1;
                    TempData["ErrorMessage"] = string.Format("Sản phẩm {0} chỉ còn {1} sản phẩm", Session["TenSP"].ToString(), Session["SoLuongTonHienCo"].ToString());
                }
                return Redirect(strURL);
            }
            item = new GioHang(msp);
            listGioHang.Add(item);
            return Redirect(strURL);
        }
        // Xây dựng phương thức tính tổng số lượng
        private int TongSoLuong()
        {
            int tongSoLuong = 0;
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang != null)
            {
                tongSoLuong = listGioHang.Sum(sp => sp.soLuong);
                Session.Add("TongSoLuong", tongSoLuong);
            }
            return tongSoLuong;
        }

        // Xây dựng phương thức tính tổng thành tiền
        private double TongThanhTien()
        {
            double tongThanhTien = 0;
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang != null)
            {
                tongThanhTien += listGioHang.Sum(sp => sp.thanhTien);
            }
            return tongThanhTien;
        }

        // Xây dựng trang giỏ hàng

        public ActionResult GioHang()
        {
            ViewBag.loaiSP = db.LoaiSanPhams.OrderBy(sp => sp.MaLoaiSP);
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("GioHangTrong", "GioHang");
            }
            List<GioHang> listGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongThanhTien = TongThanhTien();
            return View(listGioHang);
        }

        public ActionResult GioHangPartial()
        {
            List<GioHang> listGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            return PartialView();
        }

        public ActionResult CapNhatGioHang(int maSP, FormCollection f)
        {
            // Kiểm tra sản phẩm này trong database đã tồn tại chưa (tránh trường hợp user tự get URL). Nếu k tồn tại => Trang 404
            SanPham product = db.SanPhams.SingleOrDefault(pd => pd.MaSP.Equals(maSP));
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            // Lấy ra số lượng tồn hiện có hiển thị ra view HetHang
            Session["SoLuongTonHienCo"] = product.SoLuongTon;
            List<GioHang> listGH = LayGioHang();
            GioHang sp = listGH.Single(s => s.maSP == maSP);
            Session["TenSP"] = sp.tenSP;
            if (sp != null)
            {
                sp.soLuong = int.Parse(f["txtSoLuong"].ToString());
                if (product.SoLuongTon < sp.soLuong)
                {
                    sp.soLuong = 1;
                    TempData["ErrorMessage"] = string.Format("Sản phẩm {0} chỉ còn {1} sản phẩm", Session["TenSP"].ToString(), Session["SoLuongTonHienCo"].ToString());
                }
            }
            return RedirectToAction("SanPhamPartial", "SanPham");
        }

        public ActionResult XoaGioHang(int maSP)
        {
            List<GioHang> listGH = LayGioHang();
            GioHang sp = listGH.Single(s => s.maSP == maSP);
            if (sp != null)
            {
                listGH.RemoveAll(s => s.maSP == maSP);
                return RedirectToAction("SanPhamPartial", "SanPham");
            }
            if (listGH.Count == 0)
            {
                return RedirectToAction("SanPhamPartial", "SanPham");
            }
            return RedirectToAction("SanPhamPartial", "SanPham");
        }

        public ActionResult XoaGioHangAll()
        {
            List<GioHang> listGH = LayGioHang();
            listGH.Clear();
            if (listGH.Count == 0)
            {
                return RedirectToAction("SanPhamPartial", "SanPham");
            }
            return RedirectToAction("SanPhamPartial", "SanPham");
        }

        public ActionResult GioHangTrong()
        {
            ViewBag.thongBao = "Your cart is empty";
            return View();
        }

        public ActionResult ViewGioHangHover()
        {
            List<GioHang> listGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.thongBao = "Your cart is empty";
            ViewBag.TongThanhTien = TongThanhTien();
            return View(listGioHang);
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiểm tra đăng nhập
            if (Session["taikhoan"] == null || Session["taikhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "User");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("SanPhamPartial", "SanPham");
            }

            // Lấy giỏ hàng từ Session
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            if (ViewBag.TongSoLuong == 0)
            {
                return RedirectToAction("SanPhamPartial", "SanPham");
            }
            ViewBag.TongThanhTien = TongThanhTien();
            return View(lstGioHang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            // Thêm đơn hàng
            HoaDon ddh = new HoaDon();
            KhachHang kh = (KhachHang)Session["taikhoan"];
            List<GioHang> gh = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            var NgayGiao = String.Format("{0:mm/dd/yyyy}", f["NgayGiao"]);
            ddh.NgayGiao = DateTime.Parse(NgayGiao);
            ddh.TinhTrang = false;
            db.HoaDons.InsertOnSubmit(ddh);
            db.SubmitChanges();
            Session.Add("NgayGiao", ddh.NgayGiao);
            Session.Add("MaHD", ddh.MaHD);
            foreach (var item in gh)
            {
                ChiTietHoaDon ctdh = new ChiTietHoaDon();
                ctdh.MaHD = ddh.MaHD;
                ctdh.MaSP = item.maSP;
                ctdh.SoLuong = item.soLuong;
                ctdh.DonGia = (decimal)item.donGia;
                db.ChiTietHoaDons.InsertOnSubmit(ctdh);
                // Lấy mã sản phẩm trong database
                SanPham sp = db.SanPhams.SingleOrDefault(i => i.MaSP == item.maSP);
                // Cập nhật số lượng tồn trong database
                sp.SoLuongTon = sp.SoLuongTon - item.soLuong;
            }
            db.SubmitChanges();
            return RedirectToAction("XacNhanDatHang", "GioHang");
        }

        public ActionResult XacNhanDatHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongThanhTien = TongThanhTien();
            Session["GioHang"] = null;                  
            return View(lstGioHang);
        }

        public ActionResult XacNhanDatHangMOMO()
        {
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongThanhTien = TongThanhTien();
            Session["GioHang"] = null;
            return View(lstGioHang);
        }


        public ActionResult Payment()
        {
            HoaDon ddh = new HoaDon();
            KhachHang kh = (KhachHang)Session["taikhoan"];
            List<GioHang> gh = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            //ddh.NgayGiao = DateTime.Parse("29/06/2022");
            ddh.TinhTrang = true;
            db.HoaDons.InsertOnSubmit(ddh);
            db.SubmitChanges();
            Session.Add("NgayGiao", ddh.NgayGiao);
            Session.Add("MaHD", ddh.MaHD);
            foreach (var item in gh)
            {
                ChiTietHoaDon ctdh = new ChiTietHoaDon();
                ctdh.MaHD = ddh.MaHD;
                ctdh.MaSP = item.maSP;
                ctdh.SoLuong = item.soLuong;
                ctdh.DonGia = (decimal)item.donGia;
                db.ChiTietHoaDons.InsertOnSubmit(ctdh);
                // Lấy mã sản phẩm trong database
                SanPham sp = db.SanPhams.SingleOrDefault(i => i.MaSP == item.maSP);
                // Cập nhật số lượng tồn trong database
                sp.SoLuongTon = sp.SoLuongTon - item.soLuong;
            }
            db.SubmitChanges();
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOTANF20220408";
            string accessKey = "m79Wjcp5ezhTnSDs";
            string serectkey = "yUxFeCKx1aktPc60yqJ2EJ3hWRO4SAC3";
            string orderInfo = "tay";
            string returnUrl = "http://localhost:11173//GioHang/ConfirmPaymentClient";
            string notifyurl = "http://ba1adf48beba.ngrok.io/GioHang/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

            string amount = TongThanhTien().ToString();
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i
        public ActionResult ConfirmPaymentClient()
        {
            Session["NgayGiao"] = "28/06/2022";
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongThanhTien = TongThanhTien();
            string param = Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
            param = Server.UrlDecode(param);
            MoMoSecurity crypto = new MoMoSecurity();
            string serectkey = "yUxFeCKx1aktPc60yqJ2EJ3hWRO4SAC3";
            string signature = crypto.signSHA256(param, serectkey);
            if (signature != Request["signature"].ToString())
            {
                ViewBag.message = "Thông tin Request không hợp lệ";
                return View();
            }
            if (!Request.QueryString["errorCode"].Equals("0"))
            {
                ViewBag.message = "Thanh toán thất bại";
            }
            else
            {
                ViewBag.message = "Thanh toán thành công";
                Session["GioHang"] = new List<GioHang>();
            }
            return View(lstGioHang);
        }

        [HttpPost]
        public void SavePayment()
        {
            //cập nhật dữ liệu vào db
        }
    }
}