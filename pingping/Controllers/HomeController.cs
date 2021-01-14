using MailChimp;
using MailChimp.Helper;
using Modal.DAO;
using Modal.EF;
using pingping.Common;
using pingping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace pingping.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                var dao_tc = new LuongTruyCap_DAO();
                var res_ = dao_tc.update_truycap_soluong("Trang Chủ","Khách Vãn Lai");

                ViewBag.Name = "My Account";
                ViewBag.Messager = "Đăng Nhập";
                ViewBag.Login = "../Accounts/Login";
            }
            else
            {
                var dao_tc = new LuongTruyCap_DAO();
                var res_ = dao_tc.update_truycap_soluong("Trang Chủ", "Khách Hàng");
                ViewBag.Name = session_acc.hoten;
                ViewBag.Messager = "Đăng Xuất";
                ViewBag.Login = "../Accounts/Logout";
            }


            #region Auction
            var dao = new DauGia_DAO();
            var result_dg = dao.get_daugia("Đang áp dụng"); //get daugia có status="đang áp dụng"
            if (result_dg == null)
            {
                return View();
            }
            else
            {
                double? sum = 0;

                var dao_sp = new SanPham_DAO();
                var dao_lsdg = new LichSuDG_DAO();

                List<SanPham> sp = new List<SanPham>();
                List<LichSuDG> lsdg = new List<LichSuDG>();
                DauGia_SanPham_LichSu_Model model = new DauGia_SanPham_LichSu_Model();
                foreach (var dg in result_dg) //list daugia
                {
                    //update time_end dg
                    System.TimeSpan time =new TimeSpan();
                    DateTime now = DateTime.Now;
                    DateTime flat = dg.time_end;
                    
                    time = (now).Subtract(flat);
                    if (time.TotalDays >= 0)
                    {
                        if (time.TotalHours >= 0)
                        {
                            if (time.TotalMinutes >= 0)
                            {
                                if (time.TotalSeconds >= 0)
                                {
                                    var res_up = dao.update_daugia_status(dg.id_daugia,0);
                                    if (res_up == null) return HttpNotFound();
                                }
                            }
                        }
                    }
                }

                //load lại daugia
                result_dg = dao.get_daugia("Đang áp dụng");
                foreach (var dg in result_dg) //list daugia
                {
                    //get sp có trong đấu gia
                    var listsp = dao_sp.get_product_idsanpham_(dg.id_sanpham);
                    foreach (var list in listsp)
                    {
                        sp.Add(new SanPham()
                        {
                            id_sanpham = list.id_sanpham,
                            id_loaisp = list.id_loaisp,
                            tensp = list.tensp,
                            tenngan = list.tenngan,
                            soluong = list.soluong,
                            dongia = list.dongia,
                            giasale = list.giasale,
                            trangthai = list.trangthai,
                            hienthi = list.hienthi,
                            barcode = list.barcode,
                            tinhtrang = list.tinhtrang,
                            thongtin = list.thongtin,
                            hinhanh1 = list.hinhanh1,
                            hinhanh2 = list.hinhanh2,
                            hinhanh3 = list.hinhanh3,
                            hinhanh4 = list.hinhanh4,
                            xeploai = list.xeploai,
                        });
                    }

                    //get lichsudaugia có trong daugia
                    var listls = dao_lsdg.get_lichsudg_daugia_id(dg.id_daugia);
                    foreach (var l in listls)
                    {
                        lsdg.Add(new LichSuDG()
                        {
                            id_lichsudg = l.id_lichsudg,
                            id_daugia = l.id_daugia,
                            id_taikhoan = l.id_taikhoan,
                            value = l.value,
                            time_update = l.time_update
                        });
                    }
                }
                model.daugia_ = result_dg;
                model.sanpham_ = sp;
                model.lichsudg_ = lsdg;
                ViewBag.values = sum;
                return View(model);
            }
            #endregion
        }
        public ActionResult CheckOut()
        {
            
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                var dao_tc = new LuongTruyCap_DAO();
                var res_ = dao_tc.update_truycap_soluong("Giỏ Hàng", "Khách Vãn Lai");
                ViewBag.Name = "My Account";
                ViewBag.Messager = "Đăng Nhập";
                ViewBag.Login = "../Accounts/Login";
                return View();
            }
            else
            {
                var dao_tc = new LuongTruyCap_DAO();
                var res_tc = dao_tc.update_truycap_soluong("Giỏ Hàng", "Khách Hàng");
                ViewBag.Name = session_acc.hoten;
                ViewBag.Messager = "Đăng Xuất";
                ViewBag.Login = "../Accounts/Logout";

                var dao_sp = new SanPham_DAO();

                var dao = new HoaDon_DAO();
                var result = dao.get_hoadon_trangthai(session_acc.id_nguoi, "Chưa Thanh Toán");

                if (result != null)
                {
                    var dao_hdct = new HoaDonCT_DAO();
                    var result_hdct = dao_hdct.get_hoadonct(result.id_hoadon);

                    var dao_size = new Size_DAO();
                    var dao_color = new Color_DAO();

                    List<SanPham> sp = new List<SanPham>();
                    List<Size> size = new List<Size>();
                    List<Color> color = new List<Color>();
                    var order = new MyOrder_Model();
                    foreach (HoaDonCT i in result_hdct)
                    {
                        //var res = dao_sp.get_product_idsanpham(i.id_sanpham);
                        var res_ = dao_sp.get_product_idsanpham_(i.id_sanpham);

                        //get size
                        var res_size = dao_size.get_size_idsanpham(i.id_sanpham);

                        if (res_size != null)
                        {
                            foreach (var s in res_size)
                            {
                                if (size == null)
                                {
                                    size.Add(new Size()
                                    {
                                        id_size = s.id_size,
                                        id_sanpham = s.id_sanpham,
                                        size = s.size,
                                        soluong = s.soluong
                                    });

                                    if (color == null)
                                    {
                                        //get color
                                        foreach(var sflat in size)
                                        {
                                            var res_color_flat = dao_color.get_color_size_id(sflat.id_size);
                                            foreach (var c in res_color_flat)
                                            {
                                                color.Add(new Color()
                                                {
                                                    id_color = c.id_color,
                                                    id_size = c.id_size,
                                                    color1 = c.color1,
                                                    soluong = s.soluong
                                                });
                                            }
                                        }
                                    }
                                    //else
                                    //{
                                    //    int flat = 0;
                                    //    foreach (var c in color)
                                    //    {
                                    //        if (s.id_size == c.id_size)
                                    //        {
                                    //            flat += 1;
                                    //        }
                                    //    }
                                    //    if (flat == 0)
                                    //    {
                                    //        //get color
                                    //        var res_color = dao_color.get_color_size_id(s.id_size);
                                    //        foreach (var c in res_color)
                                    //        {
                                    //            color.Add(new Color()
                                    //            {
                                    //                id_color = c.id_color,
                                    //                id_size = c.id_size,
                                    //                color1 = c.color1,
                                    //                soluong = s.soluong
                                    //            });
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                    int flat = 0;
                                    foreach (var ss in size)
                                    {
                                        if (ss.id_size == s.id_size)
                                        {
                                            flat += 1; //set size trùng
                                        }
                                    }
                                    if (flat == 0) //ko có size trùng
                                    {
                                        size.Add(new Size()
                                        {
                                            id_size = s.id_size,
                                            id_sanpham = s.id_sanpham,
                                            size = s.size,
                                            soluong = s.soluong
                                        });
                                        if (color == null)
                                        {
                                            //get color
                                            foreach (var sflat in size)
                                            {
                                                var res_color_flat = dao_color.get_color_size_id(sflat.id_size);
                                                foreach (var c in res_color_flat)
                                                {
                                                    color.Add(new Color()
                                                    {
                                                        id_color = c.id_color,
                                                        id_size = c.id_size,
                                                        color1 = c.color1,
                                                        soluong = c.soluong
                                                    });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            int flat_ = 0;
                                            foreach (var sss in size)
                                            {
                                                foreach (var cc in color)
                                                {
                                                    if (sss.id_size == cc.id_size)
                                                    {
                                                        flat_ += 1; //set color trùng
                                                    }
                                                }
                                            }
                                            if (flat_ == 0)
                                            {
                                                //get color
                                                var res_color_ = dao_color.get_color_size_id(s.id_size);
                                                foreach (var c_ in res_color_)
                                                {
                                                    color.Add(new Color()
                                                    {
                                                        id_color = c_.id_color,
                                                        id_size = c_.id_size,
                                                        color1 = c_.color1,
                                                        soluong = c_.soluong
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        foreach (var item in res_)
                        {
                            if (item.dongia > item.giasale)
                            {
                                item.dongia = item.giasale;
                                //order.sanpham_ = res_;
                            }
                            if (sp == null)
                            {
                                sp.Add(new SanPham()
                                {
                                    id_sanpham = item.id_sanpham,
                                    id_loaisp = item.id_loaisp,
                                    tensp = item.tensp,
                                    tenngan = item.tenngan,
                                    soluong = item.soluong,
                                    dongia = item.dongia,
                                    giasale = item.giasale,
                                    trangthai = item.trangthai,
                                    hienthi = item.hienthi,
                                    barcode = item.barcode,
                                    tinhtrang = item.tinhtrang,
                                    thongtin = item.thongtin,
                                    hinhanh1 = item.hinhanh1,
                                    hinhanh2 = item.hinhanh2,
                                    hinhanh3 = item.hinhanh3,
                                    hinhanh4 = item.hinhanh4,
                                    size = item.size,
                                    xeploai = item.xeploai
                                });
                            }
                            else //add sp
                            {
                                int flat = 0;
                                foreach(var s in sp)
                                {
                                    if (item.id_sanpham == s.id_sanpham)
                                    {
                                        flat += 1;
                                    }
                                }
                                if (flat == 0)
                                {
                                    sp.Add(new SanPham()
                                    {
                                        id_sanpham = item.id_sanpham,
                                        id_loaisp = item.id_loaisp,
                                        tensp = item.tensp,
                                        tenngan = item.tenngan,
                                        soluong = item.soluong,
                                        dongia = item.dongia,
                                        giasale = item.giasale,
                                        trangthai = item.trangthai,
                                        hienthi = item.hienthi,
                                        barcode = item.barcode,
                                        tinhtrang = item.tinhtrang,
                                        thongtin = item.thongtin,
                                        hinhanh1 = item.hinhanh1,
                                        hinhanh2 = item.hinhanh2,
                                        hinhanh3 = item.hinhanh3,
                                        hinhanh4 = item.hinhanh4,
                                        size = item.size,
                                        xeploai = item.xeploai
                                    });
                                }
                            }
                        }

                    }

                    order.sanpham_=sp;
                    order.hoadonct_ = result_hdct;
                    order.size_ = size;
                    order.color_ = color;
                    return View(order);
                }
                else
                {
                    return View();
                }
            }
            
        }

        [HttpGet]
        public JsonResult Get_Category()
        {

            var dao = new LoaiSanPham_DAO();
            var result = dao.get_category_all();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get_Product_All()
        {
            var dao = new SanPham_DAO();
            var result = dao.get_product_all();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get_Category_TrendingItem(string type)
        {
            if (type == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            var dao = new SanPham_DAO();
            var result = dao.get_product(type);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult GetHoaDonCT_id(int id)
        {
            var dao = new HoaDonCT_DAO();
            var result = dao.get_hoadonct_id(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get_Category_Product(string type)
        {
            if (type == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            var dao = new LoaiSanPham_DAO();
            int id_loaisp = dao.get_category_shortname(type);

            var dao2 = new SanPham_DAO();
            var result = dao2.get_product_idloaisp(id_loaisp);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Get_Bill(string status)
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var dao_sp = new SanPham_DAO();

                var dao = new HoaDon_DAO();
                var result = dao.get_hoadon_trangthai(session_acc.id_nguoi, status);

                if (result != null)
                {
                    var dao_hdct = new HoaDonCT_DAO();
                    var result_hdct = dao_hdct.get_hoadonct(result.id_hoadon);
                    double? gia = 0;
                    List<HoaDon_HoaDonCT_Model> models = new List<HoaDon_HoaDonCT_Model>();
                    foreach (HoaDonCT i in result_hdct)
                    {
                        var res = dao_sp.get_product_idsanpham(i.id_sanpham);
                        if (res.dongia <= res.giasale)
                        {
                            gia = res.dongia;
                        }
                        else
                        {
                            gia = res.giasale;
                        }
                        models.Add(new HoaDon_HoaDonCT_Model()
                        {
                            id_hoadonct = i.id_hoadonct,
                            id_hoadon = i.id_hoadon,
                            id_sanpham = i.id_sanpham,
                            id_nguoimua = session_acc.id_nguoi,
                            dongia = i.dongia,
                            thoigian = i.thoigian,
                            soluong = i.soluong,
                            trangthaihd = i.trangthai,
                            tensp = res.tensp,
                            tenngan = res.tenngan,
                            trangthaisp = res.trangthai,
                            barcode = res.barcode,
                            tinhtrangsp = res.tinhtrang,
                            thongtin = res.thongtin,
                            size = res.size,
                            xeploai = res.xeploai,
                            hinhanh1 = res.hinhanh1,
                            hinhanh2 = res.hinhanh2,
                            hinhanh3 = res.hinhanh3,
                            hinhanh4 = res.hinhanh4,
                            tonggiact = gia * i.soluong
                        });
                    }
                    return Json(models, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult ProductPage()
        {
            
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                var dao_tc = new LuongTruyCap_DAO();
                var res_ = dao_tc.update_truycap_soluong("Sản Phẩm", "Khách Vãn Lai");
                ViewBag.Name = "My Account";
                ViewBag.Messager = "Đăng Nhập";
                ViewBag.Login = "../Accounts/Login";
            }
            else
            {
                var dao_tc = new LuongTruyCap_DAO();
                var res_ = dao_tc.update_truycap_soluong("Sản Phẩm", "Khách Hàng");
                ViewBag.Name = session_acc.hoten;
                ViewBag.Messager = "Đăng Xuất";
                ViewBag.Login = "../Accounts/Logout";
            }
            return View();
        }

        [HttpPost]
        public JsonResult Set_CheckOut(List<CheckOut_Model> data)
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc != null)
            {
                int soluong = 0;
                double? gia = 0;
                var dao_hd = new HoaDon_DAO();
                var dao_sp = new SanPham_DAO();
                var dao_hdct = new HoaDonCT_DAO();

                var res_hd_tt = dao_hd.get_hoadon_trangthai(session_acc.id_nguoi); //id_hoadon ChưaThanhToan
                //gia = res_hd_tt.tonggia;
                if (res_hd_tt > 0)
                {
                    var r_ = dao_hd.get_hoadon_id(res_hd_tt);
                    gia = r_.tonggia;
                    //update hoadon
                    DateTime now = DateTime.Now;
                    foreach (CheckOut_Model item in data)
                    {
                        var res_sp = dao_sp.get_product_(item.id); //sp

                        var res_hdct = dao_hdct.createHoaDonCT(res_hd_tt, item.id, item.price, now, item.quantity,0,"");
                        soluong += 1;
                        gia += item.price * item.quantity;
                    }
                    int res_ = dao_hd.updateHoaDon_quantity(res_hd_tt, soluong,gia); //return 1 or 0
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //create hoadon
                    var res_hd = dao_hd.createHoaDon(session_acc.id_nguoi); //return hoadon
                    foreach (CheckOut_Model item in data)
                    {
                        var res_sp = dao_sp.get_product_(item.id); //sp

                        var res_hdct = dao_hdct.createHoaDonCT(res_hd.id_hoadon, item.id, item.price, res_hd.thoigian, item.quantity, 0, "");
                        soluong += 1;
                        gia += item.price * item.quantity;
                    }
                    int res_ = dao_hd.updateHoaDon_quantity(res_hd.id_hoadon, soluong, gia); //return 1 or 0
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(-1, JsonRequestBehavior.AllowGet);
        }

        string MailChimpAPIKey = System.Configuration.ConfigurationManager.AppSettings["MailChimpAPIKey"];
        string MailChimpAPIKeySubsribeListID = System.Configuration.ConfigurationManager.AppSettings["MailChimpAPIKeySubsribeListID"];
        public ActionResult AddSubscribe(FormCollection frc)
        {
            string userEmail = frc["subscribe"];
            MailChimpManager mc = new MailChimpManager(MailChimpAPIKey);
            EmailParameter email = new EmailParameter()
            {
                Email = userEmail
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            EmailParameter results = mc.Subscribe(MailChimpAPIKeySubsribeListID, email);
            return View("~/Views/Home/Index.cshtml");
        }

        public ActionResult Contact()
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                var dao_tc = new LuongTruyCap_DAO();
                var res_ = dao_tc.update_truycap_soluong("Liên Hệ", "Khách Vãn Lai");
                ViewBag.Name = "My Account";
                ViewBag.Messager = "Đăng Nhập";
                ViewBag.Login = "../Accounts/Login";
            }
            else
            {
                var dao_tc = new LuongTruyCap_DAO();
                var res_ = dao_tc.update_truycap_soluong("Liên Hệ", "Khách Hàng");
                ViewBag.Name = session_acc.hoten;
                ViewBag.Messager = "Đăng Xuất";
                ViewBag.Login = "../Accounts/Logout";
            }
            return View();
        }

        [HttpGet]
        public ActionResult SingleProcduct(int id)
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                ViewBag.Name = "My Account";
                ViewBag.Messager = "Đăng Nhập";
                ViewBag.Login = "../Accounts/Login";
            }
            else
            {
                ViewBag.Name = session_acc.hoten;
                ViewBag.Messager = "Đăng Xuất";
                ViewBag.Login = "../Accounts/Logout";
            }
            //Kiem tra truyen tham so rong
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dao = new SanPham_DAO();
            SanPham sp = dao.get_product_(id);

            //get size
            var dao_s = new Size_DAO();
            var res_size = dao_s.get_size_idsanpham(id); //list

            //Neu khong thi truy xuat csdl lay ra san pham tuong ung
            if (sp == null)
            {
                // Thong bao neu san pham khong co san pham do
                return HttpNotFound();
            }

            var sanpham_size_model = new SanPham_Size_Model();
            sanpham_size_model.SanPham_ = sp;
            sanpham_size_model.Size_ = res_size;
            return View(sanpham_size_model);
        }

        [HttpGet]
        public JsonResult Get_Color_Size_id(int id_size)
        {
            //get color
            var dao = new Color_DAO();
            var res_size = dao.get_color_size_id(id_size); //list
            return Json(res_size, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Set_CheckOut_Quickly(List<CheckOut_Model> data)
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc != null)
            {
                int soluong = 0;
                double? gia = 0;
                var dao_hd = new HoaDon_DAO();
                var dao_sp = new SanPham_DAO();
                var dao_hdct = new HoaDonCT_DAO();

                var res_hd_tt = dao_hd.get_hoadon_trangthai(session_acc.id_nguoi); //id_hoadon ChưaThanhToan
                //gia = res_hd_tt.tonggia;
                if (res_hd_tt > 0)
                {
                    var r_ = dao_hd.get_hoadon_id(res_hd_tt);
                    gia = r_.tonggia;
                    //update hoadon
                    DateTime now = DateTime.Now;
                    foreach (CheckOut_Model item in data)
                    {
                        var res_sp = dao_sp.get_product_(item.id); //sp

                        var res_hdct = dao_hdct.createHoaDonCT(res_hd_tt, item.id, item.price, now, item.quantity,item.id_size,item.color);
                        soluong += 1;
                        gia += item.price * item.quantity;
                    }
                    int res_ = dao_hd.updateHoaDon_quantity(res_hd_tt, soluong, gia); //return 1 or 0
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //create hoadon
                    var res_hd = dao_hd.createHoaDon(session_acc.id_nguoi); //return hoadon
                    foreach (CheckOut_Model item in data)
                    {
                        var res_sp = dao_sp.get_product_(item.id); //sp

                        var res_hdct = dao_hdct.createHoaDonCT(res_hd.id_hoadon, item.id, item.price, res_hd.thoigian, item.quantity, item.id_size, item.color);
                        soluong += 1;
                        gia += item.price * item.quantity;
                    }
                    int res_ = dao_hd.updateHoaDon_quantity(res_hd.id_hoadon, soluong, gia); //return 1 or 0
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(-1, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MyOrder()
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                ViewBag.Name = "My Account";
                ViewBag.Messager = "Đăng Nhập";
                ViewBag.Login = "../Accounts/Login";
                return View("Index");
            }
            else
            {
                var dao_tc = new LuongTruyCap_DAO();
                var res_ = dao_tc.update_truycap_soluong("Đơn Hàng", "Khách Hàng");
                ViewBag.Name = session_acc.hoten;
                ViewBag.Messager = "Đăng Xuất";
                ViewBag.Login = "../Accounts/Logout";
            }
            return View();
        }
        
        
        [HttpPost]
        public JsonResult Update_HDCT(int id_hdct,int id_size,string color)
        {
            //get color
            var dao = new HoaDonCT_DAO();
            int res_ = dao.update_hdct(id_hdct, id_size, color);

            if (res_ != 0) { return Json(res_, JsonRequestBehavior.AllowGet);  }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[HttpGet]
        public JsonResult Bill_Deleted(int? id,int hd)
        {
            if (id == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var dao_hdct = new HoaDonCT_DAO();
                var res_hdct = dao_hdct.deleted_hdct(id,hd);
                if (res_hdct !=0 && res_hdct!=-1 )
                {
                    return Json(res_hdct, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public JsonResult Bill_Update(int? id,int hd,int l)
        {
            if (id == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var dao_hdct = new HoaDonCT_DAO();
                var res_hdct = dao_hdct.update_hdct(id,l);
                if (res_hdct == 1)
                {
                    var dao = new HoaDonCT_DAO();
                    var res_hd = dao.update_hdct_tonggia(id,hd,l);
                    return Json(res_hd, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
        }

        #region Auction
        [HttpGet]
        public JsonResult Auction(int dg,float value)
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var dao_ls = new LichSuDG_DAO();
                var res_ls = dao_ls.create_lichsudg_id_taikhoan_daugia(dg, session_acc.id_taikhoan, value); //create auction
                if (res_ls == 0)
                {
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //update Result winner
                    var dao_dg = new DauGia_DAO();
                    var res_dg = dao_dg.update_daugia_result(dg,session_acc.hoten);
                    if (res_dg == null)
                    {
                        return Json(-1, JsonRequestBehavior.AllowGet);
                    }
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion
    }
}
