using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpiritProject.WebUI.Controllers
{
    public class QuestionController : BaseController
    {
        //
        // GET: /Question/

        public ActionResult UpdateQuestion(string id)
        {
            ViewBag.Id = id;
            return View();
        }

    }
}
