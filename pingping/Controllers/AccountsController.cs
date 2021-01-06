using Modal.DAO;
using pingping.Common;
using pingping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace pingping.Controllers
{
    public class AccountsController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        // GET: Account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login_Models Login)
        {
            var dao = new TaiKhoan_DAO();
            int result = dao.CheckLogin(Login.username, Login.password);
            if (result > 0)
            {
                // add session
                if (Login.checkbox == true)
                {
                    HttpCookie newCookie = new HttpCookie("Account-Cookie");
                    newCookie["username"] = Login.username.ToString();
                    newCookie.Expires = DateTime.Now.AddDays(365);
                    newCookie["password"] = Login.password.ToString();
                    newCookie.Expires = DateTime.Now.AddDays(365);
                    Response.AppendCookie(newCookie);
                }
                //ViewBag.Message = "true";

                if (result == 0) //saller
                {
                    var dao1 = new NguoiBan_DAO();
                    var res = dao.Get_id_taikhoan(Login.username, Login.password);
                    var res_infoSaller = dao1.get_infor(res.id_taikhoan);
                    SessionHelper.SetSession(new AccLogin() {
                        id_taikhoan = res.id_taikhoan,
                        id_nguoi =res_infoSaller.id_nguoiban,
                        phone = res_infoSaller.phone,
                        street =res_infoSaller.street,
                        ward = res_infoSaller.ward,
                        dictrict = res_infoSaller.district,
                        province = res_infoSaller.province,
                        taikhoanng = res_infoSaller.taikhoanng,
                        nganhang = res_infoSaller.nganhang,
                        username =res.username,
                        password=res.password,
                        password_old=res.password_old,
                        email=res.email,
                        loaitk = res.loaitk,
                        hoten=res.hoten
                    });
                    return RedirectToAction("Index", "Manage/Manage/Index");
                }
                else if (result == 1) //manager
                {
                    var dao1 = new NguoiMua_DAO();
                    var res = dao.Get_id_taikhoan(Login.username, Login.password);
                    var res_infoSaller = dao1.get_infor(res.id_taikhoan);
                    SessionHelper.SetSession(new AccLogin()
                    {
                        id_taikhoan = res.id_taikhoan,
                        id_nguoi = res_infoSaller.id_nguoimua,
                        phone = res_infoSaller.phone,
                        street = res_infoSaller.street,
                        ward = res_infoSaller.ward,
                        dictrict = res_infoSaller.district,
                        province = res_infoSaller.province,
                        username = res.username,
                        password = res.password,
                        password_old = res.password_old,
                        email = res.email,
                        loaitk = res.loaitk,
                        hoten = res.hoten
                    });
                    return RedirectToAction("Index", "Home");
                }
                return View("Login", Login);
            }
            else //fail login
            {
                ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu!"); //-sumary
                return View("Login", Login);
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}