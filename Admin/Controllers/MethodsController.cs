using Newtonsoft.Json;
using Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Store.Controllers
{
    [RoutePrefix("Methods")]
    public class MethodsController : Controller
    {
        
        public static string Purpose = "Authentication Token";

        [Route("SetJackSparrow")]
        [HttpPost]
        public JsonResult SetJackSparrow(string value)
        {
            var unprotectedBytes = Encoding.UTF8.GetBytes(value);
            var protectedBytes = MachineKey.Protect(unprotectedBytes, Purpose);
            var protectedText = Convert.ToBase64String(protectedBytes);
            return Json(protectedText, JsonRequestBehavior.AllowGet);
        }

        [Route("GetJackSparrow")]
        [HttpPost]
        public JsonResult GetJackSparrow()
        {

            var protectedBytes = Convert.FromBase64String(System.Web.HttpContext.Current.Request.Cookies["_e"].Value.ToString());
            var unprotectedBytes = MachineKey.Unprotect(protectedBytes, Purpose);
            var unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);

            var _e = JsonConvert.DeserializeObject<e>(unprotectedText);

            return Json(_e, JsonRequestBehavior.AllowGet);

        }

        public string HTTPRequest(string vm, string url)
        {

            try
            {

                string e = (System.Web.HttpContext.Current.Request.Cookies["_e"] == null) ? "" : System.Web.HttpContext.Current.Request.Cookies["_e"].Value.ToString();
                e _e = (e == "") ? new e() : JsonConvert.DeserializeObject<e>(Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(e), MethodsController.Purpose)));

                vm = vm.Insert(1, "\"authentication\":{\"apiId\":\"FF9F2822-5F5D-4753-BB8D-2DECCF04A91A\",\"token\":\"" + _e.mT.ToString() + "\"}" + ((vm == "{}") ? "" : ","));

                var request = WebRequest.Create(url) as HttpWebRequest;
                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json";

                byte[] byteArray = Encoding.UTF8.GetBytes(vm);

                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        return reader.ReadToEnd();
                    }
                }

            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    if (ex.Response.ContentLength != 0)
                    {
                        using (var stream = ex.Response.GetResponseStream())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                string r = reader.ReadToEnd();
                                throw new InvalidOperationException(r);
                            }
                        }
                    }
                }
                return ex.Message;
            }

        }

        [Route("Get")]
        [HttpPost]
        public void Get(string vm, string m)
        {
            try
            {

                string[] mm = m.Split('_');

                Response.StatusCode = 200;
                Response.Write(HTTPRequest(vm, "https://api.bambino.software/Api/" + mm[0] + "/" + mm[1]));

            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                Response.Write(ex.Message);
            }
        }

    }
}