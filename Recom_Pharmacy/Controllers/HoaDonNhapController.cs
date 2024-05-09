using PagedList;
using Recom_Pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Filter = Recom_Pharmacy.Models.Common.Filter;

namespace Recom_Pharmacy.Controllers
{
    public class HoaDonNhapController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();

        // GET: HoaDonNhap
        public ActionResult Index(string Searchtext, int? page, int? SelectedNV)
        {
            ViewBag.NhanVien = new SelectList(db.TRINHDUOCVIENs.ToList(), "ID", "TENNV");
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<HOADONNHAP> items = db.HOADONNHAPs.OrderByDescending(x => x.ID);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchKeyword = Filter.ChuyenCoDauThanhKhongDau(Searchtext);
                items = items.Where(x => Filter.ChuyenCoDauThanhKhongDau(x.SOHD).StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                         Filter.ChuyenCoDauThanhKhongDau(x.SOHD).Contains(searchKeyword) ||
                                         x.SOHD.Contains(Searchtext));

            }
            if (SelectedNV.HasValue)
            {
                items = items.Where(x => x.TRINHDUOCVIEN.ID == SelectedNV.Value);
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedNV = SelectedNV;
            ViewBag.page = page;
            return View(items);
        }

        // GET: CTHDN/Create
        public ActionResult Add()
        {
            ViewBag.MATDV = new SelectList(db.TRINHDUOCVIENs, "ID", "TENNV");
            return View();
        }

        // POST: CTHDN/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,SOHD,MATDV,NGAYNHAP,TONGTIEN,MALOAITIEN,VAT,TIENNO,GHICHU,TRANGTHAI")] HOADONNHAP hONNHAP)
        {
            if (ModelState.IsValid)
            {
                db.HOADONNHAPs.Add(hONNHAP);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MATDV = new SelectList(db.TRINHDUOCVIENs, "ID", "TENNV", hONNHAP.MATDV);
            return View(hONNHAP);
        }
    }
}