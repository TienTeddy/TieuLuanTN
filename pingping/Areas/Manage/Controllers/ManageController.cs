using Modal.DAO;
using Modal.EF;
using pingping.Common;
using pingping.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace pingping.Areas.Manage.Controllers
{
    public class ManageController : Controller
    {
        // GET: Manage/Home
        public ActionResult Index()
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                return RedirectToAction("../../Accounts/Login");
            }
            else
            {
                ViewBag.Name = session_acc.hoten;
                ViewBag.Messager = "Đăng Xuất";
                ViewBag.Login = "../Accounts/Logout";
                return View();
            }
        }
        public ActionResult Icons()
        {
            return View();
        }
        public ActionResult Profile()
        {
            return View();
        }
        public ActionResult Tables()
        {
            return View();
        }

        public JsonResult Count_statistis()
        {
            Object statistis = new Object();

            var dao_sp = new SanPham_DAO();
            var dao_hd = new HoaDon_DAO();
            var dao_mb = new TaiKhoan_DAO();
            var dao_dg = new DauGia_DAO();

            int sp = dao_sp.get_count();
            int sphide = dao_sp.get_count_hide();

            int hd= dao_hd.get_count();

            int member = dao_mb.get_count();
            int mbad = dao_mb.get_count_loai(false);
            int mbkh = dao_mb.get_count_loai(true);

            int dg = dao_dg.get_count();
            int dgrun = dao_dg.get_count_stop();
            int dgstop = dao_dg.get_count_run();

            statistis = new
            {
                sp = sp,
                sphide=sphide,
                hd = hd,
                mb = member,
                mbad=mbad,
                mbkh=mbkh,
                dg = dg,
                dgrun=dgrun,
                dgstop=dgstop
            };
            if (statistis != null)
            {
                return Json(statistis, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        #region new
        public ActionResult QuanLyLoaiSanPham()
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                return RedirectToAction("../../Accounts/Login");
            }

            ViewBag.Name = session_acc.hoten;
            ViewBag.Messager = "Đăng Xuất";
            ViewBag.Login = "../Accounts/Logout";

            var dao = new LoaiSanPham_DAO();
            var lstLoaiSanPham = dao.get_category_all();
            return View(lstLoaiSanPham);
        }
        public ActionResult DeleteLoaiSP(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dao = new LoaiSanPham_DAO();
            var res = dao.remove_category_idloaisanpham(id);
            if (res == 1)
            {
                ViewBag.notify = "Xoá Thành Công";
                ViewBag.Alert = "alert-info";
            }
            else
            {
                ViewBag.notify = "Xoá Thất Bại!";
                ViewBag.Alert = "alert-danger";
            }
            var lstLoaiSanPham = dao.get_category_all();
            return RedirectToAction("QuanLyLoaiSanPham", lstLoaiSanPham);
        }
        public ActionResult CreateLoaiSP(LoaiSanPham lsp, HttpPostedFileBase hinhanh)
        {
            if (lsp == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (hinhanh == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dao = new LoaiSanPham_DAO();
            //Kiem Tra hình tồn tại trong csdl chưa
            if (hinhanh.ContentLength > 0)
            {
                if (hinhanh.ContentType != "image/jpeg" && hinhanh.ContentType != "image/png" && hinhanh.ContentType != "image/gif" && hinhanh.ContentType != "image/tiff" && hinhanh.ContentType != "image/BMP" && hinhanh.ContentType != "image/jpg")
                {
                    ViewBag.upload = "không phải định dạng của hình ảnh";
                    return View();
                }
                //Lay hinh anh
                var fileName = Path.GetFileName(hinhanh.FileName);
                //Lấy hình ảnh chuyển vào thư mục hình ảnh
                var path = Path.Combine(Server.MapPath("~/Source/img-public"), fileName);
                //Nếu thư mục chứa hình ảnh đó rồi sẽ xuất ra thông báo
                if (System.IO.File.Exists(path))
                {
                    ViewBag.upload = "Hình đã tồn tại";
                    return View();
                }
                else
                {
                    //Lấy Hình Ảnh đưa vào thư muc HinhAnhSP
                    hinhanh.SaveAs(path);
                    lsp.hinhanh = fileName;

                    var res = dao.set_category(lsp);
                    if (res == 1)
                    {
                        ViewBag.notify = "Thêm loại sản phẩm thành công";
                        ViewBag.Alert = "alert-info";
                    }
                    else
                    {
                        ViewBag.notify = "Thêm loại sản phẩm thât bại";
                        ViewBag.Alert = "alert-danger";
                    }
                }
            }
            var lstLoaiSanPham = dao.get_category_all();
            return RedirectToAction("QuanLyLoaiSanPham", lstLoaiSanPham);
        }

        [HttpGet]
        public JsonResult Get_LoaiSanPham_id(int id_loaisp)
        {
            var dao = new LoaiSanPham_DAO();
            var res = dao.get_category_id(id_loaisp);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateLoaiSP(FormCollection f, HttpPostedFileBase hinhanh)
        {
            if (f == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var id1 = f["a"];
            int id = Int32.Parse(f["a"]);
            var dao = new LoaiSanPham_DAO();
            LoaiSanPham lsp = new LoaiSanPham();
            var res = dao.gett_category_id(id);
            if (hinhanh == null)
            {
                if (res.hinhanh != null)
                {
                    lsp.id_loaisp = id;
                    lsp.tenloai = f["tenloai"];
                    lsp.tenngan = f["tenngan"];
                    lsp.thongtin = f["thongtin"];
                    lsp.hinhanh = res.hinhanh;
                    lsp.xeploai = f["xeploai"];
                    lsp.theloai = f["theloai"];
                    var res2 = dao.update_category_id(lsp);
                    var lstLoaiSanPham1 = dao.get_category_all();
                    return RedirectToAction("QuanLyLoaiSanPham", lstLoaiSanPham1);
                }
                else
                {
                    lsp.id_loaisp = id;
                    lsp.tenloai = f["tenloai"];
                    lsp.tenngan = f["tenngan"];
                    lsp.thongtin = f["thongtin"];
                    lsp.hinhanh = "loaisp.png";
                    lsp.xeploai = f["xeploai"];
                    lsp.theloai = f["theloai"];
                    var res3 = dao.update_category_id(lsp);
                    var lstLoaiSanPham2 = dao.get_category_all();
                    return RedirectToAction("QuanLyLoaiSanPham", lstLoaiSanPham2);
                }
            }
            //Kiem Tra hình tồn tại trong csdl chưa
            if (hinhanh.ContentLength > 0)
            {
                if (hinhanh.ContentType != "image/jpeg" && hinhanh.ContentType != "image/png" && hinhanh.ContentType != "image/gif" && hinhanh.ContentType != "image/tiff" && hinhanh.ContentType != "image/BMP" && hinhanh.ContentType != "image/jpg")
                {
                    ViewBag.upload = "không phải định dạng của hình ảnh";
                    return View();
                }
                //Lay hinh anh
                var fileName = Path.GetFileName(hinhanh.FileName);
                //Lấy hình ảnh chuyển vào thư mục hình ảnh
                var path = Path.Combine(Server.MapPath("~/Source/img-public"), fileName);
                //Nếu thư mục chứa hình ảnh đó rồi sẽ xuất ra thông báo
                if (System.IO.File.Exists(path))
                {
                    lsp.hinhanh = fileName;
                }
                else
                {
                    //Lấy Hình Ảnh đưa vào thư muc HinhAnhSP
                    hinhanh.SaveAs(path);
                    lsp.hinhanh = fileName;
                }
            }
            lsp.id_loaisp = id;
            lsp.tenloai = f["tenloai"];
            lsp.tenngan = f["tenngan"];
            lsp.thongtin = f["thongtin"];
            lsp.xeploai = f["xeploai"];
            lsp.theloai = f["theloai"];
            var res1 = dao.update_category_id(lsp);
            var lstLoaiSanPham3 = dao.get_category_all();
            return RedirectToAction("QuanLyLoaiSanPham", lstLoaiSanPham3);
        }
        public ActionResult QuanLyKhachHang()
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                return RedirectToAction("../../Accounts/Login");
            }

            ViewBag.Name = session_acc.hoten;
            ViewBag.Messager = "Đăng Xuất";
            ViewBag.Login = "../Accounts/Logout";

            var dao = new NguoiMua_DAO();
            var dao1 = new TaiKhoan_DAO();
            var lstNguoiMua = dao.get_nguoimua_all();
            List<Account_buyer_Model> tkngmua = new List<Account_buyer_Model>();
            TaiKhoan tk = new TaiKhoan();
            foreach (var item in lstNguoiMua)
            {
                tk = dao1.Get_id_taikhoanAdmin(item.id_taikhoan);
                tkngmua.Add(new Account_buyer_Model
                {
                    id_taikhoan = item.id_taikhoan,
                    hoten = tk.hoten,
                    email = tk.email,
                    loaitk = tk.loaitk,
                    phone = item.phone,
                    street = item.street,
                    ward = item.ward,
                    district = item.district,
                    province = item.province
                });
            }
            return View(tkngmua);
        }
        public ActionResult DeleteAccount(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dao = new TaiKhoan_DAO();
            var res = dao.remove_account_idtknm(id);
            if (res == 1)
            {
                ViewBag.notify = "Xoá Thành Công";
                ViewBag.Alert = "alert-info";
            }
            else
            {
                ViewBag.notify = "Xoá Thất Bại!";
                ViewBag.Alert = "alert-danger";
            }
            var dao1 = new NguoiMua_DAO();
            var lstNguoiMua = dao1.get_nguoimua_all();
            List<Account_buyer_Model> tkngmua = new List<Account_buyer_Model>();
            TaiKhoan tk = new TaiKhoan();
            foreach (var item in lstNguoiMua)
            {
                tk = dao.Get_id_taikhoanAdmin(item.id_taikhoan);
                tkngmua.Add(new Account_buyer_Model
                {
                    id_taikhoan = item.id_taikhoan,
                    hoten = tk.hoten,
                    email = tk.email,
                    loaitk = tk.loaitk,
                    phone = item.phone,
                    street = item.street,
                    ward = item.ward,
                    district = item.district,
                    province = item.province
                });
            }
            return RedirectToAction("QuanLyKhachHang", tkngmua);
        }
        public ActionResult CreateBuyer(Account_buyer_Model acc)
        {
            if (acc == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dao = new TaiKhoan_DAO();
            TaiKhoan res = dao.create_taikhoan(acc.username, acc.password, acc.hoten, acc.email);

            if (res != null)
            {
                var id_taikhoan = res.id_taikhoan;
                var dao1 = new NguoiMua_DAO();
                NguoiMua res1 = dao1.create_nguoimua_Admin(id_taikhoan, acc.phone, acc.street, acc.ward, acc.district, acc.province);
                if (res1 != null)
                {
                    ViewBag.notify = "Thêm thành viên thành công";
                    ViewBag.Alert = "alert-info";
                }
                else
                {
                    ViewBag.notify = "Thêm thành viên thât bại";
                    ViewBag.Alert = "alert-danger";
                }
            }
            var dao2 = new NguoiMua_DAO();
            var lstNguoiMua = dao2.get_nguoimua_all();
            List<Account_buyer_Model> tkngmua = new List<Account_buyer_Model>();
            TaiKhoan tk = new TaiKhoan();
            foreach (var item in lstNguoiMua)
            {
                tk = dao.Get_id_taikhoanAdmin(item.id_taikhoan);
                tkngmua.Add(new Account_buyer_Model
                {
                    id_taikhoan = item.id_taikhoan,
                    hoten = tk.hoten,
                    email = tk.email,
                    loaitk = tk.loaitk,
                    phone = item.phone,
                    street = item.street,
                    ward = item.ward,
                    district = item.district,
                    province = item.province
                });
            }
            return RedirectToAction("QuanLyKhachHang", tkngmua);
        }
        public ActionResult Products()
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                return RedirectToAction("../../Accounts/Login");
            }

            ViewBag.Name = session_acc.hoten;
            ViewBag.Messager = "Đăng Xuất";
            ViewBag.Login = "../Accounts/Logout";

            var dao = new SanPham_DAO();
            var dao1 = new LoaiSanPham_DAO();
            ViewBag.maloaisp = dao1.get_category_all_();
            var sp = dao.get_product_all_();
            return View(sp);
        }
        #endregion

        #region 13-01
        public ActionResult CreateSanPham(FormCollection f, HttpPostedFileBase[] hinhanh)
        {
            if (f == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (hinhanh == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sp = new SanPham();
            //Kiem Tra hình tồn tại trong csdl chưa
            int loi = 0;
            for (int i = 0; i < hinhanh.Count(); i++)
            {
                if (hinhanh[i] != null)
                {
                    //Kiem Tra noi dung hinh anh
                    if (hinhanh[i].ContentLength > 0)
                    {
                        if (hinhanh[i].ContentType != "image/jpeg" && hinhanh[i].ContentType != "image/png" && hinhanh[i].ContentType != "image/gif" && hinhanh[i].ContentType != "image/tiff" && hinhanh[i].ContentType != "image/BMP" && hinhanh[i].ContentType != "image/jpg")
                        {
                            ViewBag.upload += "Hình Ảnh" + i + "Không hợp lê <br />";
                            loi++;
                        }
                        else
                        {
                            //Lay hinh anh
                            var fileName = Path.GetFileName(hinhanh[i].FileName);
                            //Lấy hình ảnh chuyển vào thư mục hình ảnh
                            var path = Path.Combine(Server.MapPath("~/Source/img-public/"), fileName);
                            //Nếu thư mục chứa hình ảnh đó rồi sẽ xuất ra thông báo 
                            if (System.IO.File.Exists(path))
                            {
                                ViewBag.upload = "Hình " + i + " đã tồn tại";
                                loi++;
                                if (i == 0)
                                {
                                    sp.hinhanh1 = fileName;
                                }
                                else if (i == 1)
                                {
                                    sp.hinhanh2 = fileName;
                                }
                                else if (i == 2)
                                {
                                    sp.hinhanh3 = fileName;
                                }
                                else if (i == 3)
                                {
                                    sp.hinhanh4 = fileName;
                                }
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    hinhanh[i].SaveAs(path);
                                    sp.hinhanh1 = fileName;
                                }
                                else if (i == 1)
                                {
                                    hinhanh[i].SaveAs(path);
                                    sp.hinhanh2 = fileName;
                                }
                                else if (i == 2)
                                {
                                    hinhanh[i].SaveAs(path);
                                    sp.hinhanh3 = fileName;
                                }
                                else if (i == 3)
                                {
                                    hinhanh[i].SaveAs(path);
                                    sp.hinhanh4 = fileName;
                                }
                            }
                        }
                        
                    }
                }
            }
            var a = f["id_loaisp"];
            var b = f["soluong"];
            var c = f["dongia"];
            var d = f["giasale"];
            sp.tensp = f["tensp"];
            sp.id_loaisp = Int32.Parse(a);
            sp.tenngan = f["tenngan"];
            //sp.soluong = Int32.Parse(b);
            double dg;
            double.TryParse(c, out dg);
            sp.dongia = dg;

            double gias;
            double.TryParse(d, out gias);

            sp.giasale = gias;
            sp.trangthai = f["trangthai"];
            sp.hienthi = f["hienthi"];
            sp.tinhtrang = f["tinhtrang"];
            sp.thongtin = f["thongtin"];
            sp.xeploai = f["XepLoai"];
            var dao = new SanPham_DAO();
            var res = dao.set_product(sp);
            if (res != null)
            {
                a = f["chieurong"];
                b = f["chieudai"];
                c = f["chieucao"];
                d = f["cannang"];
                double r;
                double.TryParse(a, out r);
                double dd;
                double.TryParse(b, out dd);
                double cc;
                double.TryParse(c, out cc);
                double n;
                double.TryParse(d, out n);
                TheTich tt = new TheTich();
                tt.id_sanpham = res.id_sanpham;
                tt.chieurong = Convert.ToDouble(r);
                tt.chieudai = Convert.ToDouble(dd);
                tt.chieucao = Convert.ToDouble(cc);
                tt.cannang = Convert.ToDouble(n);
                var dao1 = new TheTich_DAO();
                var res1 = dao1.set_vol(tt);
                if (res1 == 1)
                {
                    ViewBag.notify = "Xoá Thành Cônh";
                    ViewBag.Alert = "alert-info";
                }
                else
                {
                    ViewBag.notify = "Xoá Thất Bại!";
                    ViewBag.Alert = "alert-danger";
                }
            }
            var sp1 = dao.get_product_all();
            return RedirectToAction("Products",sp1);
        }
        public JsonResult Get_TaiKhoan_id(int tk)
        {
            var dao = new TaiKhoan_DAO();
            var res_ = dao.Get_id_taikhoanAdmin(tk);
            return Json(res_, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Get_SanPham_id(int sp)
        {
            var dao = new SanPham_DAO();
            SanPham res_ = dao.get_product_idsanpham(sp);
            var dao1 = new TheTich_DAO();
            TheTich res1 = dao1.get_vol(sp);
            SanPham_TheTich_Model lst = new SanPham_TheTich_Model();
            lst.id_sanpham = res_.id_sanpham;
            lst.tensp = res_.tensp;
            lst.id_loaisp = res_.id_loaisp;
            lst.tenngan = res_.tenngan;
            lst.soluong = res_.soluong;
            lst.dongia = res_.dongia;
            lst.giasale = res_.giasale;
            lst.trangthai = res_.trangthai;
            lst.hienthi = res_.hienthi;
            lst.tinhtrang = res_.tinhtrang;
            lst.thongtin = res_.thongtin;
            lst.xeploai = res_.xeploai;
            lst.chieucao = res1.chieucao;
            lst.chieurong = res1.chieurong;
            lst.chieudai = res1.chieudai;
            lst.cannang = res1.cannang;
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteSanPham(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var dao = new HoaDonCT_DAO();
            var res = dao.get_hdct_all();
            foreach(var item in res)
            {
                var res2 = dao.remove_hdct_idsanpham(item.id_sanpham);
            }
            var dao1 = new SanPham_DAO();
            var res1 = dao1.remove_product(id);
            if (res1 != null)
            {
                ViewBag.notify = "Xoá Thành Công";
                ViewBag.Alert = "alert-info";
            }
            else
            {
                ViewBag.notify = "Xoá Thất Bại!";
                ViewBag.Alert = "alert-danger";
            }
            var sp1 = dao1.get_product_all();
            return RedirectToAction("Products", sp1);
        }
        public ActionResult UpdateSanPham(FormCollection f, HttpPostedFileBase[] hinhanh)
        {
            if (f == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sp = new SanPham();
            var dao = new SanPham_DAO();
            var idsp = f["id_sanpham"];
            var res1 = dao.get_product_idsanpham(Int32.Parse(idsp));
            //Kiem Tra hình tồn tại trong csdl chưa
            int loi = 0;
            for (int i = 0; i < hinhanh.Count(); i++)
            {
                if (hinhanh[i] != null)
                {
                    //Kiem Tra noi dung hinh anh
                    if (hinhanh[i].ContentLength > 0)
                    {
                        if (hinhanh[i].ContentType != "image/jpeg" && hinhanh[i].ContentType != "image/png" && hinhanh[i].ContentType != "image/gif" && hinhanh[i].ContentType != "image/tiff" && hinhanh[i].ContentType != "image/BMP" && hinhanh[i].ContentType != "image/jpg")
                        {
                            ViewBag.upload += "Hình Ảnh" + i + "Không hợp lê <br />";
                            loi++;
                        }
                        else
                        {
                            //Lay hinh anh
                            var fileName = Path.GetFileName(hinhanh[i].FileName);
                            //Lấy hình ảnh chuyển vào thư mục hình ảnh
                            var path = Path.Combine(Server.MapPath("~/Source/img-public/"), fileName);
                            //Nếu thư mục chứa hình ảnh đó rồi sẽ xuất ra thông báo 
                            if (System.IO.File.Exists(path))
                            {
                                ViewBag.upload = "Hình " + i + " đã tồn tại";
                                loi++;
                                if (i == 0)
                                {
                                    sp.hinhanh1 = fileName;
                                }
                                else if (i == 1)
                                {
                                    sp.hinhanh2 = fileName;
                                }
                                else if (i == 2)
                                {
                                    sp.hinhanh3 = fileName;
                                }
                                else if (i == 3)
                                {
                                    sp.hinhanh4 = fileName;
                                }
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    hinhanh[i].SaveAs(path);
                                    sp.hinhanh1 = fileName;
                                }
                                else if (i == 1)
                                {
                                    hinhanh[i].SaveAs(path);
                                    sp.hinhanh2 = fileName;
                                }
                                else if (i == 2)
                                {
                                    hinhanh[i].SaveAs(path);
                                    sp.hinhanh3 = fileName;
                                }
                                else if (i == 3)
                                {
                                    hinhanh[i].SaveAs(path);
                                    sp.hinhanh4 = fileName;
                                }
                            }
                        }

                    }
                }
                else
                {
                    loi++;
                    if (i == 0)
                    {
                        if (res1.hinhanh1 != null)
                        {  
                        sp.hinhanh1 = res1.hinhanh1;
                        }
                        else
                        {
                            sp.hinhanh1 = "loaisp.png";
                        }
                    }
                    else if (i == 1)
                    {
                        if (res1.hinhanh2 != null)
                        {
                            sp.hinhanh2 = res1.hinhanh2;
                        }
                        else
                        {
                            sp.hinhanh2 = "loaisp.png";
                        }
                    }
                    else if (i == 2)
                    {
                        if (res1.hinhanh1 != null)
                        {
                            sp.hinhanh3 = res1.hinhanh3;
                        }
                        else
                        {
                            sp.hinhanh3 = "loaisp.png";
                        }
                    }
                    else if (i == 3)
                    {
                        if (res1.hinhanh1 != null)
                        {
                            sp.hinhanh4 = res1.hinhanh4;
                        }
                        else
                        {
                            sp.hinhanh4 = "loaisp.png";
                        }
                    }
                }
            }
            var dao1 = new HoaDonCT_DAO();
            var res = dao1.get_hdct_all();
            foreach (var item in res)
            {
                var res2 = dao1.remove_hdct_idsanpham(item.id_sanpham);
            }
            var a = f["id_loaisp"];
            var b = f["soluong"];
            var c = f["dongia"];
            var d = f["giasale"];
            var e = f["id_sanpham"];
            double dg;
            double.TryParse(c, out dg);
            double gias;
            double.TryParse(d, out gias);
            var res3 = dao.update_product(Int32.Parse(e), f["tensp"], Int32.Parse(a), f["tenngan"], Int32.Parse(b),dg,gias, f["trangthai"], f["hienthi"], f["tinhtrang"], f["thongtin"], f["XepLoai"]);
            if (res3 != null)
            {
                a = f["chieurong"];
                b = f["chieudai"];
                c = f["chieucao"];
                d = f["cannang"];
                double r;
                double.TryParse(a, out r);
                double dd;
                double.TryParse(b, out dd);
                double cc;
                double.TryParse(c, out cc);
                double n;
                double.TryParse(d, out n);
                var dao2 = new TheTich_DAO();
                var res4 = dao2.update_vol(res3.id_sanpham, r, dd, cc, n);
                if (res4 != null)
                {
                    ViewBag.notify = "Xoá Thành Công";
                    ViewBag.Alert = "alert-info";
                }
                else
                {
                    ViewBag.notify = "Xoá Thất Bại!";
                    ViewBag.Alert = "alert-danger";
                }
            }
            var sp1 = dao.get_product_all();
            return RedirectToAction("Products", sp1);
        }
        public ActionResult QuanLySize_Mau()
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                return RedirectToAction("../../Accounts/Login");
            }

            ViewBag.Name = session_acc.hoten;
            ViewBag.Messager = "Đăng Xuất";
            ViewBag.Login = "../Accounts/Logout";

            var dao = new Color_DAO();
            var res = dao.get_color_all();
            var dao1 = new SanPham_DAO();
            ViewBag.sp = dao1.get_product_all();
            return View(res);
        }
        public ActionResult AddSize_Mau(FormCollection f)
        {
            var dao = new Color_DAO();
            var dao1 = new Size_DAO();
            var id_sanpham = f["id_sanpham"];
            var color = f["color"];
            var size = f["size"];
            var soluong = f["soluong"];
            var dao2 = new SanPham_DAO();
            var res2 = dao2.get_product__(Int32.Parse(id_sanpham));
            if (res2.soluong == null)
            {
                res2.soluong = Int32.Parse(soluong);
            }
            else
            {
                res2.soluong += Int32.Parse(soluong);
            }
            dao2.update_product_model(res2);
            var res1 = dao1.get_size_name(size, Int32.Parse(id_sanpham));
            if (res1 == null)
            {
                var sz=dao1.set_size_(size, Int32.Parse(id_sanpham), Int32.Parse(soluong));
                Color c = new Color();
                c.id_size = sz.id_size;
                c.color1 = color;
                c.soluong = Int32.Parse(soluong);
                dao.set_color(c);
            }
            else
            {
                res1.soluong += Int32.Parse(soluong);
                dao1.update_size(res1);
                Color c = new Color();
                c.id_size = res1.id_size;
                c.color1 = color;
                c.soluong = Int32.Parse(soluong);
                dao.checkupdate_color(c);
            }
            var res = dao.get_color_all();
            return RedirectToAction("QuanLySize_Mau",res);
        }

        [HttpGet]
        public JsonResult Get_Color_id(int id_color)
        {
            var dao = new Color_DAO();
            var res_ = dao.get_color_idColor(id_color);
            return Json(res_, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateSize_Color(FormCollection f)
        {
            if (f == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dao = new Color_DAO();
            var id_color = Int32.Parse(f["id_color"]);
            var id_sanpham = Int32.Parse(f["id_sanpham"]);
            var color = f["color"];
            var size = f["size"];
            var soluong = Int32.Parse(f["soluong"]);
            var res1= dao.get_color_idColor(id_color);
            var dao2 = new SanPham_DAO();
            var res2 = dao2.get_product__(res1.Size.id_sanpham);
            res2.soluong -= res1.soluong;
            res2.soluong += soluong;
            dao2.update_product_model(res2);
            var dao1 = new Size_DAO();
            var res3 = dao1.get_size_name(res1.Size.size,res1.Size.id_sanpham);
            if (res3.size == size)
            {
                res3.soluong -= res1.soluong;
                res3.soluong += soluong;
            }
            else
            {
                var res4 = dao1.get_size_name(size, res1.Size.id_sanpham);
                if(res4 != null)
                {
                    res4.soluong += soluong;
                    res1.id_size = res4.id_size;
                    dao1.update_size(res4);
                }
                else
                {
                    var sz = dao1.set_size_(size, res1.Size.id_sanpham, soluong);
                    res1.id_size = sz.id_size;
                }
            }
            res1.color1 = color;
            res1.soluong = soluong;
            dao.set_color(res1);
            var res = dao.get_color_all();
            return RedirectToAction("QuanLySize_Mau", res);
        }
        public ActionResult DeleteSize_Color(int? id)
        {
            var dao = new Color_DAO();
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var res1 = dao.get_color_idColor(id);
            var dao2 = new SanPham_DAO();
            var res2 = dao2.get_product__(res1.Size.id_sanpham);
            res2.soluong -= res1.soluong;
            if (res2.soluong < 0)
            {
                res2.soluong = 0;
                res2.tinhtrang = "HẾT HÀNG";
            }
            else if (res2.soluong == 0)
            {
                res2.tinhtrang = "HẾT HÀNG";
            }
            dao2.update_product_model(res2);
            var dao1 = new Size_DAO();
            var res3 = dao1.get_size_name(res1.Size.size, res1.Size.id_sanpham);
            res3.soluong -= res1.soluong;
            if (res3.soluong <= 0)
            {
                res3 = dao1.get_size_name(res1.Size.size, res1.Size.id_sanpham);
                dao.remove_color(res1);
                dao1.remove_size(res3);
            }
            else
            {
                dao1.update_size(res3);
                dao.remove_color(res1);
            }
            var res = dao.get_color_all();
            return RedirectToAction("QuanLySize_Mau", res);
        }
        public ActionResult QuanLyHoaDon()
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc == null)
            {
                return RedirectToAction("../../Accounts/Login");
            }

            ViewBag.Name = session_acc.hoten;
            ViewBag.Messager = "Đăng Xuất";
            ViewBag.Login = "../Accounts/Logout";

            var dao = new HoaDon_DAO();
            var lst = dao.get_hoadon_damua_all();
            return View(lst);
        }
        public ActionResult HoaDonChiTiet(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dao = new HoaDon_DAO();
            var res = dao.get_hoadon_id(id);
            if (res == null)
            {
                return HttpNotFound();
            }
            var dao1 = new HoaDonCT_DAO();
            var res1 = dao1.get_hdct_hoadon(id);
            ViewBag.ListHoaDonChiTiet = res1;
            return View(res);
        }

        #endregion
    }
}