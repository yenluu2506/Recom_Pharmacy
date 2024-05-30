using Newtonsoft.Json;
using Recom_Pharmacy.Models;
using Recom_Pharmacy.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

namespace Recom_Pharmacy.Controllers
{
    public class UserInterfaceController : Controller
    {
        RecomPharmacyEntities db = new RecomPharmacyEntities();
        string baseURL = "http://localhost:5555";
        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            //Response.Cookies.Clear();
            Session.Clear();

            return RedirectToAction("Index", "UserInterface");

        }
        // GET: UserInterface
        public ActionResult Index(string search)
        {
            var model = db.THUOCs.Where(x => x.TRANGTHAI == true).OrderByDescending(c => c.NGAYSX).Where(nv => nv.TENTHUOC.Contains(search) || search == null && nv.TRANGTHAI == true).ToList();

            return View(model);
        }
        public ActionResult Sessionlogin()
        {
            return PartialView();
        }
        public ActionResult Profile()
        {
            var ac = (KHACHHANG)Session["usr"];
            var t = from a in db.KHACHHANGs where a.Username == ac.Username select a;
            return View(t.ToList());
        }

        public ActionResult ViewAllProduct(string search)
        {
            var model = db.THUOCs.Where(x => x.TRANGTHAI == true).OrderByDescending(c => c.NGAYSX).Where(nv => nv.TENTHUOC.Contains(search) || search == null && nv.TRANGTHAI == true).ToList();

            return View(model);
        }
        private List<THUOC> NewItem(int count)
        {
            return db.THUOCs.Where(d => d.TRANGTHAI == true).OrderByDescending(a => a.NGAYSX).Take(count).ToList();
        }
        public ActionResult NewProduct()
        {
            return PartialView(NewItem(5));
        }
        public ActionResult Menu()
        {
            var menu = from t in db.MENUTHUOCs select t;
            return PartialView(menu);
        }
        public ActionResult LoaiThuoc(int id)
        {
            var b = (from t in db.LOAITHUOCs where t.MAMENU == id select t).ToList();

            return PartialView(b);
        }

        public ActionResult ProductByLT(int id)
        {
            var pr = from d in db.THUOCs where d.MALOAI == id && d.TRANGTHAI == true select d;
            return View(pr);
        }
        public async Task<ActionResult> DetailProduct(int id)
        {
            List<string> sanphamgoiy = new List<string>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage getData = await client.GetAsync($"api?id={id}");
                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    var productRespone = JsonConvert.DeserializeObject<ProductRespone>(results);

                    sanphamgoiy = productRespone.SanPhamGoiY;
                    ViewBag.HienThiSanPhamGoiY = sanphamgoiy;
                }
            }
            ViewBag.tatcasanpham = db.THUOCs.ToList();
            THUOC item = db.THUOCs.Find(id);
            return View(item);
        }
        public ActionResult ListOrderClient()
        {
            var ac = (KHACHHANG)Session["usr"];
            if (ac == null)
            {
                return RedirectToAction("Login", "Acction");
            }

            var temp = db.HOADONXUATs.Where(p => p.KHACHHANG.Username == ac.Username);
            List<OrderEntity> listProdcut = new List<OrderEntity>();
            foreach (var item in temp)
            {
                OrderEntity pr = new OrderEntity();
                pr.TypeOf_OrderEntity(item);
                listProdcut.Add(pr);
            }


            return View(listProdcut);


        }
        public ActionResult ListOrderDetailClient(long? id)
        {
            var temp = db.CHITIETHDXes.Where(d => d.MAHDX == id);
            List<OrderDetailEntity> listdetail = new List<OrderDetailEntity>();
            foreach (var item in temp)
            {
                OrderDetailEntity or = new OrderDetailEntity();
                or.TypeOf_OrderEntity(item);
                listdetail.Add(or);
            }


            return PartialView(listdetail);

        }
        [HttpGet]
        public ActionResult CancelOrder(long? id)
        {

            var temp = db.CHITIETHDXes.Where(d => d.MAHDX == id).ToList();
            db.CHITIETHDXes.RemoveRange(temp);
            db.SaveChanges();
            var tem = db.HOADONXUATs.SingleOrDefault(d => d.ID == id);
            db.HOADONXUATs.Remove(tem);
            db.SaveChanges();
            return RedirectToAction("ListOrderClient");

        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(FormCollection collection)
        {
            string Name = collection["Name"];
            string Email = collection["Email"];
            string Mota = collection["Mota"];
            string Chitiet = collection["Chitiet"];

            if (ModelState.IsValid)
            {
                    Feedback cs = new Feedback();
                    cs.HOTEN = Name;
                    cs.EMAIL = Email;
                    cs.MOTA = Mota;
                    cs.CHITIET = Chitiet;
                    db.Feedbacks.Add(cs);
                    db.SaveChanges();
                return RedirectToAction("ThankYou");
            }
            return View();
        }
        public ActionResult Thankyou()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        private List<Blog> NewBlogs(int count)
        {
            return db.Blogs.OrderByDescending(a => a.NGAYVIET).Take(count).ToList();
        }
        public ActionResult Blog()
        {

            return View(NewBlogs(5));
        }
        public ActionResult BlogDetail(long id)
        {

            var blog = from t in db.Blogs
                       where t.ID == id
                       select t;
            return View(blog.Single());
        }
        public ActionResult RecentBlog()
        {

            return PartialView(NewBlogs(4));
        }
    }
}