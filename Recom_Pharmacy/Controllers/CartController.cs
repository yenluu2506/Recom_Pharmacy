using Recom_Pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Recom_Pharmacy.Controllers
{
    public class CartController : Controller
    {
        RecomPharmacyEntities db = new RecomPharmacyEntities();
        public List<CartEntity> GetCart()
        {
            List<CartEntity> lstCart = Session["Cart"] as List<CartEntity>;
            if (lstCart == null)
            {

                lstCart = new List<CartEntity>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
        }

        public ActionResult AddtoCart(int id, string strURL)
        {
            List<CartEntity> lstcart = GetCart();
            //Kiem tra sách này tồn tại trong Session["Giohang"] chưa?

            CartEntity Product = lstcart.Find(n => n.IDThuoc == id);
            if (Product == null)
            {
                Product = new CartEntity(id);
                lstcart.Add(Product);
                return Redirect(strURL);
            }
            else
            {
                Product.soLuong++;
                return Redirect(strURL);
            }
        }
        public ActionResult Cart()
        {
            List<CartEntity> lstCart = GetCart();
            if (lstCart.Count == 0)
            {
                return RedirectToAction("Index", "UserInterface");
            }
            foreach (var item in lstCart)
            {
                var thuoc = db.THUOCs.SingleOrDefault(t => t.ID == item.IDThuoc);
                if (thuoc != null)
                {
                    item.maxSoLuong = thuoc.SOLUONG;
                }
                else
                {
                    item.maxSoLuong = 0; // Handle the case where the item is not found
                }
            }
            ViewBag.TotalQuantity = TotalQuantity();
            ViewBag.ToTalPrice = ToTalPrice();
            return View(lstCart);
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

        public ActionResult CartPartial()
        {
            ViewBag.TotalQuantity = TotalQuantity();
            ViewBag.TotalPrice = ToTalPrice();
            return PartialView();
        }
        //Cap nhat Giỏ hàng
        public ActionResult EditCart(int id, FormCollection f)
        {


            //List<CartEntity> lstGiohang = GetCart();

            //CartEntity item = lstGiohang.SingleOrDefault(n => n.IDThuoc == id);

            //if (item != null)
            //{
            //    item.soLuong = int.Parse(f["txtSoluong"].ToString());
            //}
            //return RedirectToAction("Cart");


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
                        var thuoc = db.THUOCs.SingleOrDefault(t => t.ID == id);
                        if (thuoc == null)
                        {
                            return HttpNotFound();
                        }

                        // Check if the requested quantity exceeds the available stock
                        if (requestedQuantity > thuoc.SOLUONG)
                        {
                            // Add an error message and return to the cart view
                            ModelState.AddModelError("", "Số lượng đặt hàng không được vượt quá số lượng hiện có.");
                            ViewBag.ErrorMessage = "Số lượng đặt hàng không được vượt quá số lượng hiện có.";

                            // Rebuild the model for the view in case of error
                            return View("Cart", lstGiohang);
                        }

                        // Update the cart item quantity
                        item.soLuong = requestedQuantity;
                    }
                    else
                    {
                        // Handle parse failure
                        ModelState.AddModelError("", "Số lượng không hợp lệ.");
                        ViewBag.ErrorMessage = "Số lượng không hợp lệ.";
                        return View("Cart", lstGiohang);
                    }
                }

                return RedirectToAction("Cart");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
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
            List<CartEntity> listcart = GetCart();
            ViewBag.ToTalQuanttity = TotalQuantity();
            ViewBag.TotalPrice = ToTalPrice();

            return View(listcart);
        }
        public ActionResult Order(FormCollection collection)
        {
            HOADONXUAT or = new HOADONXUAT();
            KHACHHANG cus = (KHACHHANG)Session["usr"];

            List<CartEntity> crt = GetCart();
            or.MAKH = cus.ID;
            or.NGAYXUAT = DateTime.Now;
            or.TRANGTHAI = false;
            or.TONGTIEN = (decimal)ToTalPrice();
            db.HOADONXUATs.Add(or);
            db.SaveChanges();
            foreach (var item in crt)
            {
                CHITIETHDX ordt = new CHITIETHDX();
                ordt.MAHDX = or.ID;
                ordt.MATHUOC = item.IDThuoc;
                ordt.SOLUONG = item.soLuong;
                ordt.DONGIA = (decimal)item.giaBan;
                ordt.TONGTIEN = (decimal)item.tongGia;
                db.CHITIETHDXes.Add(ordt);


                var it = db.THUOCs.Find(item.IDThuoc);
                it.SOLUONG = (it.SOLUONG) - item.soLuong;
                db.SaveChanges();
            }
            db.SaveChanges();
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

                var temp = db.KHACHHANGs.SingleOrDefault(x => x.Username == userName);
                if (temp != null && address != "")
                {
                    temp.DIACHI = fc["address"];
                    temp.TENKH = fc["name"];
                    temp.SDT = fc["phonenumber"];
                    temp.TINHTHANH.TENTINH = fc["tinhthanh"];
                    temp.GIOITINH = fc["gioitinh"];
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
    }
}