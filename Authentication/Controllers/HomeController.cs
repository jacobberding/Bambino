using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Authentication.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {

        public HomeController()
        {
            ViewBag.Version = System.Configuration.ConfigurationManager.AppSettings["version"];
        }

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("ResetPassword")]
        public ActionResult ResetPassword()
        {
            return View();
        }

    }
}