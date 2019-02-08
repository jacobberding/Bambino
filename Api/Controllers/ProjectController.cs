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
    public class ProjectController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] ProjectAddEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Project project = (data.projectId == Guid.Empty) ? new Project() : context.Projects
                        .Where(i => i.projectId == data.projectId
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
                    project.scale = data.scale;
                    project.isDeleted = data.isDeleted;

                    if (data.projectId == Guid.Empty)
                    {

                        Report report = context.Reports.FirstOrDefault();

                        report.projectCount = report.projectCount + 1;
                        
                        context.SubmitChanges();

                        project.companyId = a.member.activeCompanyId;
                        project.code = DateTime.Now.Year.ToString() + report.projectCount.ToString();

                        context.Projects.InsertOnSubmit(project);
                        context.SubmitChanges();

                        ProjectPhaseController.AddDefaults(project.projectId);
                        ProjectZoneController.AddDefaults(project.projectId);

                    }
                    else
                    { 
                        context.SubmitChanges();
                    }
                    
                    var activity = (data.projectId == Guid.Empty) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
                    LogController.Add(a.member.memberId, "Project " + project.name + " " + activity, "Project", "AddEditDelete", project.projectId, "Projects");

                    var vm = new AddEditDeleteReturnViewModel()
                    {
                        id = project.projectId,
                        state = (data.projectId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
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
        public HttpResponseMessage AddMember([FromBody] ProjectAddDeleteMemberViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Member member = context.Members
                        .Where(i => i.email == data.email
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (member == null)
                        throw new InvalidOperationException("Not Found");

                    Project project = context.Projects
                        .Where(i => i.projectId == data.projectId
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (project == null)
                        throw new InvalidOperationException("Not Found");

                    ProjectMember projectMember = new ProjectMember();

                    projectMember.projectId = project.projectId;
                    projectMember.memberId = member.memberId;

                    context.ProjectMembers.InsertOnSubmit(projectMember);
                    context.SubmitChanges();

                    LogController.Add(a.member.memberId, String.Format("Project {0} added {1} to the team", project.name, member.email), "Project", "AddMember", project.projectId, "Projects");

                    var vm = new MemberViewModel()
                    {
                        token = member.token,
                        email = member.email,
                        isActive = member.TimeTrackers.Any(i => i.isActive)
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
        public HttpResponseMessage DeleteMember([FromBody] ProjectAddDeleteMemberViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();
                    
                    Member member = context.Members.Where(i => i.token == data.token && !i.isDeleted).FirstOrDefault();
                    Project project = context.Projects.Where(i => i.projectId == data.projectId && !i.isDeleted).FirstOrDefault();

                    ProjectMember projectMember = context.ProjectMembers.Where(i => i.memberId == member.memberId && i.projectId == project.projectId).FirstOrDefault();

                    context.ProjectMembers.DeleteOnSubmit(projectMember);
                    context.SubmitChanges();
                    
                    LogController.Add(a.member.memberId, String.Format("Project {0} removed {1} from the team", project.name, member.email), "Project", "DeleteMember", project.projectId, "Projects");

                    var vm = new MemberViewModel()
                    {
                        token = member.token,
                        email = member.email,
                        isActive = member.TimeTrackers.Any(i => i.isActive)
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

                    var query = a.member.roles.Any(i => i.isAdmin) ? 
                        context.Projects
                            .Where(i => !i.isDeleted
                                && (i.name.Contains(data.search)
                                || i.code.Contains(data.search)))
                        : 
                        context.Projects
                            .Where(i => i.ProjectMembers.Any(x => x.memberId == a.member.memberId)
                                && !i.isDeleted
                                && (i.name.Contains(data.search)
                                || i.code.Contains(data.search)));

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = query.ToList().Count;
                    var arr = query
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
                            numOfMembers = obj.ProjectMembers.ToList().Count(),
                            isDeleted = obj.isDeleted
                        })
                        .OrderBy(i => i.name)
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

                    DateTimeOffset today = DateTimeOffset.UtcNow;

                    var vm = context.Projects
                        .Where(i => i.projectId == data.id
                            && !i.isDeleted)
                        .Select(obj => new ProjectViewModel
                        {
                            projectId = obj.projectId,
                            projectPhaseId = obj.ProjectPhases.Select(i => new { dateStart = i.dateStart, dateEnd = i.dateEnd, projectPhaseId = i.projectPhaseId }).Where(x => x.dateStart <= today && x.dateEnd >= today).DefaultIfEmpty(new { dateStart = DateTimeOffset.UtcNow, dateEnd = DateTimeOffset.UtcNow, projectPhaseId = Guid.Empty }).FirstOrDefault().projectPhaseId,
                            code = obj.code,
                            name = obj.name,
                            addressLine1 = obj.addressLine1,
                            addressLine2 = obj.addressLine2,
                            city = obj.city,
                            state = obj.state,
                            zip = obj.zip,
                            country = obj.country,
                            scale = obj.scale,
                            numOfHours = obj.TimeTrackerProjects.Select(i => i.totalHours).DefaultIfEmpty(0).ToList().Sum(x => x),
                            numOfMembers = obj.ProjectMembers.Count(),
                            projectPhases = obj.ProjectPhases.Select(projectPhase => new ProjectPhaseViewModel() {
                                projectPhaseId = projectPhase.projectPhaseId,
                                name = projectPhase.name
                            }).ToList(),
                            members = obj.ProjectMembers.Select(projectMember => new MemberViewModel() {
                                token = projectMember.Member.token,
                                firstName = projectMember.Member.firstName,
                                lastName = projectMember.Member.lastName,
                                email = projectMember.Member.email,
                                phone = projectMember.Member.phone,
                                isActive = projectMember.Member.TimeTrackers.Any(i => i.isActive)
                            })
                            .OrderBy(i => i.email)
                            .ToList(),
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

        [HttpPost]
        public HttpResponseMessage GetByCode([FromBody] ProjectGetByCodeViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _GetByCode(data.code));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public static ProjectViewModel _GetByCode(string code)
        {

            BambinoDataContext context = new BambinoDataContext();

            return context.Projects
                .Where(i => i.code == code
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
