//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Recom_Pharmacy.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CHITIETHDX
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
    }
}
