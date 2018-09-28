using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace CTITopicModelingAPI.Controllers
{
    public class TestController : ApiController
    {
        // GET: api/Test
        public HttpResponseMessage Get()
        {

            string responseText = "";
            try
            {


                // string baseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();

                string baseUrl = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Files\");

                string docsList = "[";


                foreach (string file in Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Files")))
                {
                    File.ReadAllText(file, System.Text.Encoding.Default);
                    docsList = docsList + "\"" + File.ReadAllText(file, System.Text.Encoding.Default) + "\"" + ",";
                }

                docsList = docsList + "]";

                return Request.CreateErrorResponse(HttpStatusCode.Accepted, docsList);

            }
            catch (Exception ex)
            {
                responseText = ex.Message;

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, responseText);
            }


        }

        // GET: api/Test/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Test
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Test/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Test/5
        public void Delete(int id)
        {
        }
    }
}
