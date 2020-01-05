using AFModels;
using BDS.DAO;
using BDS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BDS.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        BatDongSanContext db = new BatDongSanContext();
        LoginDAO loginDao = new LoginDAO();
        [Authorize]
        public ActionResult Index()
        {
            if (Session["User"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            string sTaiKhoan = f["txtTaiKhoan"].ToString();
            string sMatKhau = f.Get("txtMatKhau").ToString();
            QuanTri qt = (from a in db.QuanTris
                          where a.UserName == sTaiKhoan && a.Pw == sMatKhau
                          select a).SingleOrDefault();
            if (qt != null)
            {
                if (qt.MaPQ == 1)
                {
                    ViewBag.ThongBao = "Hi, Admin:" +qt.UserName;
                    FormsAuthentication.SetAuthCookie(qt.UserName, false);
                    Session["User"] = qt.UserName;
                    Session["Pw"] = qt.Pw;
                    return RedirectToAction("Index","Login");
                }
                else
                {
                    ViewBag.ThongBao = "Hi " + qt.UserName;
                    FormsAuthentication.SetAuthCookie(qt.UserName, false);
                    Session["User"] = qt.UserName;
                    Session["Pw"] = qt.Pw;
                    return RedirectToAction("Index", "Login");
                }
            }
            ViewBag.ThongBao = "Tên tài khoản hoặc mật khẩu không đúng!";
            return View();
        }


        public ActionResult ChangePass()
        {
            return View();
        }



        [HttpPost]
        public ActionResult ChangePass(FormCollection f)
        {
            string sUser = Session["User"].ToString();
            string sPw = Session["Pw"].ToString();
            string sMatKhauCu = f.Get("txtMatKhau").ToString();
            string reMatKhau = f.Get("txtreMatKhau").ToString();
            QuanTri qt = (from a in db.QuanTris
                          where a.UserName == sUser && a.Pw == sMatKhauCu
                          select a).SingleOrDefault();
            if (qt != null)
            {
                if(sPw==sMatKhauCu)
                { 

                if (ModelState.IsValid)
                    {
                        var dao = new QuanTriDAO();
                        if (dao.ChangePw(qt, reMatKhau))
                        {
                            Session["Pw"] = reMatKhau;
                            return RedirectToAction("Index", "Login");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Cập nhật không thành công");
                        }
                    }
            }
                return View("ChangePass");
            }
            ViewBag.ThongBao = "Mật khẩu không đúng!";
            return View();
        }


        public ActionResult Logout()    
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }





    }
}