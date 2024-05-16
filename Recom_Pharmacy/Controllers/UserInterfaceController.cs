using Recom_Pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Recom_Pharmacy.Controllers
{
    public class UserInterfaceController : Controller
    {
        RecomPharmacyEntities db = new RecomPharmacyEntities();

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
    }
}