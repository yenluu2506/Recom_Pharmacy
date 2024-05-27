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
            //    var temp = db.Orders.Where(a => a.Orderdate.Value.Month == dateTimeNow.Month).Sum(s => s.Totalprice);
            //    if (temp == null)
            //    {
            //        temp = 0;
            //    }
            //    data[i] = temp.ToString();
            //    dateTimeNow = dateTimeNow.AddMonths(1);
            //}
            //ViewBag.dateX = dateX;
            //ViewBag.data = data;

            //// DatachartLine();
            //var ac = (Admin)Session["Account"];
            //if (ac == null)
            //{
            //    return RedirectToAction("Login", "Admin");
            //}
            //else { return View(); }
            return View();
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
            var temp = db.HOADONXUATs.Where(o => o.TRANGTHAI == false).ToList();
            List<OrderEntity> lisorder = new List<OrderEntity>();
            foreach (var item in temp)
            {
                OrderEntity or = new OrderEntity();
                or.TypeOf_OrderEntity(item);
                lisorder.Add(or);


            }
            return View(lisorder);
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
                tem.XULY = true;
                tem.TTGIAOHANG = true;
            }
            else if (action == "KhongDuyet")
            {
                tem.XULY = false;
                tem.TTGIAOHANG = false;
            }

            tem.GHICHU = fc["note"];
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


            return View(lisorder);
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