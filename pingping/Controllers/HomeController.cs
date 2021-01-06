using MailChimp;
using MailChimp.Helper;
using Modal.DAO;
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
            ViewBag.Title = "Home Page";


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
