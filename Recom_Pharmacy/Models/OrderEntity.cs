using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Recom_Pharmacy.Models
{
    public class OrderEntity
    {
        public int ID { get; set; }
        public Nullable<int> MAKH { get; set; }
        public System.DateTime NGAYXUAT { get; set; }
        public decimal TONGTIEN { get; set; }
        public Nullable<System.DateTime> NGAYGIAOHANG { get; set; }
        public Nullable<bool> TTGIAOHANG { get; set; }
        public Nullable<bool> DATHANHTOAN { get; set; }
        public Nullable<bool> HOANTHANH { get; set; }

        public bool TRANGTHAI { get; set; }
        public string GHICHU { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIETHDX> CHITIETHDXes { get; set; }
        public virtual KHACHHANG KHACHHANG { get; set; }

        public HOADONXUAT TypeOf_Order()
        {
            HOADONXUAT order = new HOADONXUAT();
            PropertyInfo[] pithis = typeof(OrderEntity).GetProperties();
            PropertyInfo[] pieClinet = typeof(HOADONXUAT).GetProperties();
            foreach (var items in pithis)
            {
                foreach (var itempiem in pieClinet)
                {
                    if (itempiem.Name == items.Name)
                    {
                        itempiem.SetValue(order, items.GetValue(this));
                        break;
                    }
                }
            }
            return order;
        }

        // convert tu model sang view

        public void TypeOf_OrderEntity(HOADONXUAT order)
        {

            PropertyInfo[] pithis = typeof(OrderEntity).GetProperties();
            PropertyInfo[] pieClinet = typeof(HOADONXUAT).GetProperties();
            foreach (var items in pithis)
            {
                foreach (var itempiem in pieClinet)
                {
                    if (itempiem.Name == items.Name)
                    {
                        items.SetValue(this, itempiem.GetValue(order));
                        break;
                    }
                }
            }

        }
        //
        public OrderEntity(HOADONXUAT order)
        {
            TypeOf_OrderEntity(order);

        }
        public OrderEntity()
        {


        }
    }
}