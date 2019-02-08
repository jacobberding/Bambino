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
            
            BambinoDataContext context = new BambinoDataContext();

            Log log = new Log();
            
            log.memberId = memberId;
            log.activity = activity;
            log.controllerName = controllerName;
            log.methodName = methodName;
            log.tableId = tableId;
            log.tableName = tableName;

            context.Logs.InsertOnSubmit(log);
            context.SubmitChanges();
            
        }
        
        [HttpPost]
        public HttpResponseMessage GetByTableName([FromBody] LogGetByTableNameViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {
            
                try
                {
                    
                    BambinoDataContext context = new BambinoDataContext();

                    var query = context.Logs
                        .Where(i => data.tableNames.Contains(i.tableName)
                            && (i.tableName.Contains(data.search) 
                            || i.activity.Contains(data.search)
                            || String.Concat(i.Member.firstName, " ", i.Member.lastName).Contains(data.search)
                            || i.Member.email.Contains(data.search)
                            || i.Member.MemberCompanies.Any(x => x.Company.name.Contains(data.search))));

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
                                memberId        = obj.Member.memberId,
                                email           = obj.Member.email,
                                phone           = obj.Member.phone,
                                firstName       = obj.Member.firstName,
                                lastName        = obj.Member.lastName,
                                companies       = obj.Member.MemberCompanies.Select(memberCompany => new CompanyViewModel()
                                {
                                    name        = memberCompany.Company.name
                                }).ToList()
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
