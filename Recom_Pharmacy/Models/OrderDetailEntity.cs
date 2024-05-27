using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Recom_Pharmacy.Models
{
    public class OrderDetailEntity
    {
        public int ID { get; set; }
        public Nullable<int> MAHDX { get; set; }
        public Nullable<int> MATHUOC { get; set; }
        public int SOLUONG { get; set; }
        public decimal DONGIA { get; set; }
        public Nullable<int> MADVT { get; set; }
        public Nullable<double> CHIETKHAU { get; set; }
        public Nullable<decimal> TONGTIEN { get; set; }

        public virtual DONVITINH DONVITINH { get; set; }
        public virtual THUOC THUOC { get; set; }
        public virtual HOADONXUAT HOADONXUAT { get; set; }
        public CHITIETHDX TypeOf_Order()
        {
            CHITIETHDX order = new CHITIETHDX();
            PropertyInfo[] pithis = typeof(OrderDetailEntity).GetProperties();
            PropertyInfo[] pieClinet = typeof(CHITIETHDX).GetProperties();
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

        public void TypeOf_OrderEntity(CHITIETHDX order)
        {

            PropertyInfo[] pithis = typeof(OrderDetailEntity).GetProperties();
            PropertyInfo[] pieClinet = typeof(CHITIETHDX).GetProperties();
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
        public OrderDetailEntity(CHITIETHDX order)
        {
            TypeOf_OrderEntity(order);

        }
        public OrderDetailEntity()
        {


        }
    }
}