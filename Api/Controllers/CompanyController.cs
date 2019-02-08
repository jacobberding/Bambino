using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CompanyController : ApiController
    {
        
        [HttpPost]
        public async Task<HttpResponseMessage> AddEditDelete([FromBody] CompanyAddEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    
                    BambinoDataContext context = new BambinoDataContext();

                    Company company = (data.companyId == Guid.Empty) ? new Company() : context.Companies
                        .Where(i => i.companyId == data.companyId
                            && !i.isDeleted)
                        .FirstOrDefault();
                    
                    if (company == null)
                        throw new InvalidOperationException("Not Found");
                    
                    company.type                    = data.type;
                    company.terms                   = data.terms;
                    company.name                    = data.name;
                    company.pin                     = data.pin;
                    company.email                   = data.email;
                    company.phone                   = data.phone;
                    company.website                 = data.website;
                    company.billingAddressLine1     = data.billingAddressLine1;
                    company.billingAddressLine2     = data.billingAddressLine2;
                    company.billingCity             = data.billingCity;
                    company.billingState            = data.billingState;
                    company.billingZip              = data.billingZip;
                    company.shippingAddressLine1    = data.shippingAddressLine1;
                    company.shippingAddressLine2    = data.shippingAddressLine2;
                    company.shippingCity            = data.shippingCity;
                    company.shippingState           = data.shippingState;
                    company.shippingZip             = data.shippingZip;
                    company.isApproved              = data.isApproved;
                    company.isDeleted               = data.isDeleted;
                    
                    if (data.companyId == Guid.Empty)
                        context.Companies.InsertOnSubmit(company);

                    context.SubmitChanges();
                    
                    var activity = (data.companyId == Guid.Empty) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited"; 
                    LogController.Add(a.member.memberId, "Company " + company.name + " " + activity, "Company", "AddEditDelete", company.companyId, "Companies");

                    var vm = new {
                        companyId = company.companyId,
                        state = (data.companyId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
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
        public HttpResponseMessage EditSettings([FromBody] CompanyEditSettingsViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    
                    BambinoDataContext context = new BambinoDataContext();

                    Company company = context.Companies
                        .Where(i => i.companyId == data.companyId
                            && !i.isDeleted)
                        .FirstOrDefault();
                    
                    if (company == null)
                        throw new InvalidOperationException("Not Found");
                    
                    company.terms = data.terms;
                    company.pin = data.pin;
                    
                    context.SubmitChanges();
                    
                    LogController.Add(a.member.memberId, "Company " + company.name + " Settings Edited", "Company", "EditSettings", company.companyId, "Companies");

                    var vm = new {
                        companyId = company.companyId,
                        state = "edit"
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
        public HttpResponseMessage Get([FromBody] EmptyAuthenticationViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    
                    BambinoDataContext context = new BambinoDataContext();

                    List<ListViewModel> vm = context.Companies
                        .Select(obj => new ListViewModel()
                        {
                            value = obj.companyId.ToString(),
                            name = obj.name
                        })
                        .OrderBy(i => i.name)
                        .ToList();
                    
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

                    var query = context.Companies
                        .Where(i => !i.isDeleted
                            && (i.name.Contains(data.search)
                            || i.email.Contains(data.search)
                            //|| i.companyLocations.Select(x => x.name).FirstOrDefault().Contains(data.search)
                            //|| i.companyLocations.Select(x => x.state).FirstOrDefault().Contains(data.search)
                            //|| i.companyLocations.Select(x => x.city).FirstOrDefault().Contains(data.search)
                            //|| i.companyLocations.Select(x => x.zip).FirstOrDefault().Contains(data.search)
                            ));

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = query.ToList().Count;
                    var arr = query
                        .Select(obj => new CompanyViewModel
                        {
                            companyId               = obj.companyId,
                            name                    = obj.name,
                            email                   = obj.email,
                            type                    = obj.type,
                            terms                   = obj.terms,
                            phone                   = obj.phone,
                            website                 = obj.website,
                            billingAddressLine1     = obj.billingAddressLine1,
                            billingAddressLine2     = obj.billingAddressLine2,
                            billingCity             = obj.billingCity,
                            billingState            = obj.billingState,
                            billingZip              = obj.billingZip,
                            shippingAddressLine1    = obj.shippingAddressLine1,
                            shippingAddressLine2    = obj.shippingAddressLine2,
                            shippingCity            = obj.shippingCity,
                            shippingState           = obj.shippingState,
                            shippingZip             = obj.shippingZip,
                            isApproved              = obj.isApproved
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
                    
                    var vm = context.Companies
                        .Where(i => i.companyId == data.id
                            && !i.isDeleted)
                        .Select(obj => new CompanyViewModel
                        {
                            companyId               = obj.companyId,
                            type                    = obj.type,
                            terms                   = obj.terms,
                            name                    = obj.name,
                            pin                     = obj.pin,
                            email                   = obj.email,
                            phone                   = obj.phone,
                            website                 = obj.website,
                            billingAddressLine1     = obj.billingAddressLine1,
                            billingAddressLine2     = obj.billingAddressLine2,
                            billingCity             = obj.billingCity,
                            billingState            = obj.billingState,
                            billingZip              = obj.billingZip,
                            shippingAddressLine1    = obj.shippingAddressLine1,
                            shippingAddressLine2    = obj.shippingAddressLine2,
                            shippingCity            = obj.shippingCity,
                            shippingState           = obj.shippingState,
                            shippingZip             = obj.shippingZip,
                            isApproved              = obj.isApproved,
                            members                 = obj.MemberCompanies.Select(memberCompany => new MemberViewModel()
                            {
                                memberId            = memberCompany.Member.memberId,
                                activeCompanyId     = memberCompany.Member.activeCompanyId,
                                firstName           = memberCompany.Member.firstName,
                                lastName            = memberCompany.Member.lastName,
                                email               = memberCompany.Member.email,
                                phone               = memberCompany.Member.phone
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
        public HttpResponseMessage GetSignUp([FromBody] SearchViewModel data)
        {
            Authentication a = AuthenticationController.GetApiAuthenticated(data.authentication.apiId, 1);
            if (a.isAuthenticated)
            {

                try
                {
                    
                    BambinoDataContext context = new BambinoDataContext();

                    var vm = context.Companies
                        .Where(i => i.name.Contains(data.search)
                            && !i.isDeleted)
                        .Select(obj => new
                        {
                            companyId = obj.companyId,
                            name = obj.name,
                            email = obj.email
                        })
                        .Take(5)
                        .ToList();
                    
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
