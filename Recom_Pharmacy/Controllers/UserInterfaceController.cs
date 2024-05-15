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
    }
}