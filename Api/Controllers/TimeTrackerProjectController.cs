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
    public class TimeTrackerProjectController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage GetByPage([FromBody] SearchViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Expression<Func<TimeTrackerProject, bool>> query = i => i.projectId == data.id 
                            && !i.isDeleted
                            && (i.TimeTracker.Member.email.Contains(data.search)
                            || i.TimeTracker.Member.firstName.Contains(data.search)
                            || i.TimeTracker.Member.lastName.Contains(data.search));

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = context.TimeTrackerProjects.Where(query).Count();
                    decimal totalHours = context.TimeTrackerProjects.Where(query).Sum(i => i.totalHours);
                    var arr = context.TimeTrackerProjects
                        .Where(query)
                        .Select(obj => new TimeTrackerProjectViewModel
                        {
                            timeTrackerProjectId = obj.timeTrackerProjectId,
                            projectId = obj.projectId,
                            project = new ProjectViewModel()
                            {
                                name = obj.Project.name
                            },
                            description = obj.description,
                            totalHours = obj.totalHours,
                            timeTrackerId = obj.timeTrackerId,
                            timeTracker = new TimeTrackerViewModel()
                            {
                                dateCreated = obj.TimeTracker.dateCreated,
                                dateIn = obj.TimeTracker.dateIn,
                                dateOut = obj.TimeTracker.dateOut,
                                totalHours = obj.TimeTracker.totalHours,
                                isActive = obj.TimeTracker.isActive,
                                member = new MemberViewModel()
                                {
                                    token = obj.TimeTracker.Member.token,
                                    email = obj.TimeTracker.Member.email,
                                    firstName = obj.TimeTracker.Member.firstName,
                                    lastName = obj.TimeTracker.Member.lastName,
                                    isActive = obj.TimeTracker.Member.TimeTrackers.Any(i => i.isActive)
                                },
                            }
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
                        totalHours = totalHours,
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

    }
}
