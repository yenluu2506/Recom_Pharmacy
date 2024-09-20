using PagedList;
using Recom_Pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Vonage.Common.Monads;
using System.Data.Entity;
using Filter = Recom_Pharmacy.Models.Common.Filter;
using Microsoft.Ajax.Utilities;
using Recom_Pharmacy.Models.Common;
using System.Web.Helpers;
using System.Data.Entity.Migrations;

namespace Recom_Pharmacy.Controllers
{
    public class HoaDonNhapController : Controller
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();
        private tbl_SYS_SEQUENCE _sequence = new tbl_SYS_SEQUENCE();
        private SYS_SEQUENCE _seq = new SYS_SEQUENCE();

        // GET: HoaDonNhap  
        public ActionResult Index(string Searchtext, int? page, int? SelectedNV, int? SelectedNCC)
        {
            ViewBag.NhanVien = new SelectList(db.TRINHDUOCVIENs.ToList(), "ID", "TENNV");
            ViewBag.NCC = new SelectList(db.NCCs.ToList(), "ID", "TENNCC");
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
            if (SelectedNCC.HasValue)
            {
                items = items.Where(x => x.NCC.ID == SelectedNCC.Value);
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedNV = SelectedNV;
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

        // GET: CTHDN/Create
        public ActionResult Add()
        {
            ViewBag.MATDV = new SelectList(db.TRINHDUOCVIENs, "ID", "TENNV");
            ViewBag.MANCC = new SelectList(db.NCCs, "ID", "TENNCC");
            ViewBag.MAKHO = new SelectList(db.KHOes, "ID", "TENKHO");
            _seq = _sequence.getItem(Constant.maphieunhap);
            if(_seq == null)
            {
                _seq.SEQNAME = Constant.maphieunhap;
                _seq.SEQVALUE = 1;
                _sequence.add(_seq);
                TempData["SOHD"] = _seq.SEQVALUE.Value.ToString("000000") + @"/" + DateTime.Today.Year.ToString().Substring(2, 2) + @"/NHH";
            }
            else
            {
            TempData["SOHD"] = (_seq.SEQVALUE + 1).Value.ToString("000000") + @"/" + DateTime.Today.Year.ToString().Substring(2, 2) + @"/NHH";
                }
            return View();
        }

        // POST: CTHDN/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult Add(HoaDonNhapViewModel hoadon)
        {
            if (hoadon != null)
            {
                try
                {
                    HOADONNHAP hdn = new HOADONNHAP();
                    hdn = hoadon.hoadonnhap;
                    db.HOADONNHAPs.Add(hdn);
                    foreach (var item in hoadon.chitiet)
                    {
                        var cTietTonKho = db.CTTONKHOes.FirstOrDefault(a => a.MATHUOC == item.ID && a.LOSX == item.LOSX);
                        if(cTietTonKho != null)
                        {
                            return Json(new { success = false,errMessage="Lô sản xuất đã tồn tại (" + cTietTonKho.LOSX +"), Ngày sx: (" + cTietTonKho.NGAYSX+")" });
                        }
                        CHITIETHDN chiTietHdn = new CHITIETHDN();
                        chiTietHdn.MAHDN = hdn.ID;
                        chiTietHdn.MATHUOC = item.ID;
                        chiTietHdn.SOLUONG = item.SOLUONG;
                        chiTietHdn.DONGIA = item.GIANHAP;
                        chiTietHdn.MADVT = item.MADVT;
                        chiTietHdn.LOSX = item.LOSX;
                        db.CHITIETHDNs.Add(chiTietHdn);
                        
                        CTTONKHO chiTietTonKho = new CTTONKHO();
                        chiTietTonKho.MAKHO = (int)hdn.MAKHO;
                        chiTietTonKho.DVT = item.MADVT;
                        chiTietTonKho.MATHUOC = item.ID;
                        chiTietTonKho.KE = item.VITRI;
                        chiTietTonKho.LOSX = item.LOSX;
                        chiTietTonKho.NGAYSX = item.NGAYSX;
                        chiTietTonKho.NGAYHH = item.NGAYHH;
                        chiTietTonKho.SLTON = item.SOLUONG;
                        chiTietTonKho.TRANGTHAI = true;
                        chiTietTonKho.GIANHAP = item.GIANHAP;
                        chiTietTonKho.GIABAN = item.GIABAN;
                        db.CTTONKHOes.Add(chiTietTonKho);
                    }

                    db.SaveChanges();
                    _seq = _sequence.getItem(Constant.maphieunhap);
                    if (_seq != null)
                    {
                        _seq.SEQNAME = Constant.maphieunhap;
                         _sequence.update(_seq);
                    }
                }
              catch(Exception ex)
                {
                    return Json(new { success = false,errMessage=ex.Message});
                }
                return Json(new { success = true,redirectURL="/hoadonnhap/index" });
            }
            return Json(new { success = false });
        }
        public ActionResult getAllThuoc(string searchTerm,int pageSize, int pageNum)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var key = (searchTerm?.ToLower().Trim()) ?? "";
            var thuoc = db.THUOCs.Where(p =>p.TENTHUOC.ToLower().Contains(key)).ToList();
            var result = new
            {
                Total = thuoc.Count(),
                thuoc = thuoc.Skip((pageNum * pageSize) - 5).Take(pageSize)
            };
            return new JsonResult
            {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpGet]
        public JsonResult getDVTByIdThuoc(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var dvt = db.QUYDOIDVs
                .Where(q => q.MATHUOC == id)
                .Select(q => q.DONVITINH)
                .Distinct() // Đảm bảo không có bản ghi trùng lặp
                .ToList();
            return Json(new { success = true, dvtData = dvt}, JsonRequestBehavior.AllowGet);
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
                        var id = Convert.ToInt32(item);
                        var obj = db.HOADONNHAPs.Include(i => i.CHITIETHDNs).FirstOrDefault(x => x.ID == id);
                        obj.TRANGTHAI = false;
                        foreach (CHITIETHDN chitiet in obj.CHITIETHDNs)
                        {
                            var tonkho = db.CTTONKHOes.FirstOrDefault(a=>a.LOSX == chitiet.LOSX && a.MATHUOC==chitiet.MATHUOC);
                            if (tonkho != null)
                            {
                                tonkho.TRANGTHAI = false;
                                db.CTTONKHOes.AddOrUpdate(tonkho);
                            }

                        }
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}