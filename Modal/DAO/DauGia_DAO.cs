using Modal.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modal.DAO
{
    public class DauGia_DAO
    {
        Context_ db = null;
        public DauGia_DAO()
        {
            db = new Context_();
        }

        public int get_count()
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.DauGias.Count();
        }
        public int get_count_run()
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.DauGias.Count(x=>x.status_=="Đang áp dụng");
        }
        public int get_count_stop()
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.DauGias.Count(x => x.status_ == "Kết Thúc");
        }

        public List<DauGia> get_daugia(string status)
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.DauGias.Where(x=>x.status_==status).ToList();
        }
        
        public DauGia update_daugia_result(int id_daugia,string result)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var res = db.DauGias.FirstOrDefault(x => x.id_daugia == id_daugia);
            if (res != null)
            {
                res.result = result;
                var ok=db.SaveChanges();
                if (ok>0) return res;
                return null;
            }
            else
            {
                return null;
            }
        }
        public DauGia update_daugia_status(int id_daugia,int date)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var res = db.DauGias.FirstOrDefault(x => x.id_daugia == id_daugia);
            if (res != null)
            {
                if (date == 0)
                {
                    res.status_ = "Kết Thúc";
                    var ok = db.SaveChanges();
                    if (ok > 0) return res;
                    return null;
                }
                else return null;
            }
            else
            {
                return null;
            }
        }
    }
}
