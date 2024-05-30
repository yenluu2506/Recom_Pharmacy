using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Recom_Pharmacy.Models.DTO
{
    public class ProductRespone
    {
        [JsonProperty("San pham goi y")]
        public List<string> SanPhamGoiY {  get; set; }
    }
}