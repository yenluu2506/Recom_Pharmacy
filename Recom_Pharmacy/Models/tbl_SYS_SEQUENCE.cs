using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Recom_Pharmacy.Models
{
    public class tbl_SYS_SEQUENCE
    {
        private RecomPharmacyEntities db;
        public tbl_SYS_SEQUENCE()
        {
          db = new RecomPharmacyEntities();
        }

        public SYS_SEQUENCE getItem(string seqname)
        {
            return db.SYS_SEQUENCE.FirstOrDefault(x => x.SEQNAME == seqname);
        }

        public void add(SYS_SEQUENCE sequence)
        {
            try
            {
                db.SYS_SEQUENCE.Add(sequence);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }

        public void update(SYS_SEQUENCE sequence)
        {
            var seq = db.SYS_SEQUENCE.FirstOrDefault(x => x.SEQNAME == sequence.SEQNAME);
            seq.SEQVALUE = sequence.SEQVALUE + 1;
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi: " + ex.Message);
            }
        }
    }
}