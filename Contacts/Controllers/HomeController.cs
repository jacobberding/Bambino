﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contacts.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
        
    }
}