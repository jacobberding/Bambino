using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MaterialPriceOptionController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage Get([FromBody] EmptyAuthenticationViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _Get());
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public static List<ListViewModel> _Get()
        {

            BambinoDataContext context = new BambinoDataContext();

            return context.MaterialPriceOptions
                .Where(i => !i.isDeleted)
                .Select(obj => new ListViewModel()
                {
                    value = obj.materialPriceOptionKey.ToString(),
                    name = obj.abbreviation
                })
                .OrderBy(i => i.name)
                .ToList();

        }

    }
}
