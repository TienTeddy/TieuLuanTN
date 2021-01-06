using Modal.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modal.DAO
{
    public class LoaiSanPham_DAO
    {
        Context_ db = null;
        public LoaiSanPham_DAO()
        {
            db = new Context_();
        }

        public List<LoaiSanPham> get_category_all()
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.LoaiSanPhams.ToList();
        }
        
        public int get_category_shortname(string type)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var res = db.LoaiSanPhams.FirstOrDefault(x => x.tenngan == type);
            return res.id_loaisp;
        }
    }
}
