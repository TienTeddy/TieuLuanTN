using MailChimp;
using MailChimp.Helper;
using Modal.DAO;
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
            return View();
        }

        [HttpGet]
        public JsonResult Get_Category()
        {
            var dao = new LoaiSanPham_DAO();
            var result = dao.get_category_all();
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

        public ActionResult ProductPage()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Set_CheckOut(List<CheckOut_Model> data)
        {
            var session_acc = SessionHelper.GetSession();
            if (session_acc != null)
            {
                int soluong = 0;
                var dao_hd = new HoaDon_DAO();
                var dao_sp = new SanPham_DAO();
                var dao_hdct = new HoaDonCT_DAO();

                var res_hd = dao_hd.createHoaDon(session_acc.id_nguoi); //return hoadon
                foreach (CheckOut_Model item in data)
                {
                    var res_sp = dao_sp.get_product_(item.id); //sp
                    
                    var res_hdct = dao_hdct.createHoaDonCT(res_hd.id_hoadon,item.id,item.price,res_hd.thoigian,item.quantity);
                    soluong += 1;
                }
                int res_ = dao_hd.updateHoaDon_quantity(soluong); //return 1 or 0
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
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
            return View();
        }

    }
}
