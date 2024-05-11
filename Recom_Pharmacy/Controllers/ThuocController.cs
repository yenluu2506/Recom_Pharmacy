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
using System.Data.Entity.Validation;

namespace Recom_Pharmacy.Controllers
{
    public class ThuocController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: Thuoc
        public ActionResult Index(string Searchtext, int? page, int? SelectedLT, int? SelectedNCC)
        {
            ViewBag.LoaiThuoc = new SelectList(db.LOAITHUOCs.ToList(), "ID", "TENLOAI");
            ViewBag.NCC = new SelectList(db.NCCs.ToList(), "ID", "TENNCC");
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<THUOC> items = db.THUOCs.OrderByDescending(x => x.ID);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.TENTHUOC).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.TENTHUOC).Contains(searchKeyword) ||
                                         x.TENTHUOC.Contains(Searchtext));

            }
            if (SelectedLT.HasValue)
            {
                items = items.Where(x => x.LOAITHUOC.ID == SelectedLT.Value);
            }
            if (SelectedNCC.HasValue)
            {
                items = items.Where(x => x.NCC.ID == SelectedNCC.Value);
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedLT = SelectedLT;
            ViewBag.SelectedNCC = SelectedNCC;
            ViewBag.page = page;
            return View(items);
        }
        //// GET: Thuoc/Create
        public ActionResult Add()
        {
            ViewBag.MALOAI = new SelectList(db.LOAITHUOCs, "ID", "TENLOAI");
            ViewBag.MANCC = new SelectList(db.NCCs, "ID", "TENNCC");
            return View();
        }

        // POST: Thuoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,ANH,MALOAI,MADVT,MANCC,TENTHUOC,HINHTHUC,DONGGOI,NGAYSX,HSD,NHASX,NUOCSX,DOITUONGSD,CONGDUNG,GIANHAP,GIABAN,SOLUONG,TRANGTHAI")] THUOC tHUOC)
        {
            if (ModelState.IsValid)
            {
                
                try
                {
                    tHUOC.SOLUONG = 0;
                    db.THUOCs.Add(tHUOC);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Console.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }
            }

            ViewBag.MALOAI = new SelectList(db.LOAITHUOCs, "ID", "TENLOAI", tHUOC.MALOAI);
            ViewBag.MANCC = new SelectList(db.NCCs, "ID", "TENNCC", tHUOC.MANCC);
            return View(tHUOC);
        }

        // GET: Thuoc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THUOC tHUOC = db.THUOCs.Find(id);
            if (tHUOC == null)
            {
                return HttpNotFound();
            }
            ViewBag.MALOAI = new SelectList(db.LOAITHUOCs, "ID", "TENLOAI", tHUOC.MALOAI);
            ViewBag.MANCC = new SelectList(db.NCCs, "ID", "TENNCC", tHUOC.MANCC);
            return View(tHUOC);
        }

        // POST: Thuoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ANH,MALOAI,MADVT,MANCC,TENTHUOC,HINHTHUC,DONGGOI,NGAYSX,HSD,NHASX,NUOCSX,DOITUONGSD,CONGDUNG,GIANHAP,GIABAN,SOLUONG,TRANGTHAI")] THUOC tHUOC)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(tHUOC).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Console.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }

                
            }
            ViewBag.MALOAI = new SelectList(db.LOAITHUOCs, "ID", "TENLOAI", tHUOC.MALOAI);
            ViewBag.MANCC = new SelectList(db.NCCs, "ID", "TENNCC", tHUOC.MANCC);
            return View(tHUOC);
        }

        // GET: Thuoc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THUOC tHUOC = db.THUOCs.Find(id);
            if (tHUOC == null)
            {
                return HttpNotFound();
            }
            return View(tHUOC);
        }

        // POST: Thuoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            THUOC tHUOC = db.THUOCs.Find(id);
            db.THUOCs.Remove(tHUOC);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
