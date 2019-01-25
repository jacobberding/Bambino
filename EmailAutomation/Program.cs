using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EmailAutomation
{
    class Program
    {
        static void Main(string[] args)
        {

            string path = "C:\\logs\\_emailAutomation\\logExe_" + DateTime.UtcNow.ToLongDateString() + ".txt";

            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("Log Date\t\t\tError");
                    sw.WriteLine("-----------------------\t\t---------------------------------------------------------------------------");
                }
            }

            try
            {

                System.Configuration.AppSettingsReader rdr = new System.Configuration.AppSettingsReader();
                string authenticationId = rdr.GetValue("authenticationId", typeof(string)).ToString();
                string endpoint = rdr.GetValue("endpoint", typeof(string)).ToString();
                string vm = "{\"authenticationId\":\"" + authenticationId + "\"}";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = vm.Length;
                request.MaximumAutomaticRedirections = 1;
                request.AllowAutoRedirect = true;
                request.KeepAlive = false;
                using (Stream webStream = request.GetRequestStream())
                using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                {
                    requestWriter.Write(vm);
                }

                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream())
                {
                    using (StreamReader responseReader = new StreamReader(webStream))
                    {
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.WriteLine(DateTimeOffset.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture) + "\t\t" + responseReader.ReadToEnd());
                        }
                    }
                }

            }
            catch (WebException ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(DateTimeOffset.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture) + "\t\t" + ex.Message);
                }
            }

        }
    }
}
