using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SpiritProject.Common;
using SpiritProject.Services;
using System.Threading;
using System.Globalization;
using SpiritProject.WebUI.ActionFilters;

namespace SpiritProject.WebUI.Controllers
{

    public class MemberController : BaseController
    {
        Member memberServices = new Member();
        //  string Culture = "th-TH";



        //
        // GET: /Member/

        public ActionResult Login(string Message = "")
        {
            //Hash.EncodeQuestion();
            //Hash.EncodeChoiceWG();
            //Hash.EncodeChoiceFake();

            //SendMailerController sm = new SendMailerController();
            //sm.SendMail(new MailModel());
            //var testView = ERecruitment.CheckERecruitment();
            // Services.Hash.EncodeChoiceWG();
            ViewBag.Message = Message;
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Login", "Member");
        }

        public ActionResult NewLogin()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(string username, string password)
        {

            Session.Clear();
            MemberModel login = new MemberModel();
            /// check login
            /// 
            //if (username == "Admin")
            //{
            //    return RedirectToAction("Index", "Admin");

            //}
            //else
            //{
            if (RuntimeVariable.CurrentUser != null)
            {
                if (username != RuntimeVariable.CurrentUser.MemberId)
                {
                    //   return View("~/Views/Shared/ErrorLogin.cshtml");
                    return RedirectToAction("Error_One", "CommonViews");
                }
                else if (username == "Admin")
                {
                    return RedirectToAction("Index", "Admin");

                }
                //    }
            }

            login = Services.Member.CheckLoginUser(username, password);

            if (login != null)
            {
                if (!String.IsNullOrEmpty(login.Message))
                {
                    return Json(new
                    {
                        status = -1,
                        message = login.Message
                    });
                }
                if (login.FailLogin > 0)
                {
                    if (login.FailLogin >= 3)
                    {
                        return Json(new
                        {
                            status = -1,
                            message = "Wrong 3 Times Password"
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            status = -1,
                            message = "Wrong Password"
                        });
                    }
                }
                else
                {
                    RuntimeVariable.CurrentUser = new MemberModel();
                    RuntimeVariable.CurrentUser.MemberId = login.MemberId;
                    RuntimeVariable.CurrentUser.MemberNameTH = login.MemberNameTH;
                    RuntimeVariable.CurrentUser.MemberNameEN = login.MemberNameEN;
                    RuntimeVariable.CurrentUser.MemberLastNameTH = login.MemberLastNameTH;
                    RuntimeVariable.CurrentUser.MemberLastNameEN = login.MemberLastNameEN;
                    RuntimeVariable.CurrentUser.MemberDOB = login.MemberDOB;
                    RuntimeVariable.CurrentUser.Role = login.Role;
                    //if(Culture == "th-TH")
                    //{
                    //    RuntimeVariable.CurrentUser.MemberName = login.MemberNameTH;
                    //    RuntimeVariable.CurrentUser.MemberLastName = login.MemberLastNameTH;
                    //}
                    //else 
                    //{
                    RuntimeVariable.CurrentUser.MemberName = login.MemberNameEN;
                    RuntimeVariable.CurrentUser.MemberLastName = login.MemberLastNameEN;
                    RuntimeVariable.CurrentUser.FailLogin = login.FailLogin;
                    //}
                    //   return RedirectToAction("Dashboard", "Index");
                    if (login.Role == "1")
                    {
                        return RedirectToAction("Index", "Admin");

                    }
                    else
                    {
                        if (login.Status == 0)
                        {

                            //  Services.Member.CreateNewPassword(login);
                           // return RedirectToAction("NewLogin", "Member");
                            return Json(new
                            {
                                status = 2
                            });

                        }
                        return Json(new
                        {
                            status = 1
                        });

                        //return RedirectToAction("Index", "Dashboard");

                    }
                }

            }
            else
            {
                return Json(new
                {
                    status = -2
                });
                // return RedirectToAction("Login", "Member", new { message = "Citizen Id or Password incorrect" });

            }

            //  return RedirectToAction("Login", "Login");
        }

        public ActionResult Register()
        {
            string ip = Request.UserHostAddress;
            LogActivity.Log(new MemberModel() { MemberId = "CheckUsedFunction" }, "Register", ip);
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AuthenCheck(string username, string dob)
        {
            try
            {
                long isInt = 0;
                if (long.TryParse(username, out isInt))
                {
                    //return RedirectToAction("Login", "Member");
                    //step 1
                    ///check in base
                    SendMailerController sendMail = new SendMailerController();
                    MailModel mail = new MailModel();
                    MemberModel member = Services.Member.CheckNewLoginUser(username.Replace("-", ""), dob);
                    if (member.Status == 0)
                    {
                        try
                        {
                            sendMail.SendMail(member, "FirstTimeLogin");
                            return Json(new { data = "success", Email = member.Email });
                        }
                        catch
                        {
                            return Json(new { data = "mailFail" });
                        }
                    }
                    else if (member.Status == -2)
                        return Json(new { data = "duplicate" });
                    else if (member.Status == -3)
                        return Json(new { data = "notexisted" });
                    else if (member.Status == -4)
                        return Json(new { data = "wrongpassword" });
                    else
                        return Json(new { data = "fail" });
                }
                else
                    return Json(new { data = "fail" });

            }
            catch
            {
                return Json(new { data = "fail" });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateNewPassword(string username, string password, string newpassword)
        {

            ///update new password
            ///
            var result = Services.Member.CreateNewPassword(username.Replace("-", ""), password, newpassword);

            return RedirectToAction("Login", "Member");
        }

        [Utilities.SessionAuthen]
        [HttpPost]
        public ActionResult UpdateNewPassword(string username, string newpassword)
        {

            ///update new password
            ///

            string response = "";

            var result = Services.Member.UpdateNewPassword(RuntimeVariable.CurrentUser.MemberId, newpassword);
            if (result > 0) response = "Success";
            else response = "Fail";

            return Json(new
            {
                result
                = response
            });
        }

        public ActionResult ChangePassword()
        {

            return View();
        }

        [Utilities.SessionAuthen]
        [HttpPost]
        public ActionResult UpdatePassword(string username, string oldpassword, string newpassword)
        {

            ///update new password
            ///
            var result = Services.Member.UpdatePassword(RuntimeVariable.CurrentUser.MemberId, oldpassword, newpassword);

            return Json(new { result = result });
        }
    }
}
