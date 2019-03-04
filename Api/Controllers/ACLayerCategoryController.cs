using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class ACLayerCategoryController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage Get([FromBody] EmptyAuthenticationViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    var arr = context.ACLayerCategories
                        .Where(i => !i.isDeleted)
                        .Select(obj => new ListViewModel()
                        {
                            value = obj.acLayerCategoryId.ToString(),
                            name = obj.name
                        })
                        .OrderBy(i => i.name)
                        .ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, arr);

                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        [HttpPost]
        public HttpResponseMessage _Get([FromBody] EmptyAuthenticationViewModel data)
        {

            BambinoDataContext context = new BambinoDataContext();

            var arr = context.ACLayerCategories
                .Where(i => !i.isDeleted)
                .Select(obj => new ListViewModel()
                {
                    value = obj.acLayerCategoryId.ToString(),
                    name = obj.name
                })
                .OrderBy(i => i.name)
                .ToList();

            return Request.CreateResponse(HttpStatusCode.OK, arr);
            
        }

    }
}
