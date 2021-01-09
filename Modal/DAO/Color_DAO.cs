using Modal.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modal.DAO
{
    public class Color_DAO
    {
        Context_ db = null;
        public Color_DAO()
        {
            db = new Context_();
        }

        public List<Color> get_color_size_id(int id_size)
        {
            db.Configuration.ProxyCreationEnabled = false;

            return db.Colors.Where(x => x.id_size == id_size).ToList();
        }
    }
}
