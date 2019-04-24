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
    public class ProjectContactCompanyController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage Add([FromBody] AddDeleteManyToManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Project project = context.Projects.Where(i => i.projectId == data.tableId).FirstOrDefault();
                    ContactCompany contactCompany = context.ContactCompanies.Where(i => i.contactCompanyKey == data.manyKey).FirstOrDefault();

                    ProjectContactCompany projectContactCompany = new ProjectContactCompany();

                    projectContactCompany.projectId = project.projectId;
                    projectContactCompany.contactCompanyKey = contactCompany.contactCompanyKey;
                    projectContactCompany.isClient = contactCompany.isClient;
                    projectContactCompany.isVendorDesign = contactCompany.isVendorDesign;
                    projectContactCompany.isVendorIntegration = contactCompany.isVendorIntegration;

                    context.ProjectContactCompanies.InsertOnSubmit(projectContactCompany);
                    context.SubmitChanges();

                    LogController.Add(a.member.memberId, "Project " + project.name + " Added Company " + contactCompany.name, "ProjectContactCompany", "Add", Guid.Empty, "ProjectContactCompanies", projectContactCompany.projectContactCompanyKey);

                    var vm = new
                    {
                        contactCompany.contactCompanyKey,
                        contactCompany.name,
                        value = contactCompany.contactCompanyKey
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

        [HttpPost]
        public HttpResponseMessage Delete([FromBody] AddDeleteManyToManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Project project = context.Projects.Where(i => i.projectId == data.tableId).FirstOrDefault();
                    ContactCompany contactCompany = context.ContactCompanies.Where(i => i.contactCompanyKey == data.manyKey).FirstOrDefault();

                    ProjectContactCompany projectContactCompany = context.ProjectContactCompanies.Where(i => i.projectId == data.tableId && i.contactCompanyKey == data.manyKey).FirstOrDefault();

                    context.ProjectContactCompanies.DeleteOnSubmit(projectContactCompany);
                    context.SubmitChanges();

                    LogController.Add(a.member.memberId, "Project " + project.name + " Removed Company " + contactCompany.name, "ProjectContactCompany", "Delete", Guid.Empty, "ProjectContactCompanies", projectContactCompany.projectContactCompanyKey);

                    var vm = new
                    {
                        contactCompany.contactCompanyKey,
                        contactCompany.name,
                        value = contactCompany.contactCompanyKey
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
