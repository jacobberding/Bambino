using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
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

                    ProjectZone projectZone = (data.projectZoneKey == 0) ? new ProjectZone() : context.ProjectZones
                        .Where(i => i.projectZoneKey == data.projectZoneKey
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (projectZone == null)
                        throw new InvalidOperationException("Not Found");

                    projectZone.projectId = data.projectId;
                    projectZone.name = data.name;
                    projectZone.description = data.description;
                    projectZone.code = (data.projectZoneKey == 0) ? 
                        context.ProjectZones.Where(i => i.projectId == data.projectId && !i.isDeleted).Select(i => i.code).Max() + 1
                        : projectZone.code;
                    projectZone.isArchived = false;
                    projectZone.isDeleted = data.isDeleted;

                    if (data.projectZoneKey == 0)
                    {

                        projectZone.dateCreated = DateTimeOffset.UtcNow;

                        context.ProjectZones.InsertOnSubmit(projectZone);

                    }

                    context.SubmitChanges();

                    if (data.projectZoneKey == 0)
                        ProjectAttractionController.AddDefaults(projectZone.projectZoneKey);

                    var activity = (data.projectZoneKey == 0) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
                    LogController.Add(a.member.memberId, String.Format("Zone {0} was {1}", projectZone.name, activity), "ProjectZone", "AddEditDelete", Guid.Empty, "ProjectZones", projectZone.projectZoneKey);

                    var vm = new
                    {
                        projectZone.projectZoneKey,
                        state = (data.projectZoneKey == 0) ? "add" : (data.isDeleted) ? "delete" : "edit"
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
                
                projectZone.projectId = projectId;
                projectZone.name = "Overall";
                projectZone.description = "";
                projectZone.code = 0;
                projectZone.dateCreated = DateTimeOffset.UtcNow;
                projectZone.isArchived = false;
                projectZone.isDeleted = false;

                context.ProjectZones.InsertOnSubmit(projectZone);
                context.SubmitChanges();

                ProjectAttractionController.AddDefaults(projectZone.projectZoneKey);

            }
            catch (Exception ex)
            {

            }
            
        }

        [HttpPost]
        public HttpResponseMessage EditArchive([FromBody] EditArchiveViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    ProjectZone projectZone = context.ProjectZones
                        .Where(i => i.projectZoneKey == data.key
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (projectZone == null)
                        throw new InvalidOperationException("Not Found");

                    projectZone.isArchived = data.isArchived;

                    if (data.isArchived)
                    {

                        //Add Archive
                        ProjectZoneArchive projectZoneArchive = new ProjectZoneArchive();

                        projectZoneArchive.projectZoneKey = data.key;
                        projectZoneArchive.dateStart = DateTimeOffset.UtcNow;
                        projectZoneArchive.memberStartId = a.member.memberId;
                        projectZoneArchive.dateEnd = DateTimeOffset.MinValue;
                        projectZoneArchive.memberEndId = Guid.Empty;

                        context.ProjectZoneArchives.InsertOnSubmit(projectZoneArchive);

                    }
                    else
                    {

                        //Remove Archive
                        ProjectZoneArchive projectZoneArchive = context.ProjectZoneArchives
                            .Where(i => i.projectZoneKey == data.key
                                && i.dateEnd == DateTimeOffset.MinValue)
                            .FirstOrDefault();

                        projectZoneArchive.dateEnd = DateTimeOffset.UtcNow;
                        projectZoneArchive.memberEndId = a.member.memberId;
                        
                    }
                    
                    context.SubmitChanges();

                    var activity = (data.isArchived) ? "Archived" : "Unarchvied";
                    LogController.Add(a.member.memberId, String.Format("Zone {0} was {1}", projectZone.name, activity), "ProjectZone", "EditArchive", Guid.Empty, "ProjectZones", projectZone.projectZoneKey);

                    var vm = new
                    {
                        projectZone.projectZoneKey,
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
        public HttpResponseMessage Get([FromBody] GetByIdViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();
                    
                    var arr = context.ProjectZones
                        .Where(i => i.projectId == data.id
                            && !i.isDeleted)
                        .Select(obj => new ListViewModel
                        {
                            value = obj.projectZoneKey.ToString(),
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
        public HttpResponseMessage GetByPage([FromBody] SearchViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    var dates = context.ProjectPhases
                        .Where(i => i.projectPhaseKey == data.projectPhaseKey
                            && !i.isDeleted)
                        .Select(i => new { i.dateStart, i.dateEnd })
                        .FirstOrDefault();
                    var isCurrent = DateTimeOffset.UtcNow >= dates.dateStart && DateTimeOffset.UtcNow <= dates.dateEnd ? true : false; 

                    Expression<Func<ProjectZone, bool>> query = i => i.dateCreated <= dates.dateEnd
                                && i.projectId == data.id
                                && !i.isDeleted
                                && (i.name.Contains(data.search)
                                || i.code.ToString().Contains(data.search));

                    if (data.id == Guid.Empty)
                        query = i => i.projectId == data.id
                            && !i.isDeleted
                            && (i.name.Contains(data.search)
                            || i.code.ToString().Contains(data.search));

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = context.ProjectZones.Where(query).Count();
                    var arr = context.ProjectZones
                        .Where(query)
                        .Select(obj => new ProjectZoneViewModel
                        {
                            projectZoneKey = obj.projectZoneKey,
                            projectId = obj.projectId,
                            name = obj.name,
                            description = obj.description,
                            code = obj.code,
                            isArchived = isCurrent ? obj.isArchived : obj.ProjectZoneArchives.Any(x => x.dateStart <= dates.dateEnd && x.dateEnd >= dates.dateEnd && x.dateEnd != DateTimeOffset.MinValue),
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
                        .Where(i => i.projectZoneKey == data.key
                            && !i.isDeleted)
                        .Select(obj => new ProjectZoneViewModel
                        {
                            projectZoneKey = obj.projectZoneKey,
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
