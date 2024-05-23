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
    public class TonKhoController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: TonKho
        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<TONKHO> items = db.TONKHOes.OrderByDescending(x => x.ID);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.TENKHO).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TENKHO).Contains(searchKeyword) ||
                                         x.TENKHO.Contains(Searchtext));

            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.page = page;
            return View(items);
        }

        // GET: TonKho/Create
        public ActionResult Add()
        {
            return View();
        }

        // POST: TonKho/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,TENKHO,NGAYLAP,DIACHI,SDT,TRANGTHAI")] TONKHO tONKHO)
        {
            if (ModelState.IsValid)
            {
                db.TONKHOes.Add(tONKHO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tONKHO);
        }

        // GET: TonKho/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TONKHO tONKHO = db.TONKHOes.Find(id);
            if (tONKHO == null)
            {
                return HttpNotFound();
            }
            return View(tONKHO);
        }

        // POST: TonKho/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TENKHO,NGAYLAP,DIACHI,SDT,TRANGTHAI")] TONKHO tONKHO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tONKHO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tONKHO);
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.TONKHOes.Find(id);
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
            var item = db.TONKHOes.Find(id);
            if (item != null)
            {
                db.TONKHOes.Remove(item);
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
                        var obj = db.TONKHOes.Find(Convert.ToInt32(item));
                        db.TONKHOes.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
