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
    public class LoaiThuocController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: LoaiThuoc
        public ActionResult Index(string Searchtext, int? page, int? SelectedDM)
        {
            ViewBag.DMThuoc = new SelectList(db.MENUTHUOCs.ToList(), "ID", "TENDM");
            var pageSize = 30;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<LOAITHUOC> items = db.LOAITHUOCs.OrderByDescending(x => x.ID);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.TENLOAI).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TENLOAI).Contains(searchKeyword) ||
                                         x.TENLOAI.Contains(Searchtext));

            }
            if (SelectedDM.HasValue)
            {
                items = items.Where(x => x.MENUTHUOC.ID == SelectedDM.Value);
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedDM = SelectedDM;
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

        public ActionResult GetPhongChieuByRap(int rapId)
        {
            var loaiThuocList = db.LOAITHUOCs.Where(x => x.MENUTHUOC.ID == rapId).ToList();
            return PartialView("_LoaiThuocList", loaiThuocList);
        }

        // GET: LoaiThuoc/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOAITHUOC lOAITHUOC = db.LOAITHUOCs.Find(id);
            if (lOAITHUOC == null)
            {
                return HttpNotFound();
            }
            return View(lOAITHUOC);
        }

        // GET: LoaiThuoc/Create
        public ActionResult Add()
        {
            ViewBag.MAMENU = new SelectList(db.MENUTHUOCs, "ID", "TENDM");
            return View();
        }

        // POST: LoaiThuoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,MAMENU,TENLOAI,MOTA")] LOAITHUOC lOAITHUOC)
        {
            if (ModelState.IsValid)
            {
                db.LOAITHUOCs.Add(lOAITHUOC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MAMENU = new SelectList(db.MENUTHUOCs, "ID", "TENDM", lOAITHUOC.MAMENU);
            return View(lOAITHUOC);
        }

        // GET: LoaiThuoc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOAITHUOC lOAITHUOC = db.LOAITHUOCs.Find(id);
            if (lOAITHUOC == null)
            {
                return HttpNotFound();
            }
            ViewBag.MAMENU = new SelectList(db.MENUTHUOCs, "ID", "TENDM", lOAITHUOC.MAMENU);
            return View(lOAITHUOC);
        }

        // POST: LoaiThuoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,MAMENU,TENLOAI,MOTA")] LOAITHUOC lOAITHUOC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lOAITHUOC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MAMENU = new SelectList(db.MENUTHUOCs, "ID", "TENDM", lOAITHUOC.MAMENU);
            return View(lOAITHUOC);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.LOAITHUOCs.Find(id);
            if (item != null)
            {
                db.LOAITHUOCs.Remove(item);
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
                        var obj = db.LOAITHUOCs.Find(Convert.ToInt32(item));
                        db.LOAITHUOCs.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
