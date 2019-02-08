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

                    BambinoDataContext context = new BambinoDataContext();

                    ProjectZone projectZone = (data.projectZoneId == Guid.Empty) ? new ProjectZone() : context.ProjectZones
                        .Where(i => i.projectZoneId == data.projectZoneId
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (projectZone == null)
                        throw new InvalidOperationException("Layer Not Found");

                    projectZone.projectId = data.projectId;
                    projectZone.name = data.name;
                    projectZone.description = data.description;
                    projectZone.code = context.ProjectZones.Where(i => i.projectId == data.projectId && !i.isDeleted).Select(i => i.code).DefaultIfEmpty(-1).Max() + 1;
                    projectZone.isDeleted = data.isDeleted;

                    if (data.projectZoneId == Guid.Empty)
                        context.ProjectZones.InsertOnSubmit(projectZone);

                    context.SubmitChanges();

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

                BambinoDataContext context = new BambinoDataContext();

                ProjectZone projectZone = new ProjectZone();

                if (projectZone == null)
                    throw new InvalidOperationException("Layer Not Found");

                projectZone.projectId = projectId;
                projectZone.name = "Overall";
                projectZone.code = 0;
                
                context.ProjectZones.InsertOnSubmit(projectZone);
                context.SubmitChanges();
                
            }
            catch (Exception ex)
            {

            }
            
        }

        [HttpPost]
        public HttpResponseMessage EditArchive([FromBody] ProjectZoneEditArchiveViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    ProjectZone projectZone = (data.projectZoneId == Guid.Empty) ? new ProjectZone() : context.ProjectZones
                        .Where(i => i.projectZoneId == data.projectZoneId
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (projectZone == null)
                        throw new InvalidOperationException("Layer Not Found");

                    projectZone.isArchived = data.isArchived;

                    if (data.isArchived)
                    {

                        //Add Archive
                        ProjectZoneArchive projectZoneArchive = new ProjectZoneArchive();

                        projectZoneArchive.projectZoneId = data.projectZoneId;
                        projectZoneArchive.memberStartId = a.member.memberId;

                        context.ProjectZoneArchives.InsertOnSubmit(projectZoneArchive);

                    }
                    else
                    {

                        //Remove Archive
                        ProjectZoneArchive projectZoneArchive = context.ProjectZoneArchives
                            .Where(i => i.projectZoneId == data.projectZoneId
                                && i.dateEnd == DateTimeOffset.MinValue)
                            .FirstOrDefault();

                        projectZoneArchive.dateEnd = DateTimeOffset.UtcNow;
                        projectZoneArchive.memberEndId = a.member.memberId;
                        
                    }
                    
                    context.SubmitChanges();

                    var activity = (data.isArchived) ? "Archived" : "Unarchvied";
                    LogController.Add(a.member.memberId, String.Format("Zone {0} was {1}", projectZone.name, activity), "ProjectZone", "EditArchive", projectZone.projectZoneId, "ProjectZones");

                    var vm = new AddEditDeleteReturnViewModel()
                    {
                        id = projectZone.projectZoneId,
                        state = activity
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
        public HttpResponseMessage GetByPage([FromBody] SearchViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    var dateStart = context.ProjectPhases
                        .Where(i => i.projectPhaseId == data.projectPhaseId
                            && !i.isDeleted)
                        .Select(i => i.dateStart)
                        .FirstOrDefault();
                    
                    var query = data.id == Guid.Empty ? 
                        context.ProjectZones
                            .Where(i => i.projectId == data.id
                                && !i.isDeleted
                                && (i.name.Contains(data.search)
                                || i.code.ToString().Contains(data.search)))
                        :
                        context.ProjectZones
                            .Where(i => i.dateCreated >= dateStart
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
                            isArchived = obj.isArchived,
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

                    BambinoDataContext context = new BambinoDataContext();

                    var vm = context.ProjectZones
                        .Where(i => i.projectZoneId == data.id
                            && !i.isDeleted)
                        .Select(obj => new ProjectZoneViewModel
                        {
                            projectZoneId = obj.projectZoneId,
                            projectId = obj.projectId,
                            name = obj.name,
                            description = obj.description,
                            code = obj.code,
                            isArchived = obj.isArchived,
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
