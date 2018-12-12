using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class ProjectController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] ProjectAddEditDeleteViewModel data)
        {
            //Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            //if (a.isAuthenticated)
            //{

            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _AddEditDelete(data));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

            //}
            //return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public static AddEditDeleteReturnViewModel _AddEditDelete(ProjectAddEditDeleteViewModel data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();
            
            Project project = (data.projectId == Guid.Empty) ? new Project() : unitOfWork.ProjectRepository
                .GetBy(i => i.projectId == data.projectId
                    && !i.isDeleted)
                .FirstOrDefault();

            if (project == null)
                throw new InvalidOperationException("Not Found");
            
            project.name = data.name;
            project.addressLine1 = data.addressLine1;
            project.addressLine2 = data.addressLine2;
            project.city = data.city;
            project.state = data.state;
            project.zip = data.zip;
            project.country = data.country;
            project.isDeleted = data.isDeleted;

            if (data.projectId == Guid.Empty)
            {

                Report report = unitOfWork.ReportRepository.Get().FirstOrDefault();

                report.projectCount = report.projectCount + 1;

                unitOfWork.ReportRepository.Update(report);
                unitOfWork.Save();

                project.code = DateTime.Now.Year.ToString() + report.projectCount.ToString();

                unitOfWork.ProjectRepository.Insert(project);

            }
            else
                unitOfWork.ProjectRepository.Update(project);

            unitOfWork.Save();
            
            var activity = (data.projectId == Guid.Empty) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
            //LogController.Add(a.member.memberId, "Template Category " + category.name + " " + activity, "Category", "AddEditDelete", category.categoryId, "Categories");

            return new AddEditDeleteReturnViewModel()
            {
                id = project.projectId,
                state = (data.projectId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
            };

        }

        [HttpPost]
        public HttpResponseMessage GetByCode([FromBody] ProjectGetByCodeViewModel data)
        {
            //Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            //if (a.isAuthenticated)
            //{

            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _GetByCode(data.code));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

            //}
            //return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public static ProjectViewModel _GetByCode(string code)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            return unitOfWork.ProjectRepository
                .GetBy(i => i.code == code
                    && !i.isDeleted)
                .Select(obj => new ProjectViewModel
                {
                    projectId = obj.projectId,
                    code = obj.code,
                    name = obj.name,
                    addressLine1 = obj.addressLine1,
                    addressLine2 = obj.addressLine2,
                    city = obj.city,
                    state = obj.state,
                    zip = obj.zip,
                    country = obj.country,
                    isDeleted = obj.isDeleted
                })
                .FirstOrDefault();

        }

    }
}
