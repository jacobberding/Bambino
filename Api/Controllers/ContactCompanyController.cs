using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ContactCompanyController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] ContactCompanyAddEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    ContactCompany contactCompany = (data.contactCompanyKey == 0) ? new ContactCompany() : context.ContactCompanies
                        .Where(i => i.contactCompanyKey == data.contactCompanyKey
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (contactCompany == null)
                        throw new InvalidOperationException("Not Found");
                    
                    contactCompany.name = data.name;
                    contactCompany.email = data.email;
                    contactCompany.phone = data.phone;
                    contactCompany.website = data.website;
                    contactCompany.addressLine1 = data.addressLine1;
                    contactCompany.addressLine2 = data.addressLine2;
                    contactCompany.city = data.city;
                    contactCompany.state = data.state;
                    contactCompany.zip = data.zip;
                    contactCompany.isClient = data.isClient;
                    contactCompany.isVendor = data.isVendor;
                    contactCompany.isDeleted = data.isDeleted;

                    if (data.contactCompanyKey == 0)
                        context.ContactCompanies.InsertOnSubmit(contactCompany);

                    context.SubmitChanges();

                    var activity = (data.contactCompanyKey == 0) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
                    LogController.Add(a.member.memberId, "Contact Company " + contactCompany.name + " " + activity, "ContactCompany", "AddEditDelete", Guid.Empty, "ContactCompanies", contactCompany.contactCompanyKey);

                    var vm = new 
                    {
                        contactCompany.contactCompanyKey,
                        state = (data.contactCompanyKey == 0) ? "add" : (data.isDeleted) ? "delete" : "edit"
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

                    List<ListViewModel> vm = context.ContactCompanies
                        .Select(obj => new ListViewModel()
                        {
                            value = obj.contactCompanyKey.ToString(),
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

                    Expression<Func<ContactCompany, bool>> query = i => !i.isDeleted
                            && (i.name.Contains(data.search)
                            || i.email.Contains(data.search));

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = context.ContactCompanies.Where(query).Count();
                    var arr = context.ContactCompanies
                        .Where(query)
                        .Select(obj => new
                        {
                            obj.contactCompanyKey,
                            obj.name,
                            obj.email,
                            obj.phone,
                            obj.website,
                            obj.addressLine1,
                            obj.addressLine2,
                            obj.city,
                            obj.state,
                            obj.zip,
                            obj.isVendor,
                            obj.isClient,
                            obj.isDeleted
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

                    var vm = context.ContactCompanies
                        .Where(i => i.contactCompanyKey == data.key
                            && !i.isDeleted)
                        .Select(obj => new 
                        {
                            obj.contactCompanyKey,
                            obj.name,
                            obj.email,
                            obj.phone,
                            obj.website,
                            obj.addressLine1,
                            obj.addressLine2,
                            obj.city,
                            obj.state,
                            obj.zip,
                            obj.isVendor,
                            obj.isClient,
                            obj.isDeleted,
                            contacts = obj.Contacts.Select(contact => new 
                            {
                                contact.contactKey,
                                contact.name,
                                contact.email
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

    }
}
