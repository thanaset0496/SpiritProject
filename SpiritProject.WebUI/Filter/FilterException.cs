using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SpiritProject.WebUI.Filter
{
    public class FilterException : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
           
            if (filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.Exception != null && filterContext.Exception.GetType().ToString() == "SpiritProject.WebUI.Exceptions.CommonException")
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {

                        ExceptionType = filterContext.Exception.GetType().ToString(),
                        Massege = filterContext.Exception.Message,

                    }
                };
                filterContext.ExceptionHandled = true;
            }
            //else if (filterContext.Exception != null && filterContext.Exception.GetType().ToString() != "SpiritProject.WebUI.Exceptions.CommonException")
            //{
            //    var StatusCode = ((HttpException)filterContext.Exception).GetHttpCode();
            //    if (StatusCode == 404)
            //    {
            //        filterContext.Result = new RedirectResult("~/ErrorPage/Error_404", true);
            //    }
            //    else if (StatusCode == 500)
            //    {
            //        filterContext.Result = new RedirectResult("~/ErrorPage/Error_500", true);
            //    }
            //    //Logger loging = new Logger();
            //    //loging.Error(filterContext.Exception.GetType().ToString() + " : " + filterContext.Exception.Message, filterContext.Exception);

            //    //filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //    //filterContext.Result = new JsonResult
            //    //{
            //    //    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            //    //    Data = new
            //    //    {
            //    //        ExceptionType = filterContext.Exception.GetType().ToString(),
            //    //        Massege = filterContext.Exception.Message,

            //    //    }
            //    //};
            //    //filterContext.ExceptionHandled = true;
            //    //    base.OnException(filterContext);
            //}
        }
    }
}