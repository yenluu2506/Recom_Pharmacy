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
    
    public partial class HOADONXUAT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HOADONXUAT()
        {
            this.CHITIETHDXes = new HashSet<CHITIETHDX>();
        }
    
        public int ID { get; set; }
        public Nullable<int> MAKH { get; set; }
        public System.DateTime NGAYXUAT { get; set; }
        public decimal TONGTIEN { get; set; }
        public Nullable<double> VAT { get; set; }
        public Nullable<System.DateTime> NGAYGIAOHANG { get; set; }
        public Nullable<bool> TTGIAOHANG { get; set; }
        public bool TRANGTHAI { get; set; }
        public Nullable<bool> XULY { get; set; }
        public string GHICHU { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIETHDX> CHITIETHDXes { get; set; }
        public virtual KHACHHANG KHACHHANG { get; set; }
    }
}
