﻿using Modal.EF;
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

        public NguoiMua create_nguoimua(int id_taikhoan,int phone)
        {
            db.Configuration.ProxyCreationEnabled = false;
            NguoiMua tk_ = new NguoiMua();
            var tk = new NguoiMua()
            {
                id_taikhoan=id_taikhoan,
                phone=phone
            };

            tk_ = db.NguoiMuas.Add(tk);
            db.SaveChanges();
            if (tk_ != null) return tk_;
            return null;
        }
        public NguoiMua update_nguoimua(string street, string ward, string dictrict, string province, int phone, int id_taikhoan)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var res = db.NguoiMuas.FirstOrDefault(x => x.id_taikhoan == id_taikhoan);
            if (res != null)
            {
                res.street = street;
                res.ward = ward;
                res.district = dictrict;
                res.province = province;
                res.phone = phone;
                db.SaveChanges();
                return res;
            }
            return null;
        }
    }
}
