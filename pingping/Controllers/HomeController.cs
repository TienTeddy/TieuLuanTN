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
            return View();
        }
        public ActionResult CheckOut()
        {
            
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                ViewBag.Name = "My Account";
                ViewBag.Messager = "Đăng Nhập";
                ViewBag.Login = "../Accounts/Login";
                return View();
            }
            else
            {
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

                    List<SanPham> sp = new List<SanPham>();
                    var order = new MyOrder_Model();
                    foreach (HoaDonCT i in result_hdct)
                    {
                        //var res = dao_sp.get_product_idsanpham(i.id_sanpham);
                        var res_ = dao_sp.get_product_idsanpham_(i.id_sanpham);

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
            var dao = new SanPham_DAO();
            var result = dao.get_product(type);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get_Category_Product(string type)
        {
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
            return View();
        }

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
            //var session_acc = SessionHelper.GetSession();
            //var dao_1 = new HoaDon_DAO();
            //var hoadon = dao_1.get_hoadon_idtaikhoan(session_acc.id_taikhoan);
            //if (hoadon != null)
            //{
            //    MyOrder_Model res = new MyOrder_Model();
            //    foreach (var hd in hoadon)
            //    {
            //        var dao_2 = new HoaDonCT_DAO();
            //        var hoadonchitiet = dao_2.get_hoadonct(hd.id_hoadon);
            //        res.hoadon = hoadon;
            //        foreach (var hdct in hoadonchitiet)
            //        {
            //            res.hoadonct_ = hoadonchitiet;
            //        }
            //    }
            //}
            return View();
        }
    }
}
