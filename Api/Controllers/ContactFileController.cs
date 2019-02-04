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
    public class ContactFileController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddMany([FromBody] ContactFileAddManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    UnitOfWork unitOfWork = new UnitOfWork();
                    
                    foreach (var contactFileViewModel in data.contactFiles)
                    {

                        ContactFile contactFile = new ContactFile();

                        contactFile.contactFileId = contactFileViewModel.contactFileId;
                        contactFile.contactId = contactFileViewModel.contactId;
                        contactFile.path = contactFileViewModel.path;
                        contactFile.originalFileName = contactFileViewModel.originalFileName;
                        contactFile.isDeleted = contactFileViewModel.isDeleted;
                        
                        unitOfWork.ContactFileRepository.Insert(contactFile);
                        unitOfWork.Save();

                    }
                    
                    var vm = new { };

                    return Request.CreateResponse(HttpStatusCode.OK, vm);

                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

    }
}
