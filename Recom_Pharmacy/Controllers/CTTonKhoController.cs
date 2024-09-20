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
using System.Drawing.Printing;
using Recom_Pharmacy.Models.Common;
using Twilio.Base;
using static System.Net.Mime.MediaTypeNames;

namespace Recom_Pharmacy.Controllers
{
    public class CTTonKhoController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: CTTonKho
        public ActionResult Index(string Searchtext, int? page, int? SelectedTonKho, int? SelectedThuoc, int? id)
        {
            ViewBag.TonKho = new SelectList(db.KHOes.ToList(), "ID");
            ViewBag.Thuoc = new SelectList(db.KHOes.ToList(), "ID", "TENTHUOC");
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<CTTONKHO> items = db.CTTONKHOes.OrderByDescending(x => x.ID).Where(x=> x.KHO.ID == id);
            if (SelectedTonKho.HasValue)
            {
                items = items.Where(x => x.KHO.ID == SelectedTonKho.Value);
            }
            if (SelectedThuoc.HasValue)
            {
                items = items.Where(x => x.THUOC.ID == SelectedThuoc.Value);
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedTonKho = SelectedTonKho;
            ViewBag.SelectedThuoc = SelectedThuoc;
            ViewBag.page = page;
            return View(items);
        }

        public ActionResult ThuocDenHan(string Searchtext, int? page, int? thoiHan)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            var options = Constant.selectThuocHetHan();
            var selectThoiHan = new SelectList(options,nameof(SelectItem.value),nameof(SelectItem.optionName),nameof(SelectItem.value));
            DateTime today = DateTime.Now;
            var currentDate = today.Date;
            DateTime? expireDate = null;
            IEnumerable<CTTONKHO> thuocDenHan = db.CTTONKHOes.AsQueryable();
            if (thoiHan.HasValue)
            {
                expireDate = today.AddMonths(thoiHan.Value).Date;

                if (expireDate != null && thoiHan != 0)
                {
                    thuocDenHan = thuocDenHan.Where(t => t.NGAYHH <= expireDate && t.NGAYHH > currentDate);
                }
                else
                {
                    thuocDenHan = thuocDenHan.Where(t => t.NGAYHH <= currentDate);
                }
            }
            if (!string.IsNullOrEmpty(Searchtext))
            {
                thuocDenHan = thuocDenHan.Where(t => t.THUOC.TENTHUOC.Contains(Searchtext) || t.THUOC.TENCT.Contains(Searchtext)).ToList();
            }
            thuocDenHan = thuocDenHan.OrderByDescending(x => x.ID).ToList();
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            thuocDenHan = thuocDenHan.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.page = page;
            ViewBag.thoiHan = selectThoiHan;
            return View(thuocDenHan);
        }

        public ActionResult TonKhoLauNam(string Searchtext, int? page, int? thoiHan=1)
        {
            var pageSize = 5;
            if (page == null || page < 1)
            {
                page = 1;
            }
            var options = Constant.selectTonKhoLauNam();
            var selectThoiHan = new SelectList(options, nameof(SelectItem.value), nameof(SelectItem.optionName), nameof(SelectItem.value));
            DateTime today = DateTime.Now;
            var currentDate = today.Date;
            DateTime? fromDate = null;


            var viewModel = new TonKhoLauNamViewModel();
            var data = viewModel.getTonKhoLauNam(); // Retrieve data using your method

            if (thoiHan.HasValue)
            {
                fromDate = today.AddMonths(-thoiHan.Value).Date;

                if (fromDate != null)
                {
                    data = data.Where(t => t.NGAYNHAP <= currentDate&& t.NGAYNHAP >= fromDate); //fromDate <= ngaynhap <= currentDate
                }
            }
            if (!string.IsNullOrEmpty(Searchtext))
            {
                data = data.Where(t => t.TENTHUOC.Contains(Searchtext) || t.TENCT.Contains(Searchtext));
            }

            // Pagination
            var pageIndex = page ?? 1;
            var pagedTonKhoLauNam = data.OrderBy(x=>x.DABAN).ToPagedList(pageIndex, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.page = page;
            ViewBag.thoiHan = selectThoiHan;
            return View(pagedTonKhoLauNam);
        }


        // GET: CTTonKho/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTTONKHO cTTONKHO = db.CTTONKHOes.Find(id);
            if (cTTONKHO == null)
            {
                return HttpNotFound();
            }
            return View(cTTONKHO);
        }

        // GET: CTTonKho/Create
        public ActionResult Add(int? id)
        {
            ViewBag.MATONKHO = new SelectList(db.KHOes, "ID", "ID");
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC", id);
            return View();
        }

        // POST: CTTonKho/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,MATHUOC,MATONKHO,SLTON,TRANGTHAI")] CTTONKHO cTTONKHO)
        {
            if (ModelState.IsValid)
            {
                cTTONKHO.SLTON = 0;
                db.CTTONKHOes.Add(cTTONKHO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MATONKHO = new SelectList(db.KHOes, "ID", "ID", cTTONKHO.MAKHO);
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC", cTTONKHO.MATHUOC);
            return View(cTTONKHO);
        }

        // GET: CTTonKho/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTTONKHO cTTONKHO = db.CTTONKHOes.Find(id);
            if (cTTONKHO == null)
            {
                return HttpNotFound();
            }
            ViewBag.MATONKHO = new SelectList(db.KHOes, "ID", "ID", cTTONKHO.MAKHO);
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC", cTTONKHO.MATHUOC);
            return View(cTTONKHO);
        }

        // POST: CTTonKho/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,MATHUOC,MATONKHO,SLTON,TRANGTHAI")] CTTONKHO cTTONKHO)
        {
            if (ModelState.IsValid)
            {
                cTTONKHO.SLTON = 0;
                db.Entry(cTTONKHO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MATONKHO = new SelectList(db.KHOes, "ID", "ID", cTTONKHO.MAKHO);
            ViewBag.MATHUOC = new SelectList(db.THUOCs, "ID", "TENTHUOC", cTTONKHO.MATHUOC);
            return View(cTTONKHO);
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.CTTONKHOes.Find(id);
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
            var item = db.CTTONKHOes.Find(id);
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
                        var obj = db.CTTONKHOes.Find(Convert.ToInt32(item));
                        obj.TRANGTHAI = false;
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
