using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;
using Api.Models;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LogController : ApiController
    {
        
        public static void Add(Guid memberId, string activity, string controllerName, string methodName, Guid tableId, string tableName)
        {
            
            UnitOfWork unitOfWork = new UnitOfWork();

            Log log = new Log();
            
            log.memberId = memberId;
            log.activity = activity;
            log.controllerName = controllerName;
            log.methodName = methodName;
            log.tableId = tableId;
            log.tableName = tableName;

            unitOfWork.LogRepository.Insert(log);
            unitOfWork.Save();
            
        }
        
        [HttpPost]
        public HttpResponseMessage GetByTableName([FromBody] LogGetByTableNameViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {
            
                try
                {
                    
                    UnitOfWork unitOfWork = new UnitOfWork();

                    var query = unitOfWork.LogRepository
                        .GetBy(i => data.tableNames.Contains(i.tableName)
                            && (i.tableName.Contains(data.search) 
                            || i.activity.Contains(data.search)
                            || String.Concat(i.member.firstName, " ", i.member.lastName).Contains(data.search)
                            || i.member.email.Contains(data.search)
                            || i.member.company.name.Contains(data.search)));

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = query.ToList().Count;
                    var arr = query
                        .Select(obj => new LogViewModel()
                        {
                            logId           = obj.logId,
                            memberId        = obj.memberId,
                            member          = new MemberViewModel()
                            {
                                memberId        = obj.member.memberId,
                                email           = obj.member.email,
                                phone           = obj.member.phone,
                                firstName       = obj.member.firstName,
                                lastName        = obj.member.lastName,
                                company         = new CompanyViewModel()
                                {
                                    name        = obj.member.company.name
                                }
                            },
                            activity        = obj.activity,
                            controllerName  = obj.controllerName,
                            methodName      = obj.methodName,
                            tableName       = obj.tableName,
                            tableId         = obj.tableId,
                            createdDate     = obj.createdDate
                        })
                        .OrderByDescending(i => i.createdDate)
                        .Skip(skip)
                        .Take(data.records)
                        .ToList();
            
                    if (arr == null)
                        throw new InvalidOperationException("Not Found");
                    
                    var vm = new
                    {
                        totalRecords = totalRecords,
                        totalPages = Math.Ceiling((double)totalRecords / data.records),
                        arr = arr.Select(obj => new LogViewModel()
                        {
                            logId       = obj.logId,
                            memberId    = obj.memberId,
                            member      = new MemberViewModel()
                            {
                                memberId        = obj.member.memberId,
                                email           = obj.member.email,
                                phone           = obj.member.phone,
                                firstName       = obj.member.firstName,
                                lastName        = obj.member.lastName,
                                company         = new CompanyViewModel()
                                {
                                    name        = obj.member.company.name
                                }
                            },
                            activity        = obj.activity,
                            controllerName  = obj.controllerName,
                            methodName      = obj.methodName,
                            tableName       = obj.tableName,
                            tableId         = obj.tableId,
                            createdDate     = obj.createdDate
                        }).ToList()
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
