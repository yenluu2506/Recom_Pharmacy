using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Recom_Pharmacy.Models;
using PagedList;
using Filter = Recom_Pharmacy.Models.Common.Filter;

namespace Recom_Pharmacy.Controllers
{
    public class MENUTHUOCController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: MENUTHUOC
        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 15;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<MENUTHUOC> items = db.MENUTHUOCs.OrderByDescending(x => x.ID);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.TENDM).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TENDM).Contains(searchKeyword) ||
                                         x.TENDM.Contains(Searchtext));

            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.page = page;
            return View(items);
        }

        // GET: MENUTHUOC/Create
        public ActionResult Add()
        {
            return View();
        }

        // POST: MENUTHUOC/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,TENDM,MOTA")] MENUTHUOC mENUTHUOC)
        {
            if (ModelState.IsValid)
            {
                db.MENUTHUOCs.Add(mENUTHUOC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mENUTHUOC);
        }

        // GET: MENUTHUOC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MENUTHUOC mENUTHUOC = db.MENUTHUOCs.Find(id);
            if (mENUTHUOC == null)
            {
                return HttpNotFound();
            }
            return View(mENUTHUOC);
        }

        // POST: MENUTHUOC/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TENDM,MOTA")] MENUTHUOC mENUTHUOC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mENUTHUOC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mENUTHUOC);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.MENUTHUOCs.Find(id);
            if (item != null)
            {
                db.MENUTHUOCs.Remove(item);
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
                        var obj = db.MENUTHUOCs.Find(Convert.ToInt32(item));
                        db.MENUTHUOCs.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
