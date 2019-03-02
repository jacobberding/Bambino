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
    public class ProjectReferenceController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] ProjectReferenceAddEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    ProjectReference projectReference = (data.projectReferenceKey == 0) ? new ProjectReference() : context.ProjectReferences
                        .Where(i => i.projectReferenceKey == data.projectReferenceKey
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (projectReference == null)
                        throw new InvalidOperationException("Layer Not Found");

                    projectReference.projectReferenceKey = data.projectReferenceKey;
                    projectReference.disciplineKey = data.disciplineKey;
                    projectReference.name = data.name;
                    projectReference.description = data.description;
                    projectReference.path = data.path;
                    projectReference.originalFileName = data.originalFileName;
                    projectReference.isDeleted = data.isDeleted;

                    if (data.projectReferenceKey == 0)
                    {

                        var isExists = context.ProjectReferences.Where(i => i.projectReferenceKey == data.projectReferenceKey && i.disciplineKey == data.disciplineKey && !i.isDeleted).Count() > 0 ? true : false;
                        projectReference.code = isExists ?
                            context.ProjectReferences.Where(i => i.projectReferenceKey == data.projectReferenceKey && i.disciplineKey == data.disciplineKey && !i.isDeleted).Select(i => i.code).Max() + 1
                            : 1;
                        projectReference.dateCreated = DateTimeOffset.UtcNow;
                        
                        context.ProjectReferences.InsertOnSubmit(projectReference);

                    }

                    context.SubmitChanges();

                    var activity = (data.projectReferenceKey == 0) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
                    LogController.Add(a.member.memberId, String.Format("ProjectReference {0} was {1}", projectReference.name, activity), "ProjectReference", "AddEditDelete", Guid.Empty, "ProjectReferences", projectReference.projectReferenceKey);

                    var vm = new AddEditDeleteReturnViewModel()
                    {
                        key = projectReference.projectReferenceKey,
                        state = (data.projectReferenceKey == 0) ? "add" : (data.isDeleted) ? "delete" : "edit"
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
        public HttpResponseMessage AddTag([FromBody] AddDeleteManyToManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    ProjectReferenceTagProjectReference projectReferenceTagProjectReference = new ProjectReferenceTagProjectReference();

                    projectReferenceTagProjectReference.projectReferenceKey = data.tableKey;
                    projectReferenceTagProjectReference.projectReferenceTagKey = context.ProjectReferenceTags.Where(i => i.name == data.name).FirstOrDefault().projectReferenceTagKey;

                    context.ProjectReferenceTagProjectReferences.InsertOnSubmit(projectReferenceTagProjectReference);

                    context.SubmitChanges();

                    var vm = new
                    {
                        projectReferenceTagProjectReference.projectReferenceTagKey,
                        data.name
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
        public HttpResponseMessage EditArchive([FromBody] EditArchiveViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    ProjectReference projectReference = context.ProjectReferences
                        .Where(i => i.projectReferenceKey == data.key
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (projectReference == null)
                        throw new InvalidOperationException("Not Found");

                    projectReference.isArchived = data.isArchived;

                    if (data.isArchived)
                    {

                        //Add Archive
                        ProjectReferenceArchive projectReferenceArchive = new ProjectReferenceArchive();

                        projectReferenceArchive.projectReferenceKey = data.key;
                        projectReferenceArchive.dateStart = DateTimeOffset.UtcNow;
                        projectReferenceArchive.memberStartId = a.member.memberId;
                        projectReferenceArchive.dateEnd = DateTimeOffset.MinValue;
                        projectReferenceArchive.memberEndId = Guid.Empty;

                        context.ProjectReferenceArchives.InsertOnSubmit(projectReferenceArchive);

                    }
                    else
                    {

                        //Remove Archive
                        ProjectReferenceArchive projectReferenceArchive = context.ProjectReferenceArchives
                            .Where(i => i.projectReferenceKey == data.key
                                && i.dateEnd == DateTimeOffset.MinValue)
                            .FirstOrDefault();

                        projectReferenceArchive.dateEnd = DateTimeOffset.UtcNow;
                        projectReferenceArchive.memberEndId = a.member.memberId;

                    }

                    context.SubmitChanges();

                    var activity = (data.isArchived) ? "Archived" : "Unarchvied";
                    LogController.Add(a.member.memberId, String.Format("Attraction {0} was {1}", projectReference.name, activity), "ProjectReference", "EditArchive", Guid.Empty, "ProjectReferences", projectReference.projectReferenceKey);

                    var vm = new
                    {
                        projectReference.projectReferenceKey,
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
        public HttpResponseMessage DeleteTag([FromBody] AddDeleteManyToManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    ProjectReferenceTagProjectReference projectReferenceTagProjectReference = context.ProjectReferenceTagProjectReferences
                        .Where(i => i.projectReferenceKey == data.tableKey
                            && i.projectReferenceTagKey == data.manyKey)
                        .FirstOrDefault();

                    context.ProjectReferenceTagProjectReferences.DeleteOnSubmit(projectReferenceTagProjectReference);

                    context.SubmitChanges();

                    var vm = new
                    {
                        projectReferenceTagProjectReference.projectReferenceKey,
                        projectReferenceTagProjectReference.projectReferenceTagKey
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

                    Expression<Func<ProjectReference, bool>> query = i => i.dateCreated <= dates.dateEnd
                        && i.projectAttractionKey == data.key
                            && (i.name.Contains(data.search)
                            || i.description.Contains(data.search)
                            || i.ProjectReferenceTagProjectReferences.Any(projectReferenceTagProjectReference => projectReferenceTagProjectReference.ProjectReferenceTag.name.Contains(data.search)))
                            && !i.isDeleted;

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = context.ProjectReferences.Where(query).ToList().Count;
                    var arr = context.ProjectReferences
                        .Where(query)
                        .Select(obj => new 
                        {
                            obj.projectReferenceKey,
                            obj.projectAttractionKey,
                            obj.disciplineKey,
                            discipline = new
                            {
                                obj.Discipline.disciplineKey,
                                obj.Discipline.description,
                                obj.Discipline.name,
                                obj.Discipline.value
                            },
                            obj.name,
                            obj.description,
                            code = obj.ProjectAttraction.ProjectZone.code + "-" + obj.ProjectAttraction.code + "-" + obj.Discipline.value + "-" + obj.code,
                            obj.path,
                            obj.originalFileName,
                            tags = obj.ProjectReferenceTagProjectReferences.Select(i => new {
                                i.projectReferenceTagKey,
                                i.ProjectReferenceTag.name
                            }).ToList(),
                            isArchived = isCurrent ? obj.isArchived : obj.ProjectReferenceArchives.Any(x => x.dateStart <= dates.dateEnd && x.dateEnd >= dates.dateEnd && x.dateEnd != DateTimeOffset.MinValue),
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

                    var vm = context.ProjectReferences
                        .Where(i => i.projectReferenceKey == data.key
                            && !i.isDeleted)
                        .Select(obj => new
                        {
                            obj.projectReferenceKey,
                            obj.projectAttractionKey,
                            obj.disciplineKey,
                            discipline = new
                            {
                                obj.Discipline.disciplineKey,
                                obj.Discipline.description,
                                obj.Discipline.name,
                                obj.Discipline.value
                            },
                            obj.name,
                            obj.description,
                            code = obj.ProjectAttraction.ProjectZone.code + "-" + obj.ProjectAttraction.code + "-" + obj.Discipline.value + "-" + obj.code,
                            obj.path,
                            obj.originalFileName,
                            tags = obj.ProjectReferenceTagProjectReferences.Select(i => new {
                                i.projectReferenceTagKey,
                                i.ProjectReferenceTag.name
                            }).ToList(),
                            obj.isArchived,
                            obj.isDeleted
                        })
                        .FirstOrDefault();

                    if (vm == null)
                        throw new InvalidOperationException("Not Found");

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
