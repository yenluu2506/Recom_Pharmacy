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
    
    public partial class THUOC
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public THUOC()
        {
            this.Carts = new HashSet<Cart>();
            this.CHITIETHDNs = new HashSet<CHITIETHDN>();
            this.CHITIETHDXes = new HashSet<CHITIETHDX>();
            this.CTTONKHOes = new HashSet<CTTONKHO>();
            this.QUYDOIDVs = new HashSet<QUYDOIDV>();
        }
    
        public int ID { get; set; }
        public string ANH { get; set; }
        public int MALOAI { get; set; }
        public string TENTHUOC { get; set; }
        public string TENCT { get; set; }
        public string DONGGOI { get; set; }
        public System.DateTime NGAYSX { get; set; }
        public System.DateTime HSD { get; set; }
        public string NHASX { get; set; }
        public string NUOCSX { get; set; }
        public string DOITUONGSD { get; set; }
        public string CONGDUNG { get; set; }
        public string MOTA { get; set; }
        public Nullable<decimal> GIABAN { get; set; }
        public bool TRANGTHAI { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIETHDN> CHITIETHDNs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIETHDX> CHITIETHDXes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CTTONKHO> CTTONKHOes { get; set; }
        public virtual LOAITHUOC LOAITHUOC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUYDOIDV> QUYDOIDVs { get; set; }
    }
}
