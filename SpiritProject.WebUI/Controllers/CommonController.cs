using SpiritProject.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpiritProject.WebUI.Controllers
{
    public class CommonController : BaseController
    {
        //
        // GET: /Common/

        public JsonResult CheckSession()
        {
            try
            {
                MemberModel appSysData = (MemberModel)Session["CurrentUser"];

                if (appSysData == null)
                    return Json(false);
                else
                    return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }


    }
}
