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
    using System.Web.Mvc;

    public partial class Blog
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> NGAYVIET { get; set; }
        public string TIEUDE { get; set; }
        [AllowHtml]
        public string MOTANGAN { get; set; }
        [AllowHtml]
        public string MOTACT { get; set; }
        public string ANH { get; set; }
    }
}
