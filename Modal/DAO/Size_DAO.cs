﻿using Modal.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modal.DAO
{
    public class Size_DAO
    {
        Context_ db = null;
        public Size_DAO()
        {
            db = new Context_();
        }

        public List<Size> get_size_idsanpham(int id_sanpham)
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.Sizes.Where(x => x.id_sanpham == id_sanpham).ToList();
        }
        public int get_size(int id_size)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var res = db.Sizes.Count(x => x.id_size == id_size);
            if (res != 0) { return res; }
            return 0;
        }
    }
}