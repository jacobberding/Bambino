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
                    contactCompany.country = data.country;
                    contactCompany.zip = data.zip;
                    contactCompany.isClient = data.isClient;
                    contactCompany.isVendorDesign = data.isVendorDesign;
                    contactCompany.isVendorIntegration = data.isVendorIntegration;
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
        public HttpResponseMessage AddDiscipline([FromBody] AddDeleteManyToManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    ContactCompany contactCompany = context.ContactCompanies.Where(i => i.contactCompanyKey == data.tableKey).FirstOrDefault();
                    Discipline discipline = context.Disciplines.Where(i => i.value == data.name).FirstOrDefault();

                    ContactCompanyDiscipline contactCompanyDiscipline = new ContactCompanyDiscipline();

                    contactCompanyDiscipline.contactCompanyKey = contactCompany.contactCompanyKey;
                    contactCompanyDiscipline.disciplineKey = discipline.disciplineKey;

                    context.ContactCompanyDisciplines.InsertOnSubmit(contactCompanyDiscipline);
                    context.SubmitChanges();

                    LogController.Add(a.member.memberId, "Contact Company " + contactCompany.name + " Added Discipline " + discipline.name, "ContactCompany", "AddDiscipline", Guid.Empty, "ContactCompanies", contactCompany.contactCompanyKey);

                    var vm = new
                    {
                        manyKey = discipline.disciplineKey,
                        discipline.name,
                        discipline.value
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
        public HttpResponseMessage DeleteDiscipline([FromBody] AddDeleteManyToManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    ContactCompany contactCompany = context.ContactCompanies.Where(i => i.contactCompanyKey == data.tableKey).FirstOrDefault();
                    Discipline discipline = context.Disciplines.Where(i => i.disciplineKey == data.manyKey).FirstOrDefault();

                    ContactCompanyDiscipline contactCompanyDiscipline = context.ContactCompanyDisciplines.Where(i => i.contactCompanyKey == data.tableKey && i.disciplineKey == data.manyKey).FirstOrDefault();

                    context.ContactCompanyDisciplines.DeleteOnSubmit(contactCompanyDiscipline);
                    context.SubmitChanges();

                    LogController.Add(a.member.memberId, "Contact Company " + contactCompany.name + " Removed Discipline " + discipline.name, "ContactCompany", "DeleteDiscipline", Guid.Empty, "ContactCompanies", contactCompany.contactCompanyKey);

                    var vm = new
                    {
                        discipline.disciplineKey,
                        discipline.name,
                        discipline.value
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
                        .Where(i => !i.isDeleted)
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
                            || i.ContactCompanyDisciplines.Any(x => x.Discipline.name.Contains(data.search))
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
                            obj.country,
                            obj.zip,
                            obj.isVendorDesign,
                            obj.isVendorIntegration,
                            obj.isClient,
                            obj.isDeleted,
                            disciplines = obj.ContactCompanyDisciplines.Select(contactCompanyDiscipline => new
                            {
                                contactCompanyDiscipline.Discipline.disciplineKey,
                                contactCompanyDiscipline.Discipline.name,
                                contactCompanyDiscipline.Discipline.value
                            }).ToList()
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
                            obj.country,
                            obj.zip,
                            obj.isVendorDesign,
                            obj.isVendorIntegration,
                            obj.isClient,
                            obj.isDeleted,
                            contacts = obj.Contacts.Select(contact => new 
                            {
                                contact.contactKey,
                                contact.name,
                                contact.email,
                                contact.title,
                                contact.phone1
                            }).ToList(),
                            disciplines = obj.ContactCompanyDisciplines.Select(contactCompanyDiscipline => new
                            {
                                contactCompanyDiscipline.Discipline.disciplineKey,
                                contactCompanyDiscipline.Discipline.name,
                                contactCompanyDiscipline.Discipline.value
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
