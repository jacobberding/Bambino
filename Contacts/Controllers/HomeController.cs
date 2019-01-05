using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Contacts.Models;
using Newtonsoft.Json;

namespace Contacts.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {

        public HomeController()
        {

            string domain = "https://contacts.bambino.software";

            try
            {

                string e = (System.Web.HttpContext.Current.Request.Cookies["_e"] == null) ? "" : System.Web.HttpContext.Current.Request.Cookies["_e"].Value.ToString();
                e _e = (e == "") ? new e() : JsonConvert.DeserializeObject<e>(Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(e), MethodsController.Purpose)));

                ViewBag.Version = System.Configuration.ConfigurationManager.AppSettings["version"];

                string url = "https://api.bambino.software/Api/Member/GetByAuthTokenValidation";
                string data = @"{'apiId':'FF9F2822-5F5D-4753-BB8D-2DECCF04A91A','token':'" + _e.mT.ToString() + "','email':'" + _e.mE + "','isAdmin':'false'}";

                var request = WebRequest.Create(url) as HttpWebRequest;
                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json";

                byte[] byteArray = Encoding.UTF8.GetBytes(data);

                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string r = reader.ReadToEnd();
                        if (r == "false")
                            System.Web.HttpContext.Current.Response.Redirect("https://authentication.bambino.software/?returnUrl=" + domain);
                    }
                }

            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Redirect("https://authentication.bambino.software/?returnUrl=" + domain);
            }

        }

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
        
    }
}