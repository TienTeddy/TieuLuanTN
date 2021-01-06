using Modal.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modal.DAO
{
    public class TaiKhoan_DAO
    {
        Context_ db = null;
        public TaiKhoan_DAO()
        {
            db = new Context_();
        }
        public TaiKhoan Get_id_taikhoan(string user, string pass)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var res = db.TaiKhoans.FirstOrDefault(x => x.username == user && x.password == pass);
            return res;
        }
        public int CheckLogin(string user, string pass)
        {
            db.Configuration.ProxyCreationEnabled = false;

            if (db.TaiKhoans.Count(x => x.username == user) > 0)
            {
                if (db.TaiKhoans.Count(x => x.password == pass) > 0)
                {
                    if (db.TaiKhoans.Count(x =>x.username==user && x.loaitk == false) > 0) 
                    {
                        return 2; //saller
                    }
                    else return 1;  //buyer
                }
                else return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
