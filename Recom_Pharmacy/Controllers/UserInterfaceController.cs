using Newtonsoft.Json;
using PagedList;
using Recom_Pharmacy.Models;
using Recom_Pharmacy.Models.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        ThuocViewModel thuoc = new ThuocViewModel();
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
            var model = thuoc.getThuocWithTonKho(search);
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
        public ActionResult AllProduct(string search)
        {
            var model = thuoc.getThuocWithTonKho(search);

            return View(model);
        }


        public ActionResult ViewAllProduct(string search, int? page)
        {
            var model = thuoc.getThuocWithTonKho(search);
            int pageSize = 9; 
            int pageNumber = (page ?? 1); 

            return View(model.ToPagedList(pageNumber, pageSize));
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

        public ActionResult ProductByLT(int id, int? page)
        {
            var pr = (from d in db.THUOCs where d.MALOAI == id && d.TRANGTHAI == true select d).ToList();
            ViewBag.CategoryId = id;
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            return View(pr.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult FilterByPrice(decimal from, decimal to)
        {
            var filteredProducts = thuoc.getByFilterByPrice(from, to);
            return PartialView("_ProductList", filteredProducts);
        }
        public ActionResult FilterByPriceByLT(decimal from, decimal to, int categoryId)
        {
            var filteredProducts = thuoc.getByFilterByCategoryByPrice(from,to,categoryId);
            return PartialView("_ProductList", filteredProducts);
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
            THUOC item = thuoc.findById(id);
            return View(item);
        }
        [HttpGet]
        public ActionResult CheckStock(int productId)
        {
            var product = thuoc.findById(productId);
            bool isOutOfStock = false;
            var slTon = product?.CTTONKHOes.FirstOrDefault()?.SLTON;
            if (slTon <= 0 || slTon == null)
            {
                isOutOfStock = true;
            }
            return Json(new { stockStatus = isOutOfStock ? "outOfStock" : "inStock" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListOrderClient()
        {
            var ac = (KHACHHANG)Session["usr"];
            if (ac == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var temp = db.HOADONXUATs.Where(p => p.KHACHHANG.Username == ac.Username).OrderByDescending(x=>x.ID);
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
            var tem = db.HOADONXUATs.SingleOrDefault(d => d.ID == id);
            tem.TRANGTHAI = false;
            tem.HOANTHANH = true;
            tem.TTGIAOHANG = false;
            if(tem.DATHANHTOAN == true)
            {
                tem.GHICHU = "Hoàn tiền";
            }
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
        public ActionResult Search()
        {
            return PartialView();
        }
    }
}