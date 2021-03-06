﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Authentication
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                "Default" // Route name
                , "{action}/{id}" // URL with parameters
                , new { controller = "HomeController", action = "Index", id = "" } // Parameter defaults
            );
        }
    }
}
