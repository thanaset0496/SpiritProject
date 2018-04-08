using SpiritProject.WebUI.Filter;
using System.Web;
using System.Web.Mvc;

namespace SpiritProject.WebUI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            filters.Add(new FilterException());
        }
    }
}