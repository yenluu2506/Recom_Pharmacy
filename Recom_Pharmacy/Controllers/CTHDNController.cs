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
        public ActionResult Index(string Searchtext, int? page, int? SelectedDVT, int? SelectedCTKho, int? SelectedThuoc, int? id, int? SelectedHDN)
        {
            //var cHITIETHDNs = db.CHITIETHDNs
            //            .Include(c => c.CTKHO)
            //            .Include(c => c.DONVITINH)
            //            .Include(c => c.HOADONNHAP)
            //            .Where(c => c.HOADONNHAP.ID == id)
            //            .ToList();
            //return View(cHITIETHDNs);
            //return RedirectToAction("index", new { id = id });
            ViewBag.CTKho = new SelectList(db.CTKHOes.ToList(), "ID");
            ViewBag.Thuoc = new SelectList(db.TONKHOes.ToList(), "ID", "TENTHUOC");
            ViewBag.HDN = new SelectList(db.HOADONNHAPs.ToList(), "ID", "SOHD");
            ViewBag.DVT = new SelectList(db.HOADONNHAPs.ToList(), "ID", "TENDVT");
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<CHITIETHDN> items = db.CHITIETHDNs.OrderByDescending(x => x.ID).Where(x => x.HOADONNHAP.ID == id);
            if (SelectedCTKho.HasValue)
            {
                items = items.Where(x => x.CTKHO.ID == SelectedCTKho.Value);
            }
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
            ViewBag.SelectedTonKho = SelectedCTKho;
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
            ViewBag.MACTKHO = new SelectList(db.CTKHOes, "ID", "MAKHO");
            ViewBag.MADVT = new SelectList(db.DONVITINHs, "ID", "TENDVT");
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC");
            ViewBag.MAHDN = new SelectList(db.HOADONNHAPs, "ID", "SOHD", id);
            return View();
        }

        // POST: CTHDN/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,MAHDN,MACTKHO,MATHUOC,SOLUONG,DONGIA,MADVT,CHIETKHAU")] CHITIETHDN cHITIETHDN)
        {
            if (ModelState.IsValid)
            {
                db.CHITIETHDNs.Add(cHITIETHDN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MACTKHO = new SelectList(db.CTKHOes, "ID", "MAKHO", cHITIETHDN.MACTKHO);
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
            ViewBag.MACTKHO = new SelectList(db.CTKHOes, "ID", "KE", cHITIETHDN.MACTKHO);
            ViewBag.MADVT = new SelectList(db.DONVITINHs, "ID", "TENDVT", cHITIETHDN.MADVT);
            ViewBag.MAHDN = new SelectList(db.HOADONNHAPs, "ID", "SOHD", cHITIETHDN.MAHDN);
            return View(cHITIETHDN);
        }

        // POST: CTHDN/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,MAHDN,MACTKHO,MATHUOC,SOLUONG,DONGIA,MADVT,CHIETKHAU")] CHITIETHDN cHITIETHDN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cHITIETHDN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MACTKHO = new SelectList(db.CTKHOes, "ID", "KE", cHITIETHDN.MACTKHO);
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
