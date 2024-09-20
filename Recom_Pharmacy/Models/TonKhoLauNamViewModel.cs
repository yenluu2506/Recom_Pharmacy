using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
namespace Recom_Pharmacy.Models
{

    public class TonKhoLauNamViewModel
    {
        private RecomPharmacyEntities db = new RecomPharmacyEntities();
        public string TENDM { get; set; }
        public string TENLOAI { get; set; }
        public DateTime NGAYNHAP { get; set; }
        public int MATHUOC { get; set; }
        public string TENTHUOC { get; set; }
        public string TENCT { get; set; }
        public string TENDVT { get; set; }
        public string LOSX { get; set; }
        public DateTime NGAYSX { get; set; }
        public DateTime NGAYHH { get; set; }
        public string KE { get; set; }
        public int? SONGAYTON { get; set; }
        public int SLTON { get; set; }
        public int? DABAN { get; set; }
        public IQueryable<TonKhoLauNamViewModel> getTonKhoLauNam()
        {
            DateTime today = DateTime.Now;

            var query = from nhap in
                            (from hdn in db.HOADONNHAPs
                             join cthdn in db.CHITIETHDNs on hdn.ID equals cthdn.MAHDN
                             join thuoc in db.THUOCs on cthdn.MATHUOC equals thuoc.ID
                             join donViTinh in db.DONVITINHs on cthdn.MADVT equals donViTinh.ID
                             join ctTonKho in db.CTTONKHOes on new { cthdn.MATHUOC, cthdn.LOSX } equals new { ctTonKho.MATHUOC, ctTonKho.LOSX }
                             join loaiThuoc in db.LOAITHUOCs on thuoc.MALOAI equals loaiThuoc.ID
                             join menuThuoc in db.MENUTHUOCs on loaiThuoc.MAMENU equals menuThuoc.ID
                             select new
                             {
                                 hdn.NGAYNHAP,
                                 cthdn.MATHUOC,
                                 thuoc.TENTHUOC,
                                 thuoc.TENCT,
                                 cthdn.MADVT,
                                 donViTinh.TENDVT,
                                 ctTonKho.LOSX,
                                 ctTonKho.NGAYSX,
                                 ctTonKho.NGAYHH,
                                 ctTonKho.SLTON,
                                 ctTonKho.KE,
                                 SONGAYTON = DbFunctions.DiffDays(hdn.NGAYNHAP, today),
                                 menuThuoc.TENDM,
                                 loaiThuoc.TENLOAI
                             })
                        join xuat in
                            (from hdx in db.HOADONXUATs
                             join cthdx in db.CHITIETHDXes on hdx.ID equals cthdx.MAHDX
                             join donViTinh in db.DONVITINHs on cthdx.MADVT equals donViTinh.ID
                             join ctTonKho in db.CTTONKHOes on new { cthdx.MATHUOC, cthdx.LOSX, cthdx.MADVT } equals new { ctTonKho.MATHUOC, ctTonKho.LOSX, MADVT=ctTonKho.DVT }
                             group cthdx by new { cthdx.MATHUOC, cthdx.MADVT, cthdx.LOSX } into g
                             select new
                             {
                                 g.Key.MATHUOC,
                                 g.Key.MADVT,
                                 g.Key.LOSX,
                                 DABAN = g.Sum(x => x.SOLUONG)
                             })
                        on new { nhap.MATHUOC, nhap.MADVT, nhap.LOSX } equals new { xuat.MATHUOC, xuat.MADVT, xuat.LOSX } into gj
                        from xuat in gj.DefaultIfEmpty()
                        select new TonKhoLauNamViewModel()
                        {
                            TENDM = nhap.TENDM,
                            TENLOAI = nhap.TENLOAI,
                            NGAYNHAP = nhap.NGAYNHAP,
                            MATHUOC = nhap.MATHUOC,
                            TENTHUOC = nhap.TENTHUOC,
                            TENCT = nhap.TENCT,
                            TENDVT = nhap.TENDVT,
                            LOSX = nhap.LOSX,
                            NGAYSX = (DateTime)nhap.NGAYSX,
                            NGAYHH = (DateTime)nhap.NGAYHH,
                            KE = nhap.KE,
                            SONGAYTON = (int)nhap.SONGAYTON,
                            SLTON = (int)nhap.SLTON,
                            DABAN = xuat.DABAN
                        };

            return query;
        }

    }
}
