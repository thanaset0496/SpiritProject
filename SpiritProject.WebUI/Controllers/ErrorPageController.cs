using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpiritProject.WebUI.Controllers
{
    public class ErrorPageController : Controller

    {

        public ActionResult Error(int statusCode, Exception exception)

        {

            Response.StatusCode = statusCode;

            ViewBag.StatusCode = statusCode + " Error";
            if (statusCode == 404)
                return RedirectToAction("Error_404");

            else if (statusCode == 505)
                return RedirectToAction("Error_505");
            else
                return View();

        }

        public ActionResult Error_404()
        {
            return View();

        }


        public ActionResult Error_500()
        {
            return View();

        }
    }
}
