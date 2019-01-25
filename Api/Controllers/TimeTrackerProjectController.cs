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

                    UnitOfWork unitOfWork = new UnitOfWork();

                    var query = unitOfWork.TimeTrackerProjectRepository
                        .GetBy(i => i.projectId == data.id 
                            && !i.isDeleted
                            && (i.timeTracker.member.email.Contains(data.search)
                            || i.timeTracker.member.firstName.Contains(data.search)
                            || i.timeTracker.member.lastName.Contains(data.search)));

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = query.ToList().Count;
                    decimal totalHours = query.ToList().Sum(i => i.totalHours);
                    var arr = query
                        .Select(obj => new TimeTrackerProjectViewModel
                        {
                            timeTrackerProjectId = obj.timeTrackerProjectId,
                            projectId = obj.projectId,
                            project = new ProjectViewModel()
                            {
                                name = obj.project.name
                            },
                            description = obj.description,
                            totalHours = obj.totalHours,
                            timeTrackerId = obj.timeTrackerId,
                            timeTracker = new TimeTrackerViewModel()
                            {
                                dateCreated = obj.timeTracker.dateCreated,
                                dateIn = obj.timeTracker.dateIn,
                                dateOut = obj.timeTracker.dateOut,
                                totalHours = obj.timeTracker.totalHours,
                                isActive = obj.timeTracker.isActive,
                                member = new MemberViewModel()
                                {
                                    token = obj.timeTracker.member.token,
                                    email = obj.timeTracker.member.email,
                                    firstName = obj.timeTracker.member.firstName,
                                    lastName = obj.timeTracker.member.lastName,
                                    isActive = obj.timeTracker.member.timeTrackers.Any(i => i.isActive)
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
