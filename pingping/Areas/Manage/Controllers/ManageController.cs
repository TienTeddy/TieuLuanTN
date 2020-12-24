using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}