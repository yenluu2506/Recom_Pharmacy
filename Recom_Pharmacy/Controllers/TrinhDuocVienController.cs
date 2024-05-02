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
    public class TrinhDuocVienController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: TrinhDuocVien
        public ActionResult Index(string Searchtext, int? page, int? SelectedTT)
        {
            ViewBag.TinhThanh = new SelectList(db.TINHTHANHs.ToList(), "ID", "TENTINH");
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<TRINHDUOCVIEN> items = db.TRINHDUOCVIENs.OrderByDescending(x => x.ID);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.TENNV).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TENNV).Contains(searchKeyword) ||
                                         x.TENNV.Contains(Searchtext));

            }
            if (SelectedTT.HasValue)
            {
                items = items.Where(x => x.TINHTHANH.ID == SelectedTT.Value);
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedTT = SelectedTT;
            ViewBag.page = page;
            return View(items);
        }

        // GET: TrinhDuocVien/Create
        public ActionResult Add()
        {
            ViewBag.MATINH = new SelectList(db.TINHTHANHs, "ID", "TENTINH");
            return View();
        }

        // POST: TrinhDuocVien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,MATINH,TENNV,GIOITINH,NGAYSINH,DIACHI,SDT")] TRINHDUOCVIEN tRINHDUOCVIEN)
        {
            if (ModelState.IsValid)
            {
                db.TRINHDUOCVIENs.Add(tRINHDUOCVIEN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MATINH = new SelectList(db.TINHTHANHs, "ID", "TENTINH", tRINHDUOCVIEN.MATINH);
            return View(tRINHDUOCVIEN);
        }

        // GET: TrinhDuocVien/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRINHDUOCVIEN tRINHDUOCVIEN = db.TRINHDUOCVIENs.Find(id);
            if (tRINHDUOCVIEN == null)
            {
                return HttpNotFound();
            }
            ViewBag.MATINH = new SelectList(db.TINHTHANHs, "ID", "TENTINH", tRINHDUOCVIEN.MATINH);
            return View(tRINHDUOCVIEN);
        }

        // POST: TrinhDuocVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,MATINH,TENNV,GIOITINH,NGAYSINH,DIACHI,SDT")] TRINHDUOCVIEN tRINHDUOCVIEN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tRINHDUOCVIEN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MATINH = new SelectList(db.TINHTHANHs, "ID", "TENTINH", tRINHDUOCVIEN.MATINH);
            return View(tRINHDUOCVIEN);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.TRINHDUOCVIENs.Find(id);
            if (item != null)
            {
                db.TRINHDUOCVIENs.Remove(item);
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
                        var obj = db.TRINHDUOCVIENs.Find(Convert.ToInt32(item));
                        db.TRINHDUOCVIENs.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
