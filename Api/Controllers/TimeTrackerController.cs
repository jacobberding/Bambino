using Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TimeTrackerController : ApiController
    {
        
        [HttpPost]
        public HttpResponseMessage EditDelete([FromBody] TimeTrackerAddEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    TimeTracker timeTracker = context.TimeTrackers
                        .Where(i => i.timeTrackerId == data.timeTrackerId
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (timeTracker == null)
                        throw new InvalidOperationException("Not Found");

                    timeTracker.dateIn = data.dateIn;
                    timeTracker.dateOut = data.dateOut;
                    timeTracker.totalHours = data.totalHours;
                    timeTracker.isDeleted = data.isDeleted;
                    
                    context.TimeTrackerProjects.DeleteAllOnSubmit<TimeTrackerProject>(timeTracker.TimeTrackerProjects);
                    context.SubmitChanges();

                    List<TimeTrackerProject> timeTrackerProjects = new List<TimeTrackerProject>();
                    foreach (var project in data.projects)
                    {

                        TimeTrackerProject timeTrackerProject = new TimeTrackerProject();

                        timeTrackerProject.timeTrackerId = timeTracker.timeTrackerId;
                        timeTrackerProject.projectId = (project.projectId == Guid.Empty) ? context.Projects.Where(i => i.isDefault && i.companyId == a.member.activeCompanyId).FirstOrDefault().projectId : project.projectId;
                        timeTrackerProject.description = "";
                        timeTrackerProject.totalHours = project.totalHours;

                        timeTrackerProjects.Add(timeTrackerProject);

                    }

                    context.TimeTrackerProjects.InsertAllOnSubmit(timeTrackerProjects);
                    context.SubmitChanges();

                    var activity = (data.isDeleted) ? "Deleted" : "Edited";
                    LogController.Add(a.member.memberId, String.Format("{0} {1} {2} a time sheet record", timeTracker.Member.firstName, timeTracker.Member.lastName, activity), "TimeTracker", "EditDelete", timeTracker.timeTrackerId, "TimeTrackers");

                    var vm = new AddEditDeleteReturnViewModel()
                    {
                        id = timeTracker.timeTrackerId,
                        state = (data.isDeleted) ? "delete" : "edit"
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
        public HttpResponseMessage In([FromBody] EmptyAuthenticationViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    bool isActive = context.TimeTrackers
                        .Where(i => i.memberId == a.member.memberId
                            && i.isActive
                            && !i.isDeleted)
                        .Count() > 0 ? true : false;
                        
                    if (isActive)
                        throw new InvalidOperationException("Already Clocked In");

                    TimeTracker timeTracker = new TimeTracker();

                    timeTracker.memberId = a.member.memberId;
                    timeTracker.dateCreated = DateTimeOffset.UtcNow;
                    timeTracker.dateIn = DateTimeOffset.UtcNow;
                    timeTracker.dateOut = DateTimeOffset.MinValue;
                    timeTracker.isActive = true;

                    context.TimeTrackers.InsertOnSubmit(timeTracker);
                    context.SubmitChanges();

                    LogController.Add(a.member.memberId, String.Format("{0} clocked in", a.member.email), "TimeTracker", "In", timeTracker.timeTrackerId, "TimeTrackers");

                    var vm = new AddEditDeleteReturnViewModel()
                    {
                        id = timeTracker.timeTrackerId,
                        state = "in"
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
        public HttpResponseMessage Out([FromBody] TimeTrackerOutViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    bool isActive = context.TimeTrackers
                        .Where(i => i.memberId == a.member.memberId
                            && i.isActive
                            && !i.isDeleted)
                        .Count() > 0 ? true : false;

                    if (!isActive)
                        throw new InvalidOperationException("Not Clocked In Yet");

                    List<TimeTrackerProject> timeTrackerProjects = new List<TimeTrackerProject>();
                    TimeTracker timeTracker = context.TimeTrackers
                        .Where(i => i.memberId == a.member.memberId
                            && i.isActive
                            && !i.isDeleted)
                        .FirstOrDefault();

                    timeTracker.totalHours = data.totalHours;
                    timeTracker.dateOut = DateTimeOffset.UtcNow;
                    timeTracker.isActive = false;
                    
                    foreach (var project in data.projects)
                    {

                        TimeTrackerProject timeTrackerProject = new TimeTrackerProject();

                        timeTrackerProject.timeTrackerId = timeTracker.timeTrackerId;
                        timeTrackerProject.projectId = (project.projectId == Guid.Empty) ? context.Projects.Where(i => i.isDefault && i.companyId == a.member.activeCompanyId).FirstOrDefault().projectId : project.projectId;
                        timeTrackerProject.description = "";
                        timeTrackerProject.totalHours = project.totalHours;
                        //add description eventually
                        timeTrackerProjects.Add(timeTrackerProject);

                    }

                    context.TimeTrackerProjects.InsertAllOnSubmit(timeTrackerProjects);
                    context.SubmitChanges();

                    LogController.Add(a.member.memberId, String.Format("{0} clocked out", a.member.email), "TimeTracker", "Out", timeTracker.timeTrackerId, "TimeTrackers");

                    var vm = new AddEditDeleteReturnViewModel()
                    {
                        id = timeTracker.timeTrackerId,
                        state = "out"
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

                    Expression<Func<TimeTracker, bool>> query = i => i.Member.token == data.id
                            && !i.isDeleted
                            && (i.Member.email.Contains(data.search)
                            || i.Member.firstName.Contains(data.search)
                            || i.Member.lastName.Contains(data.search));

                    if (data.id == Guid.Empty)
                        query = i => !i.isDeleted
                            && (i.Member.email.Contains(data.search)
                            || i.Member.firstName.Contains(data.search)
                            || i.Member.lastName.Contains(data.search));

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = context.TimeTrackers.Where(query).Count();
                    decimal totalHours = context.TimeTrackers.Where(query).Sum(i => i.totalHours);
                    var arr = context.TimeTrackers
                        .Where(query)
                        .Select(obj => new TimeTrackerViewModel
                        {
                            timeTrackerId = obj.timeTrackerId,
                            member = new MemberViewModel()
                            {
                                token = obj.Member.token,
                                email = obj.Member.email,
                                firstName = obj.Member.firstName,
                                lastName = obj.Member.lastName,
                                isActive = obj.Member.TimeTrackers.Any(i => i.isActive)
                            },
                            dateCreated = obj.dateCreated,
                            dateIn = obj.dateIn,
                            dateOut = obj.dateOut,
                            totalHours = obj.totalHours,
                            isActive = obj.isActive,
                            timeTrackerProjects = obj.TimeTrackerProjects.Select(timeTrackerProject => new TimeTrackerProjectViewModel()
                            {
                                timeTrackerProjectId = timeTrackerProject.timeTrackerProjectId,
                                projectId = timeTrackerProject.projectId,
                                project = new ProjectViewModel() {
                                    name = timeTrackerProject.Project.name
                                },
                                description = timeTrackerProject.description,
                                totalHours = timeTrackerProject.totalHours,
                            }).ToList()
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

        [HttpPost]
        public HttpResponseMessage GetById([FromBody] GetByIdViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    var vm = context.TimeTrackers
                        .Where(i => i.timeTrackerId == data.id
                            && !i.isDeleted)
                        .Select(obj => new TimeTrackerViewModel
                        {
                            timeTrackerId = obj.timeTrackerId,
                            member = new MemberViewModel()
                            {
                                token = obj.Member.token,
                                email = obj.Member.email,
                                firstName = obj.Member.firstName,
                                lastName = obj.Member.lastName,
                                isActive = obj.Member.TimeTrackers.Any(i => i.isActive)
                            },
                            dateCreated = obj.dateCreated,
                            dateIn = obj.dateIn,
                            dateOut = obj.dateOut,
                            totalHours = obj.totalHours,
                            isActive = obj.isActive,
                            timeTrackerProjects = obj.TimeTrackerProjects.Select(timeTrackerProject => new TimeTrackerProjectViewModel()
                            {
                                timeTrackerProjectId = timeTrackerProject.timeTrackerProjectId,
                                projectId = timeTrackerProject.projectId,
                                project = new ProjectViewModel()
                                {
                                    name = timeTrackerProject.Project.name
                                },
                                description = timeTrackerProject.description,
                                totalHours = timeTrackerProject.totalHours,
                            }).ToList()
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

        [HttpPost]
        public HttpResponseMessage GetIsActive([FromBody] EmptyAuthenticationViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    bool isActive = context.TimeTrackers
                        .Where(i => i.memberId == a.member.memberId
                            && i.isActive
                            && !i.isDeleted)
                        .Count() > 0 ? true : false;

                    return Request.CreateResponse(HttpStatusCode.OK, isActive);

                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        [HttpPost]
        public HttpResponseMessage GetReport([FromBody] DateTimeOffsetViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    DateTimeOffset min = data.startDate.Date.AddDays(-1);
                    DateTimeOffset max = data.endDate.Date.AddDays(1);

                    var arr = context.TimeTrackers
                        .Where(i => DbFunctions.TruncateTime(i.dateCreated) >= min
                            && DbFunctions.TruncateTime(i.dateCreated) <= max
                            && !i.isActive
                            && !i.isDeleted)
                        .Select(obj => new TimeTrackerViewModel
                        {
                            timeTrackerId = obj.timeTrackerId,
                            member = new MemberViewModel()
                            {
                                token = obj.Member.token,
                                email = obj.Member.email,
                                firstName = obj.Member.firstName,
                                lastName = obj.Member.lastName,
                                isActive = obj.Member.TimeTrackers.Any(i => i.isActive)
                            },
                            dateCreated = obj.dateCreated,
                            dateIn = obj.dateIn,
                            dateOut = obj.dateOut,
                            totalHours = obj.totalHours,
                            isActive = obj.isActive,
                            timeTrackerProjects = obj.TimeTrackerProjects.Select(timeTrackerProject => new TimeTrackerProjectViewModel()
                            {
                                timeTrackerProjectId = timeTrackerProject.timeTrackerProjectId,
                                projectId = timeTrackerProject.projectId,
                                project = new ProjectViewModel()
                                {
                                    name = timeTrackerProject.Project.name
                                },
                                description = timeTrackerProject.description,
                                totalHours = timeTrackerProject.totalHours,
                            }).ToList()
                        })
                        .OrderByDescending(i => i.dateCreated)
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
        public HttpResponseMessage AutomateOut([FromBody] AutomateViewModel data)
        {

            if (!data.authenticationId.Equals("3cc44327063f42919487e9a021eb958a54e37d7b9f9346d78eecc10c3d38796ce4cf2a3980a94dfea568639473bd30fa79de697819504d5b9293e5ca6dd3163c32e865c87e8c438488fa1414fa7f83e9"))
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Invalid Token" });

            string path = "C:\\logs\\_timeTrackerOut\\log_" + DateTime.UtcNow.ToLongDateString() + ".txt";

            try
            {

                BambinoDataContext context = new BambinoDataContext();

                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(DateTime.UtcNow.ToLongDateString());
                    }
                }

                var timeTrackers = context.TimeTrackers
                    .Where(i => i.isActive
                        && !i.isDeleted)
                    .Select(obj => new 
                    {
                        member = new
                        {
                            firstName = obj.Member.firstName,
                            email = obj.Member.email
                        }
                    })
                    .ToList();

                //new Thread(() =>
                //{
                //    EmailController.Send(new MailAddress(EmailController.email),
                //        "jacobb@entdesign.com",
                //        EmailController.email,
                //        EmailController.email,
                //        "Bambino: Forget Something?",
                //        String.Format("Now {0}, why are you working so late? Remember to clock out!", timeTrackers.Count().ToString()));
                //}).Start();

                foreach (var timeTracker in timeTrackers)
                {

                    new Thread(() =>
                    {
                        EmailController.Send(new MailAddress(EmailController.email),
                            timeTracker.member.email,
                            EmailController.email,
                            EmailController.email,
                            "Bambino: Forget Something?",
                            String.Format("Now {0}, why are you working so late? <a href='https://desktop.bambino.software' target='_blank'>Please remember to clock out</a>!", timeTracker.member.firstName));
                    }).Start();

                }

                return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Success" });

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

    }
}
