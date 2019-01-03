using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ApiTokenController : ApiController
    {
        
        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] ApiTokenAddEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    
                    UnitOfWork unitOfWork = new UnitOfWork();

                    ApiToken apiToken = (data.apiTokenId == Guid.Empty) ? new ApiToken() : unitOfWork.ApiTokenRepository
                        .GetBy(i => i.apiTokenId == data.apiTokenId
                            && !i.isDeleted)
                        .FirstOrDefault();
                    
                    if (apiToken == null)
                        throw new InvalidOperationException("Not Found");
                    
                    apiToken.companyName = data.companyName;
                    apiToken.adminName = data.adminName;
                    apiToken.adminEmail = data.adminEmail;
                    apiToken.adminPhone = data.adminPhone;
                    apiToken.isDeleted = data.isDeleted;

                    if (data.apiTokenId == Guid.Empty)
                    {

                        unitOfWork.ApiTokenRepository.Insert(apiToken);

                        new Thread(() =>
                        {
                            EmailController.Send(new MailAddress(EmailController.email),
                                "jpberding@gmail.com",
                                EmailController.email,
                                EmailController.email,
                                "Api Token Request",
                                "Company Name: " + apiToken.companyName + " Name: " + apiToken.adminName + " Email: " + apiToken.adminEmail + " Phone: " + apiToken.adminPhone);
                        }).Start();

                    } else {
                        unitOfWork.ApiTokenRepository.Update(apiToken);
                    }

                    unitOfWork.Save();
                    
                    var vm = new {
                        apiTokenId = apiToken.apiTokenId,
                        state = (data.apiTokenId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
                    };
                    
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
