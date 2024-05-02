using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Recom_Pharmacy.Models;
using Filter = Recom_Pharmacy.Models.Common.Filter;

namespace Recom_Pharmacy.Controllers
{
    public class DonViTinhController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: DonViTinh
        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<DONVITINH> items = db.DONVITINHs.OrderByDescending(x => x.ID);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.TENDVT).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TENDVT).Contains(searchKeyword) ||
                                         x.TENDVT.Contains(Searchtext));

            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.page = page;
            return View(items);
        }

        // GET: DonViTinh/Create
        public ActionResult Add()
        {
            return View();
        }

        // POST: DonViTinh/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,TENDVT,TRANGTHAI")] DONVITINH dONVITINH)
        {
            if (ModelState.IsValid)
            {
                db.DONVITINHs.Add(dONVITINH);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dONVITINH);
        }

        // GET: DonViTinh/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONVITINH dONVITINH = db.DONVITINHs.Find(id);
            if (dONVITINH == null)
            {
                return HttpNotFound();
            }
            return View(dONVITINH);
        }

        // POST: DonViTinh/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TENDVT,TRANGTHAI")] DONVITINH dONVITINH)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dONVITINH).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dONVITINH);
        }
        public ActionResult IsActive(int id)
        {
            var item = db.DONVITINHs.Find(id);
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
            var item = db.DONVITINHs.Find(id);
            if (item != null)
            {
                db.DONVITINHs.Remove(item);
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
                        var obj = db.DONVITINHs.Find(Convert.ToInt32(item));
                        db.DONVITINHs.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
