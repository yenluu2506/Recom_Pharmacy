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
    public class CTTonKhoController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: CTTonKho
        public ActionResult Index(string Searchtext, int? page, int? SelectedTonKho, int? SelectedThuoc, int? id)
        {
            ViewBag.TonKho = new SelectList(db.TONKHOes.ToList(), "ID");
            ViewBag.Thuoc = new SelectList(db.TONKHOes.ToList(), "ID", "TENTHUOC");
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<CTTONKHO> items = db.CTTONKHOes.OrderByDescending(x => x.ID).Where(x=> x.TONKHO.ID == id);
            if (SelectedTonKho.HasValue)
            {
                items = items.Where(x => x.TONKHO.ID == SelectedTonKho.Value);
            }
            if (SelectedThuoc.HasValue)
            {
                items = items.Where(x => x.THUOC.ID == SelectedThuoc.Value);
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedTonKho = SelectedTonKho;
            ViewBag.SelectedThuoc = SelectedThuoc;
            ViewBag.page = page;
            return View(items);
        }

        // GET: CTTonKho/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTTONKHO cTTONKHO = db.CTTONKHOes.Find(id);
            if (cTTONKHO == null)
            {
                return HttpNotFound();
            }
            return View(cTTONKHO);
        }

        // GET: CTTonKho/Create
        public ActionResult Add(int? id)
        {
            ViewBag.MATONKHO = new SelectList(db.TONKHOes, "ID", "ID");
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC", id);
            return View();
        }

        // POST: CTTonKho/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,MATHUOC,MATONKHO,SLTON,TRANGTHAI")] CTTONKHO cTTONKHO)
        {
            if (ModelState.IsValid)
            {
                cTTONKHO.SLTON = 0;
                db.CTTONKHOes.Add(cTTONKHO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MATONKHO = new SelectList(db.TONKHOes, "ID", "ID", cTTONKHO.MATONKHO);
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC", cTTONKHO.MATHUOC);
            return View(cTTONKHO);
        }

        // GET: CTTonKho/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTTONKHO cTTONKHO = db.CTTONKHOes.Find(id);
            if (cTTONKHO == null)
            {
                return HttpNotFound();
            }
            ViewBag.MATONKHO = new SelectList(db.TONKHOes, "ID", "ID", cTTONKHO.MATONKHO);
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC", cTTONKHO.MATHUOC);
            return View(cTTONKHO);
        }

        // POST: CTTonKho/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,MATHUOC,MATONKHO,SLTON,TRANGTHAI")] CTTONKHO cTTONKHO)
        {
            if (ModelState.IsValid)
            {
                cTTONKHO.SLTON = 0;
                db.Entry(cTTONKHO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MATONKHO = new SelectList(db.TONKHOes, "ID", "ID", cTTONKHO.MATONKHO);
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC", cTTONKHO.MATHUOC);
            return View(cTTONKHO);
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.CTTONKHOes.Find(id);
            if (item != null)
            {
                item.TRANGTHAI = !item.TRANGTHAI;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isAcive = item.TRANGTHAI });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.CTTONKHOes.Find(id);
            if (item != null)
            {
                db.CTTONKHOes.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.CTTONKHOes.Find(Convert.ToInt32(item));
                        db.CTTONKHOes.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
