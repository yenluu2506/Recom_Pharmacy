using Recom_Pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Recom_Pharmacy.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        RecomPharmacyEntities db = new  RecomPharmacyEntities();
        public ActionResult SignOut()
        {
            //FormsAuthentication.SignOut();
            Response.Cookies.Clear();
            return RedirectToAction("Login", "Admin");

        }
        public ActionResult Index()
        {
            //DateTime dateTimeNow = DateTime.Now.Date;
            //dateTimeNow = dateTimeNow.AddYears(-1);

            //string[] dateX = new string[12];
            //string[] data = new string[12];
            //for (int i = 0; i < 12; i++)
            //{

            //    dateX[i] = (dateTimeNow.Month.ToString() + "/" + dateTimeNow.Year.ToString()).ToString();
            //    var temp = db.HOADONXUATs.Where(a => a.NGAYXUAT.Month == dateTimeNow.Month).Sum(s => s.TONGTIEN);
            //    if (temp == null)
            //    {
            //        temp = 0;
            //    }
            //    data[i] = temp.ToString();
            //    dateTimeNow = dateTimeNow.AddMonths(1);
            //}
            //ViewBag.dateX = dateX;
            //ViewBag.data = data;

            //DatachartLine();
            //var ac = (Admin)Session["Account"];
            //if (ac == null)
            //{
            //    return RedirectToAction("Login", "Admin");
            //}
            //else { return View(); }
            //return View();


            //DateTime dateTimeNow = DateTime.Now.Date;
            //dateTimeNow = dateTimeNow.AddYears(-1);
            DateTime startDate = new DateTime(2024, 1, 1);

            string[] dateX = new string[12];
            int[] data = new int[12];

            for (int i = 0; i < 12; i++)
            {
                dateX[i] = startDate.Month.ToString() + "/" + startDate.Year.ToString();
                var temp = db.HOADONXUATs
                              .Where(a => a.NGAYXUAT.Month == startDate.Month && a.NGAYXUAT.Year == startDate.Year)
                              .Sum(s => (int?)s.TONGTIEN) ?? 0;
                data[i] = temp;
                startDate = startDate.AddMonths(1);
            }

            ViewBag.DateX = dateX;
            ViewBag.Data = data;

            var topSellingProducts = db.CHITIETHDXes
                               .GroupBy(h => h.THUOC)
                               .Select(g => new
                               {
                                   ProductName = g.Key.TENTHUOC,
                                   TotalSold = g.Sum(h => h.SOLUONG)
                               })
                               .OrderByDescending(g => g.TotalSold)
                               .Take(5)
                               .ToList();

            ViewBag.TopSellingProducts = topSellingProducts.Select(p => p.ProductName).ToArray();
            ViewBag.TopSellingData = topSellingProducts.Select(p => p.TotalSold).ToArray();

            var ac = (Admin)Session["Account"];
            if (ac == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View();
            }
        }
        public ActionResult Login()
        {
            return View();

        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var userName = collection["userName"];

            var passWord = collection["passWord"];


            Admin ad = db.Admins.SingleOrDefault(n => n.Username == userName && n.Passwords == passWord);
            if (ad != null)
            {
                Session["Account"] = ad;
                Response.Cookies["usr"].Value = ad.Username;

                var name = db.Admins.SingleOrDefault(a => a.Username == ad.Username).Name;
                Response.Cookies["Name"].Value = name;

                var atar = db.Admins.SingleOrDefault(a => a.Username == ad.Username).Picture;
                if (atar == null || atar == "")
                {
                    atar = "~/img/Item/avatar-default-icon.png";
                }

                Response.Cookies["avatar"].Value = atar;

                return RedirectToAction("Index", "Admin");
            }
            else

                ModelState.AddModelError("", "The user login or password  is incorrect..");

            return View();



        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Passwords,Name,Picture")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Admins.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(admin);
        }

        public ActionResult ListOrder()
        {
            var temp = db.HOADONXUATs.Where(o => o.HOANTHANH == false && o.TRANGTHAI == true).ToList();
            List<OrderEntity> lisorder = new List<OrderEntity>();
            foreach (var item in temp)
            {
                OrderEntity or = new OrderEntity();
                or.TypeOf_OrderEntity(item);
                lisorder.Add(or);


            }
            
            var ac = (Admin)Session["Account"];
            if (ac == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View(lisorder);
            }
        }

        // xacs nhan

        public ActionResult Comfirm(long? id)
        {
            var temp = db.CHITIETHDXes.Where(d => d.MAHDX == id);
            List<OrderDetailEntity> listdetail = new List<OrderDetailEntity>();
            foreach (var item in temp)
            {
                OrderDetailEntity or = new OrderDetailEntity();
                or.TypeOf_OrderEntity(item);
                listdetail.Add(or);
            }
            ViewBag.Date = db.HOADONXUATs.SingleOrDefault(a => a.ID == id).NGAYGIAOHANG;
            ViewBag.Note = db.HOADONXUATs.SingleOrDefault(a => a.ID == id).GHICHU;
            ViewBag.id = id;
            return View(listdetail);

        }

        [HttpPost]
        public ActionResult Comfirm(FormCollection fc)
        {
            string date = fc["date"];
            long id = Convert.ToInt64(fc["id"]);
            var tem = db.HOADONXUATs.SingleOrDefault(d => d.ID == id);

            tem.TRANGTHAI = true;
            tem.NGAYGIAOHANG = Convert.ToDateTime(date);

            string action = fc["action"];
            if (action == "Duyet")
            {
                tem.HOANTHANH = true;
                tem.TTGIAOHANG = true;
                tem.DATHANHTOAN = true;
                // Cập nhật điểm tích lũy khi XULY = TRUE
                if ((bool)tem.HOANTHANH)
                {
                    // Chuyển đổi 0.01 thành kiểu decimal
                    double conversionRate = 0.01;
                    double totalPoints = (double)tem.TONGTIEN * conversionRate;
                    var customer = db.KHACHHANGs.SingleOrDefault(c => c.ID == tem.MAKH);
                    if (customer != null)
                    {
                        customer.TICHDIEM +=(int)totalPoints;
                    }
                }
            }
            else if (action == "KhongDuyet")
            {
                tem.TRANGTHAI = false;
                tem.HOANTHANH = true;
                tem.TTGIAOHANG = false;
                // Cập nhật điểm tích lũy khi HOANTHANH = FALSE
                if ((bool)tem.HOANTHANH)
                {
                    // Chuyển đổi 0.01 thành kiểu decimal
                    double conversionRate = 0.01;
                    double totalPoints = (double)tem.TONGTIEN * conversionRate;
                    var customer = db.KHACHHANGs.SingleOrDefault(c => c.ID == tem.MAKH);
                 
                    if (customer != null)
                    {
                        customer.TICHDIEM += tem.DIEMTLSD;
                    }
                }
            }

            tem.GHICHU = fc["note"];
            if(tem.DATHANHTOAN == true && tem.TRANGTHAI == false && tem.HOANTHANH==true && tem.TTGIAOHANG == false)//neu admin huy don hang da thanh toan
            {
                tem.GHICHU += " - Hoàn tiền";
            }
            db.SaveChanges();

            return RedirectToAction("ListOrder");

        }
        [HttpPost]
        public ActionResult Reject(long? id)
        {
            var order = db.HOADONXUATs.SingleOrDefault(d => d.ID == id);
            order.TRANGTHAI = false;
            db.SaveChanges();

            return RedirectToAction("ListOrder");
        }

        //-------------------------------------------
        public ActionResult AllListOrder()
        {
            var temp = db.HOADONXUATs.ToList();
            List<OrderEntity> lisorder = new List<OrderEntity>();
            foreach (var item in temp)
            {
                OrderEntity or = new OrderEntity();
                or.TypeOf_OrderEntity(item);
                lisorder.Add(or);


            }


            var ac = (Admin)Session["Account"];
            if (ac == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View(lisorder);
            }
        }

        // xacs nhan

        public ActionResult OrderDetail(long? id)
        {
            var temp = db.CHITIETHDXes.Where(d => d.MAHDX == id);
            List<OrderDetailEntity> listdetail = new List<OrderDetailEntity>();
            foreach (var item in temp)
            {
                OrderDetailEntity or = new OrderDetailEntity();
                or.TypeOf_OrderEntity(item);
                listdetail.Add(or);
            }

            return View(listdetail);

        }

        public ActionResult Productnotsold()
        {
            var results = from t1 in db.THUOCs
                          where !(from t2 in db.HOADONXUATs
                                  join a in db.CHITIETHDXes on t2.ID equals a.MAHDX
                                  where t2.NGAYXUAT == DateTime.Now
                                  select t2.ID).Contains(t1.ID)
                          select t1;
            return View(results.ToList());
        }
    }
}