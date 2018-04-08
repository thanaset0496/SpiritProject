using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpiritProject.WebUI.Controllers
{
    public class ChoiceController : BaseController
    {
        //
        // GET: /Choice/

        public ActionResult UpdateChoice(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }

    }
}
