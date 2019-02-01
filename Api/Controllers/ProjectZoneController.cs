using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProjectZoneController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] ProjectZoneAddEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    UnitOfWork unitOfWork = new UnitOfWork();

                    ProjectZone projectZone = (data.projectZoneId == Guid.Empty) ? new ProjectZone() : unitOfWork.ProjectZoneRepository
                        .GetBy(i => i.projectZoneId == data.projectZoneId
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (projectZone == null)
                        throw new InvalidOperationException("Layer Not Found");

                    projectZone.projectId = data.projectId;
                    projectZone.name = data.name;
                    projectZone.description = data.description;
                    projectZone.code = unitOfWork.ProjectZoneRepository.GetBy(i => i.projectId == data.projectId && !i.isDeleted).Select(i => i.code).DefaultIfEmpty(-1).Max() + 1;
                    projectZone.isDeleted = data.isDeleted;

                    if (data.projectZoneId == Guid.Empty)
                        unitOfWork.ProjectZoneRepository.Insert(projectZone);
                    else
                        unitOfWork.ProjectZoneRepository.Update(projectZone);

                    unitOfWork.Save();

                    var activity = (data.projectZoneId == Guid.Empty) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
                    LogController.Add(a.member.memberId, String.Format("Zone {0} was {1}", projectZone.name, activity), "ProjectZone", "AddEditDelete", projectZone.projectZoneId, "ProjectZones");

                    var vm = new AddEditDeleteReturnViewModel()
                    {
                        id = projectZone.projectZoneId,
                        state = (data.projectZoneId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
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
        
        public static void AddDefaults(Guid projectId)
        {

            try
            {

                UnitOfWork unitOfWork = new UnitOfWork();

                ProjectZone projectZone = new ProjectZone();

                if (projectZone == null)
                    throw new InvalidOperationException("Layer Not Found");

                projectZone.projectId = projectId;
                projectZone.name = "Overall";
                projectZone.code = 0;
                
                unitOfWork.ProjectZoneRepository.Insert(projectZone);
                unitOfWork.Save();
                
            }
            catch (Exception ex)
            {

            }
            
        }

        [HttpPost]
        public HttpResponseMessage GetByPage([FromBody] SearchViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    UnitOfWork unitOfWork = new UnitOfWork();

                    var dateStart = unitOfWork.ProjectPhaseRepository
                        .GetBy(i => i.projectPhaseId == data.projectPhaseId
                            && !i.isDeleted)
                        .Select(i => i.dateStart)
                        .FirstOrDefault();
                    
                    var query = data.id == Guid.Empty ? 
                        unitOfWork.ProjectZoneRepository
                            .GetBy(i => i.projectId == data.id
                                && !i.isDeleted
                                && (i.name.Contains(data.search)
                                || i.code.ToString().Contains(data.search)))
                        :
                        unitOfWork.ProjectZoneRepository
                            .GetBy(i => i.dateCreated >= dateStart
                                && i.projectId == data.id
                                && !i.isDeleted
                                && (i.name.Contains(data.search)
                                || i.code.ToString().Contains(data.search)));

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = query.ToList().Count;
                    var arr = query
                        .Select(obj => new ProjectZoneViewModel
                        {
                            projectZoneId = obj.projectZoneId,
                            projectId = obj.projectId,
                            name = obj.name,
                            description = obj.description,
                            code = obj.code,
                            isDeleted = obj.isDeleted
                        })
                        .OrderBy(data.sort)
                        .Skip(skip)
                        .Take(data.records)
                        .ToList();

                    if (arr == null)
                        throw new InvalidOperationException("Not Found");

                    var vm = new
                    {
                        totalRecords = totalRecords,
                        totalPages = Math.Ceiling((double)totalRecords / data.records),
                        arr = arr.ToList()
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
        public HttpResponseMessage GetById([FromBody] GetByIdViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    UnitOfWork unitOfWork = new UnitOfWork();

                    var vm = unitOfWork.ProjectZoneRepository
                        .GetBy(i => i.projectZoneId == data.id
                            && !i.isDeleted)
                        .Select(obj => new ProjectZoneViewModel
                        {
                            projectZoneId = obj.projectZoneId,
                            projectId = obj.projectId,
                            name = obj.name,
                            description = obj.description,
                            code = obj.code,
                            isDeleted = obj.isDeleted
                        })
                        .FirstOrDefault();

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
