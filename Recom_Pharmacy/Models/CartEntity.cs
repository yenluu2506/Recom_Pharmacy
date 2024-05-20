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
        public int IDThuoc { get; set; }
        public string tenThuoc { get; set; }
        public string Picture { get; set; }
        public Double giaBan { get; set; }
        public int soLuong { get; set; }
        public int maxSoLuong { get; set; }
        public double tongGia
        {
            get { return giaBan * soLuong; }
        }
        public CartEntity(int id)
        {
            IDThuoc = id;
            THUOC product = db.THUOCs.Single(n => n.ID == IDThuoc);
            tenThuoc = product.TENTHUOC;
            Picture = product.ANH;
            giaBan = Double.Parse(product.GIABAN.ToString());
            soLuong = 1;
        }
    }
}