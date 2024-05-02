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
using System.Web.UI;

namespace Recom_Pharmacy.Controllers
{
    public class TinhThanhController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: TinhThanh
        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<TINHTHANH> items = db.TINHTHANHs.OrderByDescending(x => x.ID);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.TENTINH).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TENTINH).Contains(searchKeyword) ||
                                         x.TENTINH.Contains(Searchtext));

            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.page = page;
            return View(items);
        }

        // GET: TinhThanh/Create
        public ActionResult Add()
        {
            return View();
        }

        // POST: TinhThanh/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,TENTINH")] TINHTHANH tINHTHANH)
        {
            if (ModelState.IsValid)
            {
                db.TINHTHANHs.Add(tINHTHANH);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tINHTHANH);
        }

        // GET: TinhThanh/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TINHTHANH tINHTHANH = db.TINHTHANHs.Find(id);
            if (tINHTHANH == null)
            {
                return HttpNotFound();
            }
            return View(tINHTHANH);
        }

        // POST: TinhThanh/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TENTINH")] TINHTHANH tINHTHANH)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tINHTHANH).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tINHTHANH);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.TINHTHANHs.Find(id);
            if (item != null)
            {
                db.TINHTHANHs.Remove(item);
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
                        var obj = db.TINHTHANHs.Find(Convert.ToInt32(item));
                        db.TINHTHANHs.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
