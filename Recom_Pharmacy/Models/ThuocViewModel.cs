using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Recom_Pharmacy.Models
{
    public class ThuocViewModel
    {
        private readonly RecomPharmacyEntities db;

        public ThuocViewModel() { 
            this.db = new RecomPharmacyEntities();
        }

        public List<THUOC> getThuocWithTonKho(string search)
        {
            var today = DateTime.Today;
            var thresholdDate = today.AddMonths(1);//ngày hiện tại + 1 tháng

            var thuocs = db.THUOCs
            .Include(t => t.CTTONKHOes)
            .Include(t => t.LOAITHUOC)
            .Include(t => t.QUYDOIDVs)
            .Where(x => x.TRANGTHAI == true && x.QUYDOIDVs.Any(q => q.DONVIBANONL == true) && (search == null || x.TENTHUOC.Contains(search)))
            .ToList();


            foreach (var thuoc in thuocs)
            {
                var cttonkho = thuoc.CTTONKHOes
                                    .Where(ct => ct.TRANGTHAI == true && ct.NGAYHH.HasValue && ct.NGAYHH >= thresholdDate)//chỉ lấy tồn kho còn hạn trên 1 tháng
                                    .OrderBy(ct => ct.NGAYHH)
                                    .FirstOrDefault();

                if (cttonkho != null && cttonkho.NGAYHH.HasValue && cttonkho.NGAYHH <= thresholdDate)
                {
                    // Cập nhật TRANGTHAI của CTTONKHO này
                    cttonkho.TRANGTHAI = false;
                    db.SaveChanges();
                    // Lấy CTTONKHO kế tiếp cho thuốc (nếu có)
                    var nextCTTONKHO = thuoc.CTTONKHOes
                                            .Where(ct => ct.TRANGTHAI == true && ct.NGAYHH >= thresholdDate)
                                            .OrderBy(ct => ct.NGAYHH)
                                            .FirstOrDefault();
                    // Sử dụng nextCTTONKHO
                }
            }

            return thuocs;
        }


        public List<THUOC> getByFilterByCategoryByPrice(decimal from, decimal to, int categoryId)
        {
            var listThuoc = getThuocWithTonKho("");
            listThuoc.Where(p => p.GIABAN >= from && p.GIABAN <= to && p.MALOAI == categoryId);
            return listThuoc;
        }

        public List<THUOC> getByFilterByPrice(decimal from, decimal to)
        {
            var listThuoc = getThuocWithTonKho("");
            listThuoc.Where(p => p.GIABAN >= from && p.GIABAN <= to);
            return listThuoc;
        }
            public THUOC findById(int id)
        {
            var listThuoc = getThuocWithTonKho("");
            var thuoc = listThuoc.FirstOrDefault(x=>x.ID==id);
            return thuoc;
        }

    }


}