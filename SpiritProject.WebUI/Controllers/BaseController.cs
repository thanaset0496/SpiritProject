using SpiritProject.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpiritProject.WebUI.Controllers
{
    public class BaseController : Controller
    {
        public MemberModel CurrentUser
        {
            get
            {
                return (MemberModel)HttpContext.Session["CurrentUser"]; //Session
            }
        }

    }
}
