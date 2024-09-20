using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Recom_Pharmacy.Models;

namespace Recom_Pharmacy.Controllers
{
    public class QUYDOIDVController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: QUYDOIDV
        public ActionResult Index(int? id)
        {
            var qUYDOIDVs = db.QUYDOIDVs.Include(q => q.DONVITINH).Include(q => q.THUOC).Where(q=>q.MATHUOC==id);
            return View(qUYDOIDVs.ToList());
        }

        // GET: QUYDOIDV/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QUYDOIDV qUYDOIDV = db.QUYDOIDVs.Find(id);
            if (qUYDOIDV == null)
            {
                return HttpNotFound();
            }
            return View(qUYDOIDV);
        }

        // GET: QUYDOIDV/Create
        public ActionResult Create()
        {
            ViewBag.MADVT = new SelectList(db.DONVITINHs, "ID", "TENDVT");
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC");
            return View();
        }

        // POST: QUYDOIDV/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,MATHUOC,MADVT,TILEQUYDOI,DONVIBANONL")] QUYDOIDV qUYDOIDV)
        {
            if (ModelState.IsValid)
            {
                db.QUYDOIDVs.Add(qUYDOIDV);
                db.SaveChanges();
                return Json(new { success = true,id= qUYDOIDV.ID });
            }
            return Json(new { success = false });

            //ViewBag.MADVT = new SelectList(db.DONVITINHs, "ID", "TENDVT", qUYDOIDV.MADVT);
            //ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "ANH", qUYDOIDV.MATHUOC);
            //return View(qUYDOIDV);
        }

        // GET: QUYDOIDV/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QUYDOIDV qUYDOIDV = db.QUYDOIDVs.Find(id);
            if (qUYDOIDV == null)
            {
                return HttpNotFound();
            }
            ViewBag.MADVT = new SelectList(db.DONVITINHs, "ID", "TENDVT", qUYDOIDV.MADVT);
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC", qUYDOIDV.MATHUOC);

            return View(qUYDOIDV);
        }

        // POST: QUYDOIDV/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,MATHUOC,MADVT,TILEQUYDOI")] QUYDOIDV qUYDOIDV)
        {
            if (ModelState.IsValid)
            {
                db.Entry(qUYDOIDV).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", "thuoc",new {id = qUYDOIDV.MATHUOC });
            }
            ViewBag.MADVT = new SelectList(db.DONVITINHs, "ID", "TENDVT", qUYDOIDV.MADVT);
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "ANH", qUYDOIDV.MATHUOC);
            return View(qUYDOIDV);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QUYDOIDV qUYDOIDV = db.QUYDOIDVs.Find(id);
            if (qUYDOIDV != null)
            {
                db.QUYDOIDVs.Remove(qUYDOIDV);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public ActionResult IsActive(int id)
        {
            var item = db.QUYDOIDVs.Find(id);
            if (item != null)
            {
                item.DONVIBANONL = !item.DONVIBANONL;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isAcive = item.DONVIBANONL });
            }
            return Json(new { success = false });
        }

        // POST: QUYDOIDV/Delete/5
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
