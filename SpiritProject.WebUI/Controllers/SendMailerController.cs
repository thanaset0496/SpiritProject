using SpiritProject.Common;
using SpiritProject.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Configuration;
using System.Net.Configuration;
using SpiritProject.WebUI.Exceptions;

namespace SpiritProject.WebUI.Controllers
{

    // GET: /SendMailer/

    public class SendMailerController : BaseController
    {
        //  
        // GET: /SendMailer/   
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult SendMail(MemberModel _objModelMail, String type)
        {
            try
            {
                var From = ConfigurationManager.AppSettings["EmailFrom"];
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(From);
                mailMessage.To.Add(_objModelMail.Email);
                string Subject = "";
                if (type == "FirstTimeLogin")
                {
                    Subject = ConfigurationManager.AppSettings["MailSubjectFirstTimeLogin"];
                }
                else if (type == "ForgotPassword")
                {
                    Subject = ConfigurationManager.AppSettings["MailSubjectForgotPassword"];
                }
                var config = Config.GetConfig(type);
                string HtmlBody = "";
                if (config != null)
                    HtmlBody = config.Value;
                else
                    HtmlBody = "";

                var FullNameTH = _objModelMail.MemberNameTH + " " + _objModelMail.MemberLastNameTH;
                var FullNameEN = _objModelMail.MemberNameEN + " " + _objModelMail.MemberLastNameEN;
                HtmlBody = HtmlBody.Replace("@Password", _objModelMail.Password);
                HtmlBody = HtmlBody.Replace("@ToTH", FullNameTH);
                HtmlBody = HtmlBody.Replace("@ToEN", FullNameEN);


                //string HtmlBody1 = @"<label>เรียนคุณ</label> " + _objModelMail.FullName + @" <br />
                //<label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ขอต้อนรับเข้าสู่ระบบ Personality Assessment</label><br>
                //<label>คุณได้เข้าสู่ระบบเป็นครั้งแรกดังนั้นการเข้าสู่ระบบขอให้ผู้ใช้งานนำรหัสผ่านดังนี้ " + _objModelMail.Password + @" ใช้สำหรับ </label><br />
                //<label>เข้าสู่ระบบในครั้งแรกและระบบจะแนะนำให้ผู้ใช้งานเปลี่ยนเป็นรหัสผ่านที่ใช้สำหรับใช้งานจริงต่อไป </label><br />";

                //string HtmlBody2 = @"<label>เรียนคุณ</label>" + _objModelMail.FullName + @"<br />
                //<label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ขอต้อนรับเข้าสู่ระบบ Personality Assessment</label><br>
                //<label>ระบบ Personality Assessment สร้างรหัสการใช้งานใหม่ ให้ผู้ใช้งาน แล้วดังนี้ " + _objModelMail.Password + @"</label><br>
                //<label>ผู้ใช้งานสามารถนำรหัสผานดังนี้ไปใช้สำหรับเข้าระบบได้ทันที และทางระบบนำให้ท่านเปลี่ยนเป็นรหัสที่</label><br>
                //<label>ต้องการใช้งานต่อไปภายในระบบได้ตามต้องการ</label>
                //    <br>";

                mailMessage.Subject = Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = HtmlBody;

                SmtpClient smtp = new SmtpClient();
                var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                //smtp.Host = "pttapp2smtp.ptt.corp";
                //smtp.Port = 25;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = false;


                smtp.Send(mailMessage);
                LogActivity.Log(new MemberModel() { MemberId = "TestSendMail" }, "PASS", "TestSendMail");

                //  }
            }
            catch (Exception ex)
            {
                LogActivity.Log(new MemberModel() { MemberId = "TestSendMail" }, ex.Message, "TestSendMail");
                throw new CommonException("Send Mail Fail");
            }
            //else
            //{
            //    return View();
            //}

            //  return RedirectToAction("Login", "Member");
            return View("Index", _objModelMail);
        }

        private void GetConfig(string v)
        {
            throw new NotImplementedException();
        }

        public ActionResult AdminForm()
        {
            string ip = Request.UserHostAddress;
            LogActivity.Log(new MemberModel() { MemberId="CheckUsedFunction"}, "ForgetPassword", ip);
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AdminFormSubmit(String Username)
        {
            try
            {
                var admin = Member.GetMember(Username);
                if (admin != null)
                {
                    string password = Member.GenerateNewPassword(admin.MemberId);
                    admin.Password = password;
                    try
                    {
                        SendMail(admin, "ForgotPassword");
                    }
                    catch (Exception ex)
                    {
                        LogActivity.Log(new MemberModel() { MemberId = "TestSendMail" }, ex.Message, "TestSendMail");
                        return Json(new { result = "Sand Mail Fail" });

                    }
                    LogActivity.Log(new MemberModel() { MemberId = "TestSendMail" }, "PASS", "TestSendMail");
                    return Json(new { result = "success", Email = admin.Email });
                }
                else
                    return Json(new { result = "Not Found User" });
            }
            catch (Exception ex)
            {
                LogActivity.Log(new MemberModel() { MemberId = "TestSendMail" }, ex.Message, "TestSendMail");
                return Json(new { result = "Sand Mail Fail" });

            }

        }

        public ActionResult MemberForm()
        {
            string ip = Request.UserHostAddress;
            LogActivity.Log(new MemberModel() { MemberId = "CheckUsedFunction" }, "ForgetPassword", ip);
            return View();

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult MemberFormSubmit(String Username)
        {
            try
            {
                long isInt = 0;
                if (long.TryParse(Username, out isInt))
                {
                    var ExistedMember = Member.GetMember(Username);
                    if (ExistedMember != null)
                    {
                        var member = ERecruitment.GetMail(Username);
                        if (member != null)
                        {
                            Member.UpdateMail(member.PersonalID, member.Email);

                            string password = Member.GenerateNewPassword(member.PersonalID);
                            //model.Password = password;
                            ExistedMember.Password = password;
                            try
                            {
                                SendMail(ExistedMember, "ForgotPassword");
                            }
                            catch (Exception ex)
                            {
                                LogActivity.Log(new MemberModel() { MemberId = "TestSendMail" }, ex.Message, "TestSendMail");
                                return Json(new { result = "Sand Mail Fail" });
                            }
                            LogActivity.Log(new MemberModel() { MemberId = "TestSendMail" }, "PASS", "TestSendMail");
                            return Json(new { result = "success", Email = member.Email });
                        }
                        return Json(new { result = "Not Found User" });
                    }
                    else
                        return Json(new { result = "Not Found User" });
                }
                else
                    return Json(new { result = "Error" });
            }
            catch (Exception ex)
            {
                LogActivity.Log(new MemberModel() { MemberId = "TestSendMail" }, ex.Message, "TestSendMail");
                return Json(new { result = "Sand Mail Fail" });

            }

        }

        public ActionResult SendEmail(MailModel obj)
        {

            try
            {
                //Configuring webMail class to send emails  
                //gmail smtp server  
                WebMail.SmtpServer = "smtp.gmail.com";
                //gmail port to send emails  
                WebMail.SmtpPort = 587;
                WebMail.SmtpUseDefaultCredentials = true;
                //sending emails with secure protocol  
                WebMail.EnableSsl = true;
                //EmailId used to send emails from application  
                WebMail.UserName = "YourGamilId@gmail.com";
                WebMail.Password = "YourGmailPassword";

                //Sender email address.  
                WebMail.From = "SenderGamilId@gmail.com";

                //Send email  
                WebMail.Send(to: "to", subject: "titel", body: "body", cc: "cc", isBodyHtml: true);
                ViewBag.Status = "Email Sent Successfully.";
            }
            catch (Exception)
            {
                ViewBag.Status = "Problem while sending email, Please check details.";

            }
            return View();
        }

    }
}
