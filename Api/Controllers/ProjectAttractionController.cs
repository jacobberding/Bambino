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
    public class ProjectAttractionController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] ProjectAttractionAddEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    ProjectAttraction projectAttraction = (data.projectAttractionKey == 0) ? new ProjectAttraction() : context.ProjectAttractions
                        .Where(i => i.projectAttractionKey == data.projectAttractionKey
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (projectAttraction == null)
                        throw new InvalidOperationException("Not Found");

                    projectAttraction.projectZoneKey = data.projectZoneKey;
                    projectAttraction.name = data.name;
                    projectAttraction.description = data.description;
                    projectAttraction.isArchived = false;
                    projectAttraction.isDeleted = data.isDeleted;

                    if (data.projectAttractionKey == 0)
                    {

                        var isExists = context.ProjectAttractions.Where(i => i.projectZoneKey == data.projectZoneKey && !i.isDeleted).Count() > 0 ? true : false;
                        projectAttraction.code = isExists ? 
                            context.ProjectAttractions.Where(i => i.projectZoneKey == data.projectZoneKey && !i.isDeleted).Select(i => i.code).Max() + 1
                            : 0;
                        projectAttraction.dateCreated = DateTimeOffset.UtcNow;

                        context.ProjectAttractions.InsertOnSubmit(projectAttraction);

                    }

                    context.SubmitChanges();

                    var activity = (data.projectAttractionKey == 0) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
                    LogController.Add(a.member.memberId, String.Format("Attraction {0} was {1}", projectAttraction.name, activity), "ProjectAttraction", "AddEditDelete", Guid.Empty, "ProjectAttractions", projectAttraction.projectAttractionKey);

                    var vm = new 
                    {
                        projectAttraction.projectAttractionKey,
                        state = (data.projectAttractionKey == 0) ? "add" : (data.isDeleted) ? "delete" : "edit"
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

        public static void AddDefaults(long projectZoneKey)
        {

            try
            {

                BambinoDataContext context = new BambinoDataContext();

                ProjectAttraction projectAttraction = new ProjectAttraction();

                projectAttraction.projectZoneKey = projectZoneKey;
                projectAttraction.name = "Overall";
                projectAttraction.description = "";
                projectAttraction.code = 0;
                projectAttraction.dateCreated = DateTimeOffset.UtcNow;
                projectAttraction.isArchived = false;
                projectAttraction.isDeleted = false;

                context.ProjectAttractions.InsertOnSubmit(projectAttraction);
                context.SubmitChanges();

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

                    ProjectAttraction projectAttraction = context.ProjectAttractions
                        .Where(i => i.projectAttractionKey == data.key
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (projectAttraction == null)
                        throw new InvalidOperationException("Not Found");

                    projectAttraction.isArchived = data.isArchived;

                    if (data.isArchived)
                    {

                        //Add Archive
                        ProjectAttractionArchive projectAttractionArchive = new ProjectAttractionArchive();

                        projectAttractionArchive.projectAttractionKey = data.key;
                        projectAttractionArchive.dateStart = DateTimeOffset.UtcNow;
                        projectAttractionArchive.memberStartId = a.member.memberId;
                        projectAttractionArchive.dateEnd = DateTimeOffset.MinValue;
                        projectAttractionArchive.memberEndId = Guid.Empty;

                        context.ProjectAttractionArchives.InsertOnSubmit(projectAttractionArchive);

                    }
                    else
                    {

                        //Remove Archive
                        ProjectAttractionArchive projectAttractionArchive = context.ProjectAttractionArchives
                            .Where(i => i.projectAttractionKey == data.key
                                && i.dateEnd == DateTimeOffset.MinValue)
                            .FirstOrDefault();

                        projectAttractionArchive.dateEnd = DateTimeOffset.UtcNow;
                        projectAttractionArchive.memberEndId = a.member.memberId;

                    }

                    context.SubmitChanges();

                    var activity = (data.isArchived) ? "Archived" : "Unarchvied";
                    LogController.Add(a.member.memberId, String.Format("Attraction {0} was {1}", projectAttraction.name, activity), "ProjectAttraction", "EditArchive", Guid.Empty, "ProjectAttractions", projectAttraction.projectAttractionKey);

                    var vm = new 
                    {
                        projectAttraction.projectAttractionKey,
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

                    var dates = context.ProjectPhases
                        .Where(i => i.projectPhaseKey == data.projectPhaseKey
                            && !i.isDeleted)
                        .Select(i => new { i.dateStart, i.dateEnd })
                        .FirstOrDefault();
                    var isCurrent = DateTimeOffset.UtcNow >= dates.dateStart && DateTimeOffset.UtcNow <= dates.dateEnd ? true : false;

                    Expression<Func<ProjectAttraction, bool>> query = i => i.dateCreated <= dates.dateEnd
                        && i.ProjectZone.projectId == data.id
                        && !i.isDeleted
                        && (i.name.Contains(data.search)
                        || i.code.ToString().Contains(data.search));
                    
                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = context.ProjectAttractions.Where(query).Count();
                    var arr = context.ProjectAttractions
                        .Where(query)
                        .Select(obj => new
                        {
                            obj.projectAttractionKey,
                            obj.projectZoneKey,
                            projectZone = new 
                            {
                                obj.ProjectZone.name,
                                obj.ProjectZone.code
                            },
                            obj.name,
                            obj.description,
                            code = obj.ProjectZone.code + "-" + obj.code,
                            isArchived = isCurrent ? obj.isArchived : obj.ProjectAttractionArchives.Any(x => x.dateStart <= dates.dateEnd && x.dateEnd >= dates.dateEnd && x.dateEnd != DateTimeOffset.MinValue),
                            obj.isDeleted
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

                    var vm = context.ProjectAttractions
                        .Where(i => i.projectAttractionKey == data.key
                            && !i.isDeleted)
                        .Select(obj => new 
                        {
                            obj.projectAttractionKey,
                            obj.projectZoneKey,
                            obj.name,
                            obj.description,
                            code = obj.ProjectZone.code + "-" + obj.code,
                            obj.isArchived,
                            obj.isDeleted,
                            numOfElements = obj.ProjectElements.Count(),
                            numOfReferences = obj.ProjectReferences.Count()
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
