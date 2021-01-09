using Modal.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pingping.Models
{
    public class MyOrder_Model
    {
        public List<HoaDon> hoadon_ { get; set; }
        public List<HoaDonCT> hoadonct_ { get; set; }
    }
}