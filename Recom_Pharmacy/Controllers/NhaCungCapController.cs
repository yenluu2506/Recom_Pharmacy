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
using Filter = Recom_Pharmacy.Models.Common.Filter;

namespace Recom_Pharmacy.Controllers
{
    public class NhaCungCapController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: NhaCungCap
        public ActionResult Index(string Searchtext, int? page, int? SelectedQG)
        {
            ViewBag.QuocGia = new SelectList(db.QUOCGIAs.ToList(), "ID", "TENQG");
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<NCC> items = db.NCCs.OrderByDescending(x => x.ID);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.TENNCC).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TENNCC).Contains(searchKeyword) ||
                                         x.TENNCC.Contains(Searchtext));

            }
            if (SelectedQG.HasValue)
            {
                items = items.Where(x => x.QUOCGIA.ID == SelectedQG.Value);
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedQG = SelectedQG;
            ViewBag.page = page;
            return View(items);
        }

        // GET: NhaCungCap/Create
        public ActionResult Add()
        {
            ViewBag.MAQUOCGIA = new SelectList(db.QUOCGIAs, "ID", "TENQG");
            return View();
        }

        // POST: NhaCungCap/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,TENNCC,DIACHI,SDT,EMAIL,MAQUOCGIA,TRANGTHAI")] NCC nCC)
        {
            if (ModelState.IsValid)
            {
                db.NCCs.Add(nCC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MAQUOCGIA = new SelectList(db.QUOCGIAs, "ID", "TENQG", nCC.MAQUOCGIA);
            return View(nCC);
        }

        // GET: NhaCungCap/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NCC nCC = db.NCCs.Find(id);
            if (nCC == null)
            {
                return HttpNotFound();
            }
            ViewBag.MAQUOCGIA = new SelectList(db.QUOCGIAs, "ID", "TENQG", nCC.MAQUOCGIA);
            return View(nCC);
        }

        // POST: NhaCungCap/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TENNCC,DIACHI,SDT,EMAIL,MAQUOCGIA,TRANGTHAI")] NCC nCC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nCC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MAQUOCGIA = new SelectList(db.QUOCGIAs, "ID", "TENQG", nCC.MAQUOCGIA);
            return View(nCC);
        }
        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.NCCs.Find(id);
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
            var item = db.NCCs.Find(id);
            if (item != null)
            {
                db.NCCs.Remove(item);
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
                        var obj = db.NCCs.Find(Convert.ToInt32(item));
                        db.NCCs.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
