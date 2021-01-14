using Modal.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modal.DAO
{
    public class HoaDon_DAO
    {
        Context_ db = null;
        public HoaDon_DAO()
        {
            db = new Context_();
        }
        public int get_count()
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.HoaDons.Count(x=>x.trangthai=="Đã Thanh Toán");
        }

        public HoaDon get_hoadon_id(int id_hoadon)
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.HoaDons.FirstOrDefault(x => x.id_hoadon == id_hoadon);
        }

        public HoaDon get_hoadon_trangthai(int id_nguoimua,string status)
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.HoaDons.FirstOrDefault(x => x.id_nguoimua == id_nguoimua&&x.trangthai== status);
        }

        // nếu hóa đơn nào chưa đc thanh toán thì lấy ra
        public int get_hoadon_trangthai(int id_nguoimua)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var res= db.HoaDons.FirstOrDefault(x => x.id_nguoimua == id_nguoimua && x.trangthai == "Chưa Thanh Toán");

            if (res == null)
            {
                return 0;
            }
            return res.id_hoadon;
        }

        public int updateHoaDon_quantity(int id_hoadon,int soluong,double? gia)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var dao = new HoaDonCT();
            int quantity = db.HoaDonCTs.Count(x => x.id_hoadon == id_hoadon);
            var res = db.HoaDons.FirstOrDefault(x => x.id_hoadon == id_hoadon);
            if (res!=null){
                res.soluong = quantity;
                res.tonggia = gia;
                db.SaveChanges();

                return 1;
            }
            return 0;
        }
        public HoaDon createHoaDon(int id_nguoimua)
        {
            db.Configuration.ProxyCreationEnabled = false;

            HoaDon result = new HoaDon();
            DateTime now = DateTime.Now;

            var modal_To_EF = new HoaDon()
            {
                id_nguoimua=id_nguoimua,
                //mahd
                //tonggia
                thoigian=now,
                hinhthuctt ="Mới Đặt",
                freeship=0,
                trangthai= "Chưa Thanh Toán"
            };

            result = db.HoaDons.Add(modal_To_EF);
            db.SaveChanges();
            if (result != null) return result;
            return null;
        }

        public List<HoaDon> get_hoadon_taikhoan_id(int id_taikhoan)
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.HoaDons.Where(x => x.id_nguoimua == id_taikhoan).ToList();
        }

        #region 13-01
        public List<HoaDon> get_hoadon_damua_all()
        {
            return db.HoaDons.Where(n => n.trangthai == "Đã Thanh Toán").OrderBy(n => n.thoigian).ToList();
        }
        #endregion
    }
}
