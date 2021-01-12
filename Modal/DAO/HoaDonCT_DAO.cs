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

        public HoaDonCT createHoaDonCT(int id_hoadon,int id_sanpham,float dongia,DateTime thoigian,int soluong,int id_size,string color)
        {
            db.Configuration.ProxyCreationEnabled = false;
            HoaDonCT result = new HoaDonCT();

            //xử lý sp trùng nhau trong hoadonct
            var res_ = db.HoaDonCTs.FirstOrDefault(x => x.id_hoadon == id_hoadon && x.id_sanpham == id_sanpham);
            if (res_ != null)
            {
                var dao_size = new Size_DAO();
                var res_size = dao_size.get_size_(id_size);
                if (res_size != null) //tồn tại sp có size đó
                {
                    //var dao_color = new Color_DAO();
                    //var res_color = dao_color.get_color_size_id_color(res_size,color);

                    var checkhdct_color = db.HoaDonCTs.Count(x => x.color == color && id_size == res_size.id_size);
                    if (checkhdct_color > 0)
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
                            trangthai = "Chưa Thanh Toán",
                            id_size = id_size,
                            color = color
                        };

                        result = db.HoaDonCTs.Add(modal_To_EF);
                        db.SaveChanges();
                        if (result != null) return result;
                    }

                }
                else //tạo mới hdct
                {
                    var modal_To_EF = new HoaDonCT()
                    {
                        id_hoadon = id_hoadon,
                        id_sanpham = id_sanpham,
                        dongia = dongia,
                        thoigian = thoigian,
                        soluong = soluong,
                        trangthai = "Chưa Thanh Toán",
                        id_size = id_size,
                        color = color
                    };

                    result = db.HoaDonCTs.Add(modal_To_EF);
                    db.SaveChanges();
                    if (result != null) return result;
                }
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
                    trangthai = "Chưa Thanh Toán",
                    id_size = id_size,
                    color = color
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
        public int get_hoadonct_id(int id_hdct)
        {
            db.Configuration.ProxyCreationEnabled = false;

            int id =db.HoaDonCTs.FirstOrDefault(x => x.id_hoadonct == id_hdct).id_hoadonct;
            if (id > 0) return id;
            return 0;
        }

        public int update_hdct(int id_hdct,int id_size,string color)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var res_ = db.HoaDonCTs.FirstOrDefault(x => x.id_hoadonct == id_hdct);
            if (res_ != null)
            {
                res_.id_size = id_size;
                res_.color = color;
                db.SaveChanges();
                return res_.id_hoadonct;
            }
            return 0;
        }

        public double? update_hdct_tonggia(int? id_hdct,int id_hoadon, int soluong)
        {
            if (id_hdct == null)
            {
                return 0;
            }
            else
            {
                double? sum = 0;
                var hdct = db.HoaDonCTs.Where(x=>x.id_hoadon==id_hoadon).ToList();
                foreach (var c in hdct)
                {
                    sum += c.soluong * c.dongia;
                }

                var hd = db.HoaDons.Find(id_hoadon);
                if (hd != null)
                {
                    hd.tonggia = sum;
                    db.SaveChanges();
                    return sum;
                }
            }
            return -1;
        }
        #region CRUD
        public double? deleted_hdct(int? id_hdct,int id_hoadon)
        {
            if (id_hdct == null)
            {
                return 0;
            }
            else
            {
                HoaDonCT hdct = db.HoaDonCTs.Find(id_hdct);
                db.HoaDonCTs.Remove(hdct);
                db.SaveChanges();
                if (hdct != null)
                {
                    double? sum = 0; int q = 0;
                    var soluong_hdct = db.HoaDonCTs.Where(x => x.id_hoadon == id_hoadon).ToList();
                    foreach(var c in soluong_hdct)
                    {
                        sum += c.soluong * c.dongia;
                        q++;
                    }
                    var hd_soluong = db.HoaDons.FirstOrDefault(x => x.id_hoadon == id_hoadon);
                    hd_soluong.soluong = q;
                    hd_soluong.tonggia = sum;
                    db.SaveChanges();
                    return sum;
                }
            }
            return -1;
        }
        public int update_hdct(int? id_hdct,int soluong)
        {
            if (id_hdct == null)
            {
                return 0;
            }
            else
            {
                HoaDonCT hdct = db.HoaDonCTs.Find(id_hdct);
                
                if (hdct != null)
                {
                    hdct.soluong = soluong;
                    db.SaveChanges();
                    return 1;
                }
            }
            return -1;
        }
        #endregion
    }
}
