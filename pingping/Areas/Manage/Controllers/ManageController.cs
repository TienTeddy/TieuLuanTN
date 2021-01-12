using Modal.DAO;
using Modal.EF;
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
            return View();
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

        #region new
        public ActionResult QuanLyLoaiSanPham()
        {
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
                ViewBag.notify = "Xoá Thành Cônh";
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
                ViewBag.notify = "Xoá Thành Cônh";
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
            var dao = new SanPham_DAO();
            var sp = dao.get_product_all();
            //var dao1 = new TheTich_DAO();
            //var dao_ = new LoaiSanPham_DAO();
            //var lsp = dao_.get_category_all();

            //var model = new SanPham_LoaiSanPham_Model();
            //model.loaisp_ = lsp;
            //model.sanpham_ = sp;
            //return View(model);
            return View(sp);
        }
        #endregion
    }
}