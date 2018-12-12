using System.Web;
using System.Web.Mvc;

namespace Api
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {

            filters.Add(new HandleErrorAttribute());
            //________________PRODUCTION CODE______________________________
            //filters.Add(new RequreSecureConnectionFilter());
            //if (!HttpContext.Current.IsDebuggingEnabled)
            //    filters.Add(new RequireHttpsAttribute());

        }
    }
}
