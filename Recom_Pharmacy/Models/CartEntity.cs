using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Recom_Pharmacy.Models
{
    public class CartEntity
    {
        RecomPharmacyEntities db = new RecomPharmacyEntities();
        ThuocViewModel tb_thuoc = new ThuocViewModel();
        public int IDThuoc { get; set; }
        public string tenThuoc { get; set; }
        public string Picture { get; set; }
        public Double giaBan { get; set; }
        public int madvt { get; set; }

        public string tendvt { get; set; }
        public int soLuong { get; set; }
        public int maxSoLuong { get; set; }
        public double tongGia
        {
            get { return giaBan * soLuong; }
        }
        public string losx { get; set; }
        public CartEntity(int id,string loThuoc)
        {
            int idTonKho = Convert.ToInt32(loThuoc);
            var list = tb_thuoc.getThuocWithTonKho("");
            var product = list.FirstOrDefault(t=>t.ID == id);
            var cTTONKHO = product.CTTONKHOes.FirstOrDefault(t => t.ID == idTonKho);
            double giaThuoc = (double)cTTONKHO.GIABAN;
            string getLoSx = cTTONKHO.LOSX;
            IDThuoc = id;
            tenThuoc = product.TENTHUOC;
            Picture = product.ANH;
            giaBan = giaThuoc;
            losx = getLoSx;
            soLuong = 1;
            madvt = cTTONKHO.DONVITINH.ID;
            tendvt = cTTONKHO.DONVITINH.TENDVT;
        }
        public List<CartEntity> Items { get; set; }
        public CartEntity()
        {
            this.Items = new List<CartEntity>();
        }
    }
}