using Recom_Pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static Recom_Pharmacy.Models.Payment.VNPayLibary;
using Recom_Pharmacy.Models.Common;
namespace Recom_Pharmacy.Controllers
{
    public class CartController : Controller
    {
        RecomPharmacyEntities db = new RecomPharmacyEntities();
        ThuocViewModel tb_thuoc = new ThuocViewModel();

        public List<CartEntity> GetCart()
        {
            List<CartEntity> lstCart = Session["Cart"] as List<CartEntity>;
            if (lstCart == null)
            {

                lstCart = new List<CartEntity>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
            //int userId = ((KHACHHANG)Session["usr"]).ID;
            //using (var context = new RecomPharmacyEntities())
            //{
            //    return context.Carts
            //        .Where(ci => ci.ID == userId)
            //        .Select(ci => new CartEntity
            //        {
            //            IDThuoc = (int)ci.MATHUOC,
            //            soLuong = (int)ci.SOLUONG,
            //            tenThuoc = context.THUOCs.FirstOrDefault(t => t.ID == ci.MATHUOC).TENTHUOC,
            //            Picture = context.THUOCs.FirstOrDefault(t => t.ID == ci.MATHUOC).ANH,
            //            giaBan = Double.Parse(context.THUOCs.FirstOrDefault(t => t.ID == ci.MATHUOC).GIABAN.ToString()),
            //            maxSoLuong = context.THUOCs.FirstOrDefault(t => t.ID == ci.MATHUOC).SOLUONG
            //        })
            //        .ToList();
            //}
        }

        public ActionResult AddtoCart(int id,int quantity, string loThuoc, string strURL)
        {
            KHACHHANG cus = (KHACHHANG)Session["usr"];
            if (cus != null)
            {
                List<CartEntity> lstcart = GetCart();
                //Kiem tra sách này tồn tại trong Session["Giohang"] chưa?

                CartEntity Product = lstcart.Find(n => n.IDThuoc == id);
                if (Product == null)
                {
                    Product = new CartEntity(id, loThuoc);
                    Product.soLuong = quantity;
                    lstcart.Add(Product);
                    return Redirect(strURL);
                }
                else
                {
                    Product.soLuong += quantity;
                    return Redirect(strURL);
                }
            }
            return RedirectToAction("Login", "Login");
        }
        public ActionResult Cart()
        {
            KHACHHANG cus = (KHACHHANG)Session["usr"];
            if(cus != null)
            {
                List<CartEntity> lstCart = GetCart();
                if (lstCart.Count == 0)
                {
                    return RedirectToAction("gioHangTrong", "Cart");
                }
                var listThuoc = tb_thuoc.getThuocWithTonKho("");
                foreach (var item in lstCart)
                {
                    var thuoc = listThuoc.FirstOrDefault(t => t.ID == item.IDThuoc);
                    if (thuoc != null)
                    {
                        item.maxSoLuong = thuoc.CTTONKHOes.First().SLTON ?? 0;
                    }
                    else
                    {
                        item.maxSoLuong = 0; // Handle the case where the item is not found
                    }
                }

                TempData["TotalQuantity"] = TotalQuantity();
                TempData["ToTalPrice"] = ToTalPrice();
                TempData["AvailablePoints"] = cus.TICHDIEM;
                TempData["PointsUsed"] = 0; // Default points used is 0
                TempData["DiscountFromPoints"] = 0; // Default discount is 0
                TempData["FinalPriceAfterPoints"] = ToTalPrice(); // Default final price is total price

                return View(lstCart);
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        public ActionResult Cart(int pointsToUse)
        {
            List<CartEntity> lstCart = GetCart();
            if (lstCart.Count == 0)
            {
                return RedirectToAction("gioHangTrong", "Cart");
            }
            foreach (var item in lstCart)
            {
                var thuoc = tb_thuoc.findById(item.IDThuoc);
                int soluong = thuoc.CTTONKHOes.FirstOrDefault().SLTON ?? 0;
                if (thuoc != null)
                {
                    item.maxSoLuong = soluong;
                }
                else
                {
                    item.maxSoLuong = 0; // Handle the case where the item is not found
                }
            }

            KHACHHANG cus = (KHACHHANG)Session["usr"];
            TempData["TotalQuantity"] = TotalQuantity();
            TempData["ToTalPrice"] = ToTalPrice();

            // Calculate discount based on points
            double finalPrice = FinalPriceAfterPoints(pointsToUse);

            // Lưu trữ điểm tích lũy của khách hàng
            TempData["AvailablePoints"] = cus.TICHDIEM;

            // Lưu trữ điểm sử dụng
            TempData["PointsUsed"] = pointsToUse;

            // Lưu trữ giá giảm từ điểm
            TempData["DiscountFromPoints"] = ToTalPrice() - finalPrice;

            // Lưu trữ tổng giá sau khi sử dụng điểm
            TempData["FinalPriceAfterPoints"] = finalPrice;
            TempData.Keep();
            return View(lstCart);
        }
        public ActionResult gioHangTrong()
        {
            return View();
        }
        private int TotalQuantity()
        {
            int iTongSoLuong = 0;
            List<CartEntity> lstcart = Session["Cart"] as List<CartEntity>;
            if (lstcart != null)
            {
                iTongSoLuong = lstcart.Sum(n => n.soLuong);
            }
            return iTongSoLuong;
        }

        private Double ToTalPrice()
        {
            double ToTal = 0;
            List<CartEntity> lstCart = Session["Cart"] as List<CartEntity>;
            if (lstCart != null)
            {
                ToTal = lstCart.Sum(n => n.tongGia);
            }
            return ToTal;
        }
        private Double FinalPriceAfterPoints(int pointsToUse)
        {
            double totalPrice = ToTalPrice();
            KHACHHANG cus = (KHACHHANG)Session["usr"];
            int maxPointsToUse = Math.Min((int)pointsToUse, (int)cus.TICHDIEM);
            double discount = maxPointsToUse * 1; // Giả sử mỗi điểm tích lũy giảm 1000 đơn vị tiền tệ
            double finalPrice = totalPrice - discount;

            // Đảm bảo giá trị không âm
            if (finalPrice < 0)
            {
                finalPrice = 0;
            }

            return finalPrice;
        }


        public ActionResult CartPartial()
        {
            ViewBag.TotalQuantity = TotalQuantity();
            ViewBag.TotalPrice = ToTalPrice();
            return PartialView();
        }
        //Cap nhat Giỏ hàng
        public ActionResult EditCart(int id, FormCollection f)
        {
            // Get the current cart
            List<CartEntity> lstGiohang = GetCart();
            try
            {
                // Log form values for debugging
                foreach (var key in f.AllKeys)
                {
                    System.Diagnostics.Debug.WriteLine($"Key: {key}, Value: {f[key]}");
                }


                // Find the cart item
                CartEntity item = lstGiohang.SingleOrDefault(n => n.IDThuoc == id);

                if (item != null)
                {
                    // Get the requested quantity from the form
                    if (int.TryParse(f["txtSoluong"], out int requestedQuantity))
                    {
                        // Retrieve the current available stock for the item
                        var thuoc = tb_thuoc.findById(id);
                        if (thuoc == null)
                        {
                            return HttpNotFound();
                        }

                        // Check if the requested quantity exceeds the available stock
                        if (requestedQuantity > thuoc.CTTONKHOes.FirstOrDefault().SLTON)
                        {
                            ModelState.AddModelError("", "Số lượng đặt hàng không được vượt quá số lượng hiện có.");
                            ViewBag.ErrorMessage = "Số lượng đặt hàng không được vượt quá số lượng hiện có.";

                            return View("Cart", lstGiohang);
                        }

                        item.soLuong = requestedQuantity;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Số lượng không hợp lệ.");
                        ViewBag.ErrorMessage = "Số lượng không hợp lệ.";
                        return View("Cart", lstGiohang);
                    }
                }

                return RedirectToAction("Cart");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật giỏ hàng.");
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi cập nhật giỏ hàng.";
                return View("Cart", lstGiohang);
            }
        }
        //Xoa Giohang
        public ActionResult DeleteCart(int id)
        {

            List<CartEntity> lstGiohang = GetCart();

            CartEntity sanpham = lstGiohang.SingleOrDefault(n => n.IDThuoc == id);

            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.IDThuoc == id);
                return RedirectToAction("Cart");

            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "UserInterface");
            }
            return RedirectToAction("Cart");
        }

        public ActionResult DeleteAll()
        {

            List<CartEntity> lstGiohang = GetCart();
            lstGiohang.Clear();
            return RedirectToAction("Index", "UserInterface");
        }
        [HttpGet]
        public ActionResult Order()
        {
            if (Session["usr"] == null || Session["usr"].ToString() == "")
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "UserInterface");
            }

            KHACHHANG cus = (KHACHHANG)Session["usr"];
            List<CartEntity> listcart = GetCart();
            TempData["InitialPoints"] = cus.TICHDIEM;
            ViewBag.TotalQuantity = TempData.ContainsKey("TotalQuantity") ? TempData["TotalQuantity"] : TotalQuantity();
            ViewBag.TotalPrice = TempData.ContainsKey("ToTalPrice") ? TempData["ToTalPrice"] : ToTalPrice();
            ViewBag.AvailablePoints = TempData.ContainsKey("AvailablePoints") ? TempData["AvailablePoints"] : cus.TICHDIEM;
            ViewBag.PointsUsed = TempData.ContainsKey("PointsUsed") ? TempData["PointsUsed"] : 0;
            ViewBag.DiscountFromPoints = TempData.ContainsKey("DiscountFromPoints") ? TempData["DiscountFromPoints"] : 0;
            ViewBag.FinalPriceAfterPoints = TempData.ContainsKey("FinalPriceAfterPoints") ? TempData["FinalPriceAfterPoints"] : ViewBag.TotalPrice;
            return View(listcart);
        }
        public ActionResult Order(FormCollection collection, int TypePaymentVN, int TypePayment, int pointsToUse)
        {
            HOADONXUAT or = new HOADONXUAT();
            KHACHHANG cus = (KHACHHANG)Session["usr"];
            KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(t => t.ID == cus.ID);

            List<CartEntity> crt = GetCart();
            or.MAKH = cus.ID;
            or.NGAYXUAT = DateTime.Now;
            or.TRANGTHAI = true;
            or.HOANTHANH = false;
            or.DATHANHTOAN = false;
            or.TTGIAOHANG = false;
            or.DIEMTLSD = pointsToUse;
            kh.TICHDIEM -= pointsToUse;
            cus.TICHDIEM -= pointsToUse;
          
            double finalPrice = FinalPriceAfterPoints(pointsToUse);
            ViewBag.AvailablePoints = cus.TICHDIEM;
            ViewBag.PointsUsed = pointsToUse;
            ViewBag.DiscountFromPoints = ToTalPrice() - finalPrice;
            ViewBag.FinalPriceAfterPoints = finalPrice;
            or.TONGTIEN = (decimal)finalPrice;
            //ketthuc
            db.HOADONXUATs.Add(or);
            db.SaveChanges();
            foreach (var item in crt)
            {
                CHITIETHDX ordt = new CHITIETHDX();
                ordt.MAHDX = or.ID;
                ordt.MATHUOC = item.IDThuoc;
                ordt.SOLUONG = item.soLuong;
                ordt.DONGIA = (decimal)item.giaBan;
                ordt.MADVT = item.madvt;
                ordt.TONGTIEN = (decimal)item.tongGia;
                ordt.LOSX = item.losx;
                db.CHITIETHDXes.Add(ordt);

                if (TypePayment == 1) {//Nếu đơn hàng COD thì trừ sl tồn
                    var it = db.CTTONKHOes.Where(x => x.LOSX == item.losx).FirstOrDefault();
                    it.SLTON = (it.SLTON) - item.soLuong;
                }
                db.SaveChanges();
            }
            if (TypePayment != 1)
            {
                var url = UrlPayment(TypePaymentVN, or.ID.ToString());
                cus.TICHDIEM = (int)TempData["InitialPoints"];
                return Redirect(url);
            }
            db.SaveChanges();
            //send mail cho khachs hang
            var strSanPham = "";
            var thanhtien = decimal.Zero;
            var TongTien = decimal.Zero;
            foreach (var sp in crt)
            {
                strSanPham += "<tr>";
                strSanPham += "<td>" + sp.tenThuoc + "</td>";
                strSanPham += "<td>" + sp.soLuong + "</td>";
                strSanPham += "<td>" + Recom_Pharmacy.Common.Common.FormatNumber(sp.tongGia, 0) + "</td>";
                strSanPham += "</tr>";
                thanhtien = sp.soLuong * (decimal)sp.giaBan;
            }
            TongTien = thanhtien;
            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
            contentCustomer = contentCustomer.Replace("{{MaDon}}", or.ID.ToString());
            contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
            contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", cus.TENKH);
            contentCustomer = contentCustomer.Replace("{{Phone}}", cus.SDT);
            contentCustomer = contentCustomer.Replace("{{Email}}", cus.EMAIL);
            contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", cus.DIACHI);
            contentCustomer = contentCustomer.Replace("{{ThanhTien}}", Recom_Pharmacy.Common.Common.FormatNumber(thanhtien, 0));
            contentCustomer = contentCustomer.Replace("{{TongTien}}", Recom_Pharmacy.Common.Common.FormatNumber(TongTien, 0));
            Recom_Pharmacy.Common.Common.SendMail("Pharmacy", "Đơn hàng #" + or.ID, contentCustomer.ToString(), cus.EMAIL);

            string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
            contentAdmin = contentAdmin.Replace("{{MaDon}}", or.ID.ToString());
            contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
            contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
            contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", cus.TENKH);
            contentAdmin = contentAdmin.Replace("{{Phone}}", cus.SDT);
            contentAdmin = contentAdmin.Replace("{{Email}}", cus.EMAIL);
            contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", cus.DIACHI);
            contentAdmin = contentAdmin.Replace("{{ThanhTien}}", Recom_Pharmacy.Common.Common.FormatNumber(thanhtien, 0));
            contentAdmin = contentAdmin.Replace("{{TongTien}}", Recom_Pharmacy.Common.Common.FormatNumber(TongTien, 0));
            Recom_Pharmacy.Common.Common.SendMail("Pharmacy", "Đơn hàng mới #" + or.ID, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
            Session["Cart"] = null;


            return RedirectToAction("OrderConfirmation", "Cart");

        }


        public ActionResult OrderConfirmation()
        {
            return View();
        }
        public ActionResult Newinformation()
        {
            if (Session["usr"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var ac = ((KHACHHANG)Session["usr"]);

            return View(new AccountClientEntity(ac));
        }
        [HttpPost]
        public ActionResult Newinformation(FormCollection fc)
        {
            var ac = ((KHACHHANG)Session["usr"]);

            if (Session["usr"] != null)
            {
                string userName = fc["userName"].ToString();
                string address = fc["address"].ToString();
                string phoneNumber = fc["phonenumber"].ToString();
                string name = fc["name"].ToString();
                string tinhthanh = fc["tinhthanh"].ToString();
                string gioitinh = fc["gioitinh"].ToString();
                string payment = fc["payment"].ToString();

                var temp = db.KHACHHANGs.SingleOrDefault(x => x.Username == userName);
                if (temp != null && address != "")
                {
                    temp.DIACHI = fc["address"];
                    temp.TENKH = fc["name"];
                    temp.SDT = fc["phonenumber"];
                    temp.TINHTHANH.TENTINH = fc["tinhthanh"];
                    temp.GIOITINH = fc["gioitinh"];
                    temp.Payment = fc["payment"];
                    db.SaveChanges();
                    Session["usr"] = temp;
                    return RedirectToAction("Order", "Cart");
                }
            }
            else
            {
                return RedirectToAction("Index", "UserInterface");
            }
            ModelState.AddModelError("", "Error cannot change Infomation..");
            return View(new AccountClientEntity(ac));
        }

        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                int orderCode = Convert.ToInt32(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = db.HOADONXUATs.FirstOrDefault(x => x.ID == (orderCode));
                        if (itemOrder != null)
                        {
                            HOADONXUAT or = new HOADONXUAT();
                            KHACHHANG cus = (KHACHHANG)Session["usr"];

                            List<CartEntity> crt = GetCart();
                            itemOrder.DATHANHTOAN = true;
                            db.HOADONXUATs.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            //send mail cho khachs hang
                            var strSanPham = "";
                            var thanhtien = decimal.Zero;
                            var TongTien = decimal.Zero;

                            foreach (var sp in crt)
                            {
                                var tonkho = db.CTTONKHOes.Where(x => x.LOSX == sp.losx).FirstOrDefault();
                                if (tonkho != null)
                                {
                                    tonkho.SLTON = tonkho.SLTON - sp.soLuong;
                                    db.SaveChanges();
                                }
                                strSanPham += "<tr>";
                                strSanPham += "<td>" + sp.tenThuoc + "</td>";
                                strSanPham += "<td>" + sp.soLuong + "</td>";
                                strSanPham += "<td>" + Recom_Pharmacy.Common.Common.FormatNumber(sp.tongGia, 0) + "</td>";
                                strSanPham += "</tr>";
                                thanhtien = sp.soLuong * (decimal)sp.giaBan;
                            }
                            TongTien = thanhtien;
                            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                            contentCustomer = contentCustomer.Replace("{{MaDon}}", itemOrder.ID.ToString());
                            contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                            contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", cus.TENKH);
                            contentCustomer = contentCustomer.Replace("{{Phone}}", cus.SDT);
                            contentCustomer = contentCustomer.Replace("{{Email}}", cus.EMAIL);
                            contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", cus.DIACHI);
                            contentCustomer = contentCustomer.Replace("{{ThanhTien}}", Recom_Pharmacy.Common.Common.FormatNumber(thanhtien, 0));
                            contentCustomer = contentCustomer.Replace("{{TongTien}}", Recom_Pharmacy.Common.Common.FormatNumber(TongTien, 0));
                            Recom_Pharmacy.Common.Common.SendMail("Pharmacy", "Đơn hàng #" + itemOrder.ID, contentCustomer.ToString(), cus.EMAIL);

                            string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                            contentAdmin = contentAdmin.Replace("{{MaDon}}", itemOrder.ID.ToString());
                            contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                            contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                            contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", cus.TENKH);
                            contentAdmin = contentAdmin.Replace("{{Phone}}", cus.SDT);
                            contentAdmin = contentAdmin.Replace("{{Email}}", cus.EMAIL);
                            contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", cus.DIACHI);
                            contentAdmin = contentAdmin.Replace("{{ThanhTien}}", Recom_Pharmacy.Common.Common.FormatNumber(thanhtien, 0));
                            contentAdmin = contentAdmin.Replace("{{TongTien}}", Recom_Pharmacy.Common.Common.FormatNumber(TongTien, 0));
                            Recom_Pharmacy.Common.Common.SendMail("Pharmacy", "Đơn hàng mới #" + itemOrder.ID, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
                            Session["Cart"] = null;
                        }
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                         var itemOrder = db.HOADONXUATs.FirstOrDefault(x => x.ID == (orderCode));
                        if (itemOrder != null)
                        {
                            HOADONXUAT or = new HOADONXUAT();
                            KHACHHANG cus = (KHACHHANG)Session["usr"];

                            List<CartEntity> crt = GetCart();
                            itemOrder.DATHANHTOAN = false; // Đặt lại trạng thái thành false nếu thanh toán không thành công
                            db.HOADONXUATs.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            //send mail cho khachs hang
                            var strSanPham = "";
                            var thanhtien = decimal.Zero;
                            var TongTien = decimal.Zero;
                            foreach (var sp in crt)
                            {
                                strSanPham += "<tr>";
                                strSanPham += "<td>" + sp.tenThuoc + "</td>";
                                strSanPham += "<td>" + sp.soLuong + "</td>";
                                strSanPham += "<td>" + Recom_Pharmacy.Common.Common.FormatNumber(sp.tongGia, 0) + "</td>";
                                strSanPham += "</tr>";
                                thanhtien = sp.soLuong * (decimal)sp.giaBan;
                            }
                            TongTien = thanhtien;
                            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                            contentCustomer = contentCustomer.Replace("{{MaDon}}", itemOrder.ID.ToString());
                            contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                            contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", cus.TENKH);
                            contentCustomer = contentCustomer.Replace("{{Phone}}", cus.SDT);
                            contentCustomer = contentCustomer.Replace("{{Email}}", cus.EMAIL);
                            contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", cus.DIACHI);
                            contentCustomer = contentCustomer.Replace("{{ThanhTien}}", Recom_Pharmacy.Common.Common.FormatNumber(thanhtien, 0));
                            contentCustomer = contentCustomer.Replace("{{TongTien}}", Recom_Pharmacy.Common.Common.FormatNumber(TongTien, 0));
                            Recom_Pharmacy.Common.Common.SendMail("Pharmacy", "Đơn hàng #" + itemOrder.ID, contentCustomer.ToString(), cus.EMAIL);

                            string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                            contentAdmin = contentAdmin.Replace("{{MaDon}}", itemOrder.ID.ToString());
                            contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                            contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                            contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", cus.TENKH);
                            contentAdmin = contentAdmin.Replace("{{Phone}}", cus.SDT);
                            contentAdmin = contentAdmin.Replace("{{Email}}", cus.EMAIL);
                            contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", cus.DIACHI);
                            contentAdmin = contentAdmin.Replace("{{ThanhTien}}", Recom_Pharmacy.Common.Common.FormatNumber(thanhtien, 0));
                            contentAdmin = contentAdmin.Replace("{{TongTien}}", Recom_Pharmacy.Common.Common.FormatNumber(TongTien, 0));
                            Recom_Pharmacy.Common.Common.SendMail("Pharmacy", "Đơn hàng mới #" + itemOrder.ID, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
                            Session["Cart"] = null;
                        }
                    }
                }
            }
            return View();
        }
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            int orderIntId = Convert.ToInt32(orderCode);
            var order = db.HOADONXUATs.FirstOrDefault(x => x.ID == orderIntId);
            //Get Config Info
            if (order == null)
            {
                // Xử lý khi đơn hàng không tồn tại
                throw new Exception("Order not found");
            }
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TONGTIEN * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.NGAYXUAT.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.ID);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", Convert.ToString(order.ID)); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
    }
}