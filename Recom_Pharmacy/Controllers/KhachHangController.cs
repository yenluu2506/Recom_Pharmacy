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
    public class KhachHangController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: KhachHang
        public ActionResult Index(string Searchtext, int? page, int? SelectedTT)
        {
            ViewBag.TinhThanh = new SelectList(db.TINHTHANHs.ToList(), "ID", "TENTINH");
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<KHACHHANG> items = db.KHACHHANGs.OrderByDescending(x => x.ID);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.TENKH).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TENKH).Contains(searchKeyword) ||
                                         x.TENKH.Contains(Searchtext));

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


        // GET: KhachHang/Create
        public ActionResult Add()
        {
            ViewBag.MATINH = new SelectList(db.TINHTHANHs, "ID", "TENTINH");
            return View();
        }

        // POST: KhachHang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,TENKH,SDT,GIOITINH,NGAYSINH,MATINH,DIACHI,Username,Passwords,Picture")] KHACHHANG kHACHHANG)
        {
            if (ModelState.IsValid)
            {
                db.KHACHHANGs.Add(kHACHHANG);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MATINH = new SelectList(db.TINHTHANHs, "ID", "TENTINH", kHACHHANG.MATINH);
            return View(kHACHHANG);
        }

        // GET: KhachHang/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHACHHANG kHACHHANG = db.KHACHHANGs.Find(id);
            if (kHACHHANG == null)
            {
                return HttpNotFound();
            }
            ViewBag.MATINH = new SelectList(db.TINHTHANHs, "ID", "TENTINH", kHACHHANG.MATINH);
            return View(kHACHHANG);
        }

        // POST: KhachHang/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TENKH,SDT,GIOITINH,NGAYSINH,MATINH,DIACHI,Username,Passwords,Picture")] KHACHHANG kHACHHANG)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kHACHHANG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MATINH = new SelectList(db.TINHTHANHs, "ID", "TENTINH", kHACHHANG.MATINH);
            return View(kHACHHANG);
        }

        // GET: KhachHang/Delete/5
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
