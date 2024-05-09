using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Recom_Pharmacy.Models
{
    public class HoaDonNhapViewModel
    {
        public HOADONNHAP hoadonnhap  { get; set; }
        public List <CHITIETHDN>  cthdn { get; set; }
    }
}