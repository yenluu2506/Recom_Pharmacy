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
    public class QuocGiaController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: QuocGia
        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<QUOCGIA> items = db.QUOCGIAs.OrderByDescending(x => x.ID);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.TENQG).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TENQG).Contains(searchKeyword) ||
                                         x.TENQG.Contains(Searchtext));

            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.page = page;
            var ac = (Admin)Session["Account"];
            if (ac == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View(items);
            }
        }

        // GET: QuocGia/Create
        public ActionResult Add()
        {
            return View();
        }

        // POST: QuocGia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,TENQG")] QUOCGIA qUOCGIA)
        {
            if (ModelState.IsValid)
            {
                db.QUOCGIAs.Add(qUOCGIA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(qUOCGIA);
        }

        // GET: QuocGia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QUOCGIA qUOCGIA = db.QUOCGIAs.Find(id);
            if (qUOCGIA == null)
            {
                return HttpNotFound();
            }
            return View(qUOCGIA);
        }

        // POST: QuocGia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TENQG")] QUOCGIA qUOCGIA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(qUOCGIA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(qUOCGIA);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.QUOCGIAs.Find(id);
            if (item != null)
            {
                db.QUOCGIAs.Remove(item);
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
                        var obj = db.QUOCGIAs.Find(Convert.ToInt32(item));
                        db.QUOCGIAs.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
