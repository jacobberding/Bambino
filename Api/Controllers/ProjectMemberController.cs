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
    public class ProjectMemberController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage GetProjectsByMember([FromBody] EmptyAuthenticationViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    UnitOfWork unitOfWork = new UnitOfWork();

                    bool isActive = unitOfWork.TimeTrackerRepository
                        .GetBy(i => i.memberId == a.member.memberId
                            && i.isActive
                            && !i.isDeleted)
                        .ToList()
                        .Count() > 0 ? true : false;

                    if (!isActive)
                        throw new InvalidOperationException("Not Clocked In Yet");

                    List<ProjectViewModel> projects = unitOfWork.ProjectRepository
                        .GetBy(i => i.members.Any(x => x.memberId == a.member.memberId)
                            && !i.isDeleted)
                        .Select(obj => new ProjectViewModel
                        {
                            projectId = obj.projectId,
                            code = obj.code,
                            name = obj.name
                        })
                        .OrderBy(i => i.name)
                        .ToList();

                    if (projects == null)
                        throw new InvalidOperationException("Not Found");

                    var vm = new
                    {
                        projects = projects,
                        dateIn = unitOfWork.TimeTrackerRepository
                            .GetBy(i => i.memberId == a.member.memberId
                                && i.isActive
                                && !i.isDeleted)
                            .FirstOrDefault()
                            .dateIn
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

                    UnitOfWork unitOfWork = new UnitOfWork();

                    var query = unitOfWork.MemberRepository
                        .GetBy(i => !i.isDeleted 
                            && (i.email.Contains(data.search)
                            || String.Concat(i.firstName, " ", i.lastName).Contains(data.search)
                            || i.phone.Contains(data.search)
                            || i.roles.Select(x => x.name).FirstOrDefault().Contains(data.search)));

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = query.ToList().Count;
                    var arr = query
                        .Select(obj => new MemberViewModel
                        {
                            memberId = obj.memberId,
                            firstName = obj.firstName,
                            lastName = obj.lastName,
                            email = obj.email,
                            phone = obj.phone,
                            isActive = obj.timeTrackers.Any(i => i.isActive),
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

    }
}
