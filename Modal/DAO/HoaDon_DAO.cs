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

        public int updateHoaDon_quantity(int soluong)
        {
            db.Configuration.ProxyCreationEnabled = false;

            HoaDon result = new HoaDon();
            var modal_To_EF = new HoaDon()
            {
                soluong=soluong
            };

            var ok=db.SaveChanges();
            if (ok>0) return 1;
            return 0;
        }
        public HoaDon createHoaDon(int id_nguoimua)
        {
            db.Configuration.ProxyCreationEnabled = false;

            HoaDon result = new HoaDon();
            DateTime now = DateTime.Now;

            var modal_To_EF = new HoaDon()
            {
                id_nguoimua = id_nguoimua,
                //mahd
                //tonggia
                thoigian = now,
                hinhthuctt = "Mới Đặt",
                freeship = 0,
                trangthai = "Chưa Thanh Toán";
            };

            result = db.HoaDons.Add(modal_To_EF);
            db.SaveChanges();
            if (result != null) return result;
            return null;
        }
    }
}
