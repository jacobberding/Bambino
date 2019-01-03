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
                    return Request.CreateResponse(HttpStatusCode.OK, _Get());
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public static List<ACLayerCategoryViewModel> _Get()
        {

            UnitOfWork unitOfWork = new UnitOfWork();
            
            return unitOfWork.ACLayerCategoryRepository
                .GetBy(i => !i.isDeleted)
                .Select(obj => new ACLayerCategoryViewModel()
                {
                    acLayerCategoryId = obj.acLayerCategoryId,
                    name = obj.name,
                    description = obj.description,
                    value = obj.value,
                    isDeleted = obj.isDeleted
                })
                .OrderBy(i => i.name)
                .ToList();

        }

    }
}
