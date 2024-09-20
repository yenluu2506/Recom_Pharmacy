using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Recom_Pharmacy.Models
{
    public class HoaDonNhapViewModel
    {
        public HOADONNHAP hoadonnhap  { get; set; }
        public List <ChiTietNhapViewModel>  chitiet { get; set; }
    }

    public class ChiTietNhapViewModel
    {
        public int ID { get; set; }
        public int MADVT { get; set; }
        public string TENTHUOC { get; set; }
        public decimal GIANHAP { get; set; }
        public decimal GIABAN { get; set; }
        public int SOLUONG { get; set; }
        public double THANHTIEN { get; set; }
        public DateTime NGAYSX { get; set; }
        public DateTime NGAYHH { get; set; }

        public string LOSX { get; set; }
        public string VITRI { get; set; }

        public bool TRANGTHAI { get; set; }


        public virtual DONVITINH DONVITINH { get; set; }
        public virtual HOADONNHAP HOADONNHAP { get; set; }
        public virtual THUOC THUOC { get; set; }
    }
}