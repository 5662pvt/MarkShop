using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MarkShop
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Application["SoNgTruyCap"] = 0;
            Application["SoNgDangTruyCap"] = 0;

        }
        protected void Session_Start()
        {
            Application.Lock();
            Application["SoNgTruyCap"] = (int)Application["SoNgTruyCap"] + 1;
            Application["SoNgDangTruyCap"] = (int)Application["SoNgDangTruyCap"] + 1;
            Application.UnLock();
        }
        protected void Session_End()
        {
            Application.Lock();
            Application["SoNgDangTruyCap"] = (int)Application["SoNgDangTruyCap"] - 1;
            Application.UnLock();
        }
    }
}
