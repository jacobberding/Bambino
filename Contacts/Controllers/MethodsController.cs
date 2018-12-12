using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Contacts.Controllers
{
    [RoutePrefix("Methods")]
    public class MethodsController : Controller
    {
        
        public static string Purpose = "Authentication Token";
        
        public string HTTPRequest(string vm, string url)
        {

            try
            {
                
                //vm = vm.Insert(1, "\"authentication\":{\"apiId\":\"5aa5c39e-efd3-4e69-8fa2-c7c587ea69f6\",\"token\":\"" + _c.mT.ToString() + "\"}" + ((vm == "{}") ? "" : ","));

                var request = WebRequest.Create(url) as HttpWebRequest;
		        request.KeepAlive = true;
		        request.Method = "POST";
		        request.ContentType = "application/json";
                
		        byte[] byteArray = Encoding.UTF8.GetBytes(vm);
                
			    using (var writer = request.GetRequestStream()) {
				    writer.Write(byteArray, 0, byteArray.Length);
			    }

			    using (var response = request.GetResponse() as HttpWebResponse) {
				    using (var reader = new StreamReader(response.GetResponseStream())) {
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
            try {
                
                string[] mm = m.Split('_');
                
                Response.StatusCode = 200;
                Response.Write(HTTPRequest(vm, "http://api.entdesign.com/Api/" + mm[0] + "/" + mm[1]));

            }
            catch (Exception ex) {
                Response.StatusCode = 500;
                Response.Write(ex.Message);
            }
        }
        
    }
}