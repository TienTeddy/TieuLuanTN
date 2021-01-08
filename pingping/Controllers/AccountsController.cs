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

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Register_Model model)
        {
            if (model != null)
            {
                var dao_tk = new TaiKhoan_DAO();
                var res_tk = dao_tk.create_taikhoan(model.username,model.password,model.hoten,model.email);
                if (res_tk != null)
                {
                    var dao_nm = new NguoiMua_DAO();
                    var res_nm = dao_nm.create_nguoimua(res_tk.id_taikhoan,Int32.Parse(model.phone));
                    if (res_nm != null)
                    {
                        //add cookie
                        HttpCookie newCookie = new HttpCookie("Account-Cookie");
                        newCookie["username"] = model.username.ToString();
                        newCookie.Expires = DateTime.Now.AddDays(365);
                        newCookie["password"] = model.password.ToString();
                        newCookie.Expires = DateTime.Now.AddDays(365);
                        Response.AppendCookie(newCookie);

                        //add sessionHelper
                        SessionHelper.SetSession(new AccLogin() 
                        {
                            id_taikhoan = res_tk.id_taikhoan,
                            id_nguoi = res_nm.id_nguoimua,
                            phone = res_nm.phone,
                            street = res_nm.street,
                            ward = res_nm.ward,
                            dictrict = res_nm.district,
                            province = res_nm.province,
                            username = res_tk.username,
                            password = res_tk.password,
                            password_old = res_tk.password_old,
                            email = res_tk.email,
                            loaitk = res_tk.loaitk,
                            hoten = res_tk.hoten
                        });
                        return RedirectToAction("Index", "Home");
                    }

                }
                ModelState.AddModelError("", "Đăng Ký Không Thành Công!"); //-sumary
                return View("Register", model);
            }
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
        [HttpGet]
        public ActionResult MyAccount()
        {
            var acc = SessionHelper.GetSession();
            if (acc == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(acc);
        }
        [HttpPost]
        public ActionResult ChangeAccounts(FormCollection f)
        {

            var acc = SessionHelper.GetSession();
            if (acc != null)
            {
                if (f == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var dao = new TaiKhoan_DAO();
                    var res_tk = dao.update_account(f["email"].ToString(), f["hoten"].ToString(), acc.id_taikhoan);
                    if (res_tk != null)
                    {
                        var dao_1 = new NguoiMua_DAO();
                        var res_nm = dao_1.update_nguoimua(f["street"].ToString(), f["ward"].ToString(), f["dictrict"].ToString(), f["province"].ToString(), Int32.Parse(f["phone"]), acc.id_taikhoan);
                        if (res_nm != null)
                        {
                            ViewBag.notify = "Thay Đổi Thành Công";
                            ViewBag.Alert = "alert-info";
                            SessionHelper.SetSession(new AccLogin()
                            {
                                id_taikhoan = res_tk.id_taikhoan,
                                id_nguoi = res_nm.id_nguoimua,
                                phone = res_nm.phone,
                                street = res_nm.street,
                                ward = res_nm.ward,
                                dictrict = res_nm.district,
                                province = res_nm.province,
                                username = res_tk.username,
                                password = res_tk.password,
                                password_old = res_tk.password_old,
                                email = res_tk.email,
                                loaitk = res_tk.loaitk,
                                hoten = res_tk.hoten

                            });
                            acc = SessionHelper.GetSession();
                        }
                        else
                        {
                            ViewBag.notify = "Thay Đổi Thất Bại";
                            ViewBag.Alert = "alert-danger";
                        }
                    }
                    else
                    {
                        ViewBag.notify = "Thay Đổi Thất Bại";
                        ViewBag.Alert = "alert-danger";
                    }
                }

            }
            return View("MyAccount", acc);
        }
        public ActionResult ChangePassword(FormCollection f)
        {

            var acc = SessionHelper.GetSession();
            if (acc != null)
            {
                if (f["password_old"] != acc.password)
                {
                    ViewBag.notify = "Mật khẩu không đúng!";
                    ViewBag.Alert = "alert-danger";
                    return View("MyAccount", acc);

                }
                else if (f["password"] == acc.password_old)
                {
                    ViewBag.notify = "Bạn đã sử dụng mật khẩu này!";
                    ViewBag.Alert = "alert-danger";
                    return View("MyAccount", acc);
                }
                else
                {
                    var dao_1 = new TaiKhoan_DAO();
                    var res_nm = dao_1.update_pass(acc.id_taikhoan, f["password"].ToString());
                    if (res_nm != null)
                    {
                        ViewBag.notify = "Đổi mật khẩu thành công";
                        ViewBag.Alert = "alert-info";
                        SessionHelper.SetSession(new AccLogin()
                        {
                            id_taikhoan = acc.id_taikhoan,
                            id_nguoi = acc.id_nguoi,
                            phone = acc.phone,
                            street = acc.street,
                            ward = acc.ward,
                            dictrict = acc.dictrict,
                            province = acc.province,
                            username = acc.username,
                            password = res_nm.password,
                            password_old = res_nm.password_old,
                            email = acc.email,
                            loaitk = acc.loaitk,
                            hoten = acc.hoten

                        });
                        acc = SessionHelper.GetSession();
                    }
                    else
                    {
                        ViewBag.notify = "Đổi mật khẩu thất bại";
                        ViewBag.Alert = "alert-danger";
                    }
                }

            }
            return View("MyAccount", acc);
        }
    }
}