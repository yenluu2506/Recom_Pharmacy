using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Recom_Pharmacy.Models.Common
{
    public static class Constant
    {
        public static string maphieunhap = "NHH@" + DateTime.Today.Year.ToString();

          public static List<SelectItem> selectThuocHetHan()
        {
            var options = new List<SelectItem>
            {
                new SelectItem(1, "Trong 1 tháng"),
                new SelectItem(6, "Trong 6 tháng"),
                new SelectItem(0, "Đã hết hạn")
            };
            return options;
        }

        public static List<SelectItem> selectTonKhoLauNam()
        {
            var options = new List<SelectItem>
            {
                new SelectItem(1, "Trong 1 tháng"),
                new SelectItem(6, "Trong 6 tháng"),
                new SelectItem(12, "Trong 1 năm")
            };
            return options;
        }
    }

    public class SelectItem
    {
        public int value { get; set; }
        public string optionName { get; set; }

        public SelectItem(int value, string optionName)
        {
            this.value = value;
            this.optionName = optionName;
        }
    }
}