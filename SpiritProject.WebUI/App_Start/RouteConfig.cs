using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SpiritProject.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            //routes.MapRoute(
            // name: "AdminDefaultLocalized",
            // url: "{lang}/{controller}/{action}/{id}",
            // defaults: new
            // {
            //     controller = "Admin",
            //     action = "Login",
            //     id = UrlParameter.Optional,
            //     lang = "en"
            // });


            //routes.MapRoute(
            //  name: "AdminDefaultLocalized2",
            //  url: "{lang}/Admin",
            //  defaults: new
            //  {
            //      action = "Login",
            //      id = UrlParameter.Optional,
            //      lang = "en"
            //  });

            //routes.MapRoute(
            //name: "AdminDefaultLocalized3",
            //url: "Admin",
            //defaults: new
            //{
            //    action = "Login",
            //    id = UrlParameter.Optional,
            //    lang = "en"
            //});


            routes.MapRoute(
               name: "DefaultLocalized",
               url: "{culture}/{controller}/{action}/{id}",
               defaults: new
               {
                   culture = "th",
                   controller = "Member",
                   action = "Login",
                   id = UrlParameter.Optional
               }, constraints: new { culture = "[a-z]{2}" });


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { culture = "th", controller = "Member", action = "Login", id = UrlParameter.Optional }
                //defaults: new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}