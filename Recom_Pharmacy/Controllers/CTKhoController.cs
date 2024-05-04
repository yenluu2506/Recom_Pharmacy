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
    public class CTKhoController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: CTKho
        public ActionResult Index(string Searchtext, int? page, int? SelectedKho)
        {
            ViewBag.Kho = new SelectList(db.KHOes.ToList(), "ID", "TENKHO");
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<CTKHO> items = db.CTKHOes.OrderByDescending(x => x.ID);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.KE).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.KE).Contains(searchKeyword) ||
                                         x.KE.Contains(Searchtext));

            }
            if (SelectedKho.HasValue)
            {
                items = items.Where(x => x.KHO.ID == SelectedKho.Value);
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedKho = SelectedKho;
            ViewBag.page = page;
            return View(items);
        }

        // GET: CTKho/Create
        public ActionResult Add()
        {
            ViewBag.MAKHO = new SelectList(db.KHOes, "ID", "TENKHO");
            return View();
        }

        // POST: CTKho/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,KE,NGAN,TRANGTHAI,MAKHO")] CTKHO cTKHO)
        {
            if (ModelState.IsValid)
            {
                db.CTKHOes.Add(cTKHO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MAKHO = new SelectList(db.KHOes, "ID", "TENKHO", cTKHO.MAKHO);
            return View(cTKHO);
        }

        // GET: CTKho/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTKHO cTKHO = db.CTKHOes.Find(id);
            if (cTKHO == null)
            {
                return HttpNotFound();
            }
            ViewBag.MAKHO = new SelectList(db.KHOes, "ID", "TENKHO", cTKHO.MAKHO);
            return View(cTKHO);
        }

        // POST: CTKho/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,KE,NGAN,TRANGTHAI,MAKHO")] CTKHO cTKHO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cTKHO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MAKHO = new SelectList(db.KHOes, "ID", "TENKHO", cTKHO.MAKHO);
            return View(cTKHO);
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.CTKHOes.Find(id);
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
            var item = db.CTKHOes.Find(id);
            if (item != null)
            {
                db.CTKHOes.Remove(item);
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
                        var obj = db.CTKHOes.Find(Convert.ToInt32(item));
                        db.CTKHOes.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
