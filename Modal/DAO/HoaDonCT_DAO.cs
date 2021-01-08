using Modal.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modal.DAO
{
    public class HoaDonCT_DAO
    {
        Context_ db = null;
        public HoaDonCT_DAO()
        {
            db = new Context_();
        }

        public HoaDonCT createHoaDonCT(int id_hoadon,int id_sanpham,float dongia,DateTime thoigian,int soluong)
        {
            db.Configuration.ProxyCreationEnabled = false;
            HoaDonCT result = new HoaDonCT();

            //xử lý sp trùng nhau trong hoadonct
            var res_ = db.HoaDonCTs.FirstOrDefault(x => x.id_hoadon == id_hoadon && x.id_sanpham == id_sanpham);
            if (res_ != null)
            {
                res_.thoigian = thoigian;
                res_.soluong += soluong;
                db.SaveChanges();
            }
            else
            {
                var modal_To_EF = new HoaDonCT()
                {
                    id_hoadon = id_hoadon,
                    id_sanpham = id_sanpham,
                    dongia = dongia,
                    thoigian = thoigian,
                    soluong = soluong,
                    trangthai = "Chưa Thanh Toán"
                };

                result = db.HoaDonCTs.Add(modal_To_EF);
                db.SaveChanges();
                if (result != null) return result;
            }
            return null;
        }

        public List<HoaDonCT> get_hoadonct(int id_hoadon)
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.HoaDonCTs.Where(x => x.id_hoadon == id_hoadon).ToList();
        }
    }
}
