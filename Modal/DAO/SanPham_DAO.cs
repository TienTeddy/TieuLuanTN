using Modal.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modal.DAO
{
    public class SanPham_DAO
    {
        Context_ db = null;
        public SanPham_DAO()
        {
            db = new Context_();
        }
        
        public List<SanPham> get_product(string type)
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.SanPhams.Where(x=>x.xeploai==type).ToList();
        }
        public List<SanPham> get_product_idloaisp(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.SanPhams.Where(x => x.id_loaisp == id).ToList();
        }

        public SanPham get_product_idsanpham(int? id_sanpham)
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.SanPhams.FirstOrDefault(x => x.id_sanpham == id_sanpham);
        }

        public List<SanPham> get_product_all()
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.SanPhams.ToList();
        }
        public SanPham get_product_(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.SanPhams.FirstOrDefault(x => x.id_sanpham == id);
        }
    }
}
