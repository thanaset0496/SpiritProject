using ActionMailer.Net.Mvc;
using SpiritProject.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpiritProject.WebUI.Controllers
{
    public class MailController : MailerBase
    {
        //
        // GET: /Mail/

        public ActionResult Index()
        {
            return View();
        }

        public EmailResult Email(MemberModel member, string type)
        {
            To.Add(member.Email);
            if (type == "FirsTimeLogin")
            {
                Subject = ConfigurationManager.AppSettings["MailSubjectFirstTimeLogin"];
            }
            else if (type == "ForgotPassword")
            {
                Subject = ConfigurationManager.AppSettings["MailSubjectForgotPassword"];
            }
            ViewBag.TypeEmail = type;
            return Email("Email", member);
        }

    }

}
