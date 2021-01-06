using Modal.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modal.DAO
{
    public class NguoiMua_DAO
    {
        Context_ db = null;
        public NguoiMua_DAO()
        {
            db = new Context_();
        }

        public NguoiMua get_infor(int id_taikhoan)
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.NguoiMuas.FirstOrDefault(x => x.id_taikhoan == id_taikhoan);
        }
    }
}
