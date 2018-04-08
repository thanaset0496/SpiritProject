using SpiritProject.WebUI.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SpiritProject.WebUI.ActionFilters
{
    public class InternationalizationAttribute : ActionFilterAttribute
    {
        private readonly IList<string> _supportedLocales;
        private readonly string _defaultLang;

        public InternationalizationAttribute()
        {
            // Get supported locales list
            _supportedLocales = LocalizationHelper.GetSupportedLocales();

            // Set default locale
            _defaultLang = _supportedLocales[0];
        }

        /// <summary>
        /// Apply locale to current thread
        /// </summary>
        /// <param name="lang">locale name</param>
        protected void SetLang(string culture)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Get locale from route values
            string lang = (string)filterContext.RouteData.Values["culture"] ?? _defaultLang;

            // If we haven't found appropriate culture - seet default locale then
            if (!_supportedLocales.Contains(lang))
                lang = _defaultLang;

            SetLang(lang);
        }
    }

}