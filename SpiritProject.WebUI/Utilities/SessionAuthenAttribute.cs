using SpiritProject.Common;
using SpiritProject.Services;
using SpiritProject.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SpiritProject.WebUI.Utilities
{
    public sealed class SessionAuthenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.Controller.GetType().BaseType == typeof(BaseController))
            {
                BaseController controller = (BaseController)filterContext.Controller;
                if (controller.CurrentUser == null)
                {
                    if (controller.Request.IsAjaxRequest())
                    {
                        filterContext.HttpContext.Response.StatusCode = 444;
                        filterContext.Result = new JsonResult() { Data = new { message = "Session Timeout", url = controller.Url.Action("Login", "Member") } };
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult("~/Member/Login", true);

                    }
                }
                else
                {
                    var ip = HttpContext.Current.Request.UserHostAddress;
                    LogActivity.Log(controller.CurrentUser, filterContext.ActionDescriptor.ActionName, ip);

                }
            }

        
            //base.OnActionExecuting(filterContext);
            //if (filterContext.HttpContext.Request.UrlReferrer == null ||
            //       filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
            //{
            //    filterContext.Result = new RedirectResult("~/Login", true);

            //    //filterContext.Result = new RedirectToRouteResult(new
            //    //               RouteValueDictionary(new { controller = "Member", action = "Login", area = "" }));
            //}
            //MemberModel member = (MemberModel)filterContext.HttpContext.Session["CurrentUser"];

            //if (member == null)
            //{
            //    filterContext.Result = new RedirectResult("~/", true);
            //}
            //else if (string.IsNullOrEmpty(member.MemberId))
            //{
            //    filterContext.Result = new RedirectResult("~/", true);
            //}
            //else if (RuntimeVariable.CurrentUser == null)
            //{
            //    filterContext.Result = new RedirectResult("~/", true);
            //}
            //else
            //{
            //    var ip = HttpContext.Current.Request.UserHostAddress;
            //    LogActivity.Log(member, filterContext.ActionDescriptor.ActionName, ip);

            //}

        }
    }
}