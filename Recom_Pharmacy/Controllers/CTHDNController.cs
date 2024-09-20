using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.UI;
using Recom_Pharmacy.Models;

namespace Recom_Pharmacy.Controllers
{
    public class CTHDNController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: CTHDN
        public ActionResult Index(string Searchtext, int? page, int? SelectedDVT, int? SelectedThuoc, int? id, int? SelectedHDN)
        {
            //var cHITIETHDNs = db.CHITIETHDNs
            //            .Include(c => c.CTKHO)
            //            .Include(c => c.DONVITINH)
            //            .Include(c => c.HOADONNHAP)
            //            .Where(c => c.HOADONNHAP.ID == id)
            //            .ToList();
            //return View(cHITIETHDNs);
            //return RedirectToAction("index", new { id = id });
            ViewBag.CTTonKho = new SelectList(db.CTTONKHOes.ToList(), "ID", "ID");
            ViewBag.Thuoc = new SelectList(db.KHOes.ToList(), "ID", "TENTHUOC");
            ViewBag.HDN = new SelectList(db.HOADONNHAPs.ToList(), "ID", "SOHD");
            ViewBag.DVT = new SelectList(db.HOADONNHAPs.ToList(), "ID", "TENDVT");
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<CHITIETHDN> items = db.CHITIETHDNs.OrderByDescending(x => x.ID).Where(x => x.HOADONNHAP.ID == id);
      
            if (SelectedThuoc.HasValue)
            {
                items = items.Where(x => x.THUOC.ID == SelectedThuoc.Value);
            }
            if (SelectedHDN.HasValue)
            {
                items = items.Where(x => x.HOADONNHAP.ID == SelectedHDN.Value);
            }
            if (SelectedDVT.HasValue)
            {
                items = items.Where(x => x.DONVITINH.ID == SelectedDVT.Value);
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedThuoc = SelectedThuoc;
            ViewBag.SelectedHDN = SelectedHDN;
            ViewBag.SelectedDVT = SelectedDVT;
            ViewBag.page = page;
            return View(items);
        }

        // GET: CTHDN/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CHITIETHDN cHITIETHDN = db.CHITIETHDNs.Find(id);
            if (cHITIETHDN == null)
            {
                return HttpNotFound();
            }
            return View(cHITIETHDN);
        }

        // GET: CTHDN/Create
        public ActionResult Create(int? id)
        {
            //var selectList = db.CTTONKHOes
            //     .Select(ct => new {
            //         ID = ct.ID,
            //         TenKho = ct.KHO.TENKHO // Assuming KHO is a navigation property in CTTONKHO
            //     }).Distinct()
            //     .ToList();
            //ViewBag.MACTTONKHO = new SelectList(db.CTTONKHOes, "ID", "ID");
            ////ViewBag.MACTTONKHO = new SelectList(selectList, "ID", "TenKho");
            //ViewBag.MADVT = new SelectList(db.DONVITINHs, "ID", "TENDVT");
            //ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC");
            //ViewBag.MAHDN = new SelectList(db.HOADONNHAPs, "ID", "SOHD", id);
            //return View();


            ViewBag.MADVT = new SelectList(db.DONVITINHs, "ID", "TENDVT");
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC");

            var selectListCTTONKHO = new SelectList(db.CTTONKHOes, "ID", "ID");

            if (id.HasValue || Request.IsAjaxRequest()) // Check for edit or AJAX request
            {
                int selectedDrugId = id.HasValue ? id.Value : Convert.ToInt32(Request.Params["selectedDrugId"]); // Get drug ID from query string (assuming you pass it)
                var ctTonKhoList = db.CTTONKHOes.Where(ct => ct.THUOC.ID == selectedDrugId).ToList();
                selectListCTTONKHO = new SelectList(ctTonKhoList, "ID", "ID");

                if (Request.IsAjaxRequest()) // Handle AJAX request
                {
                    return Json(selectListCTTONKHO.Select(item => new { id = item.Value, text = item.Text }), JsonRequestBehavior.AllowGet);
                }
            }

            ViewBag.MACTTONKHO = selectListCTTONKHO;
            ViewBag.MAHDN = new SelectList(db.HOADONNHAPs, "ID", "SOHD", id);

            return View();
        }

        // POST: CTHDN/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,MAHDN,MACTTONKHO,MATHUOC,SOLUONG,DONGIA,MADVT,CHIETKHAU")] CHITIETHDN cHITIETHDN)
        {
            if (ModelState.IsValid)
            {
                db.CHITIETHDNs.Add(cHITIETHDN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MADVT = new SelectList(db.DONVITINHs, "ID", "TENDVT", cHITIETHDN.MADVT);
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC", cHITIETHDN.MATHUOC);
            ViewBag.MAHDN = new SelectList(db.HOADONNHAPs, "ID", "SOHD", cHITIETHDN.MAHDN);
            return View(cHITIETHDN);
        }

        // GET: CTHDN/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CHITIETHDN cHITIETHDN = db.CHITIETHDNs.Find(id);
            if (cHITIETHDN == null)
            {
                return HttpNotFound();
            }
            ViewBag.MADVT = new SelectList(db.DONVITINHs, "ID", "TENDVT", cHITIETHDN.MADVT);
            ViewBag.MAHDN = new SelectList(db.HOADONNHAPs, "ID", "SOHD", cHITIETHDN.MAHDN);
            return View(cHITIETHDN);
        }

        // POST: CTHDN/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,MAHDN,MACTTONKHO,MATHUOC,SOLUONG,DONGIA,MADVT,CHIETKHAU")] CHITIETHDN cHITIETHDN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cHITIETHDN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MADVT = new SelectList(db.DONVITINHs, "ID", "TENDVT", cHITIETHDN.MADVT);
            ViewBag.MAHDN = new SelectList(db.HOADONNHAPs, "ID", "SOHD", cHITIETHDN.MAHDN);
            return View(cHITIETHDN);
        }

        // GET: CTHDN/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CHITIETHDN cHITIETHDN = db.CHITIETHDNs.Find(id);
            if (cHITIETHDN == null)
            {
                return HttpNotFound();
            }
            return View(cHITIETHDN);
        }

        // POST: CTHDN/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CHITIETHDN cHITIETHDN = db.CHITIETHDNs.Find(id);
            db.CHITIETHDNs.Remove(cHITIETHDN);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
