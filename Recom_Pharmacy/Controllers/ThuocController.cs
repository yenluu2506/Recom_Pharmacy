using PagedList;
using Recom_Pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Filter = Recom_Pharmacy.Models.Common.Filter;

namespace Recom_Pharmacy.Controllers
{
    public class ThuocController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: Thuoc
        public ActionResult Index(string Searchtext, int? page, int? SelectedLT, int? SelectedNCC)
        {
            ViewBag.LoaiThuoc = new SelectList(db.LOAITHUOCs.ToList(), "ID", "TENLOAI");
            var pageSize = 10;
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
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedLT = SelectedLT;
            ViewBag.SelectedNCC = SelectedNCC;
            ViewBag.page = page;
            //return View(items);
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
        //// GET: Thuoc/Create
        public ActionResult Add()
        {
            ViewBag.MALOAI = new SelectList(db.LOAITHUOCs, "ID", "TENLOAI");
            return View();
        }

        // POST: Thuoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,ANH,MALOAI,MADVT,TENTHUOC,TENCT,DONGGOI,NGAYSX,HSD,NHASX,NUOCSX,DOITUONGSD,CONGDUNG,MOTA,GIANHAP,GIABAN,SOLUONG,TRANGTHAI")] THUOC tHUOC, List<int> unitSelected, List<int> numberOfUnit)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    db.THUOCs.Add(tHUOC);
                    if (unitSelected != null && unitSelected.Count > 0 && numberOfUnit.Count > 0)
                    {
                        for (int i = 0; i < unitSelected.Count; i++)
                        {

                            db.QUYDOIDVs.Add(new QUYDOIDV
                            {
                                MATHUOC = tHUOC.ID,
                                MADVT = unitSelected[i],
                                TILEQUYDOI = numberOfUnit[i]
                            });

                        }
                    }
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
            ViewBag.DVT = new SelectList(db.QUYDOIDVs, "MATHUOC", "MADVT", "TILEQUYDOI", tHUOC.ID);
            return View(tHUOC);
        }

        // POST: Thuoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ANH,MALOAI,MADVT,TENTHUOC,TENCT,DONGGOI,NGAYSX,HSD,NHASX,NUOCSX,DOITUONGSD,CONGDUNG,MOTA,GIABAN,TRANGTHAI")] THUOC tHUOC)
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
            return View(tHUOC);
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.THUOCs.Find(id);
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
            var item = db.THUOCs.Find(id);
            if (item != null)
            {
                item.TRANGTHAI = false;
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
                        var obj = db.THUOCs.Find(Convert.ToInt32(item));
                        obj.TRANGTHAI = false;
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public ActionResult Partial_DVT()
        {
            ViewBag.dvt = new SelectList(db.DONVITINHs.ToList(), "ID", "TENDVT");
            return PartialView();
        }
    }
}
