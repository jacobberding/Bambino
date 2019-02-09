using Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public class ContactController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] ContactAddEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Contact contact = (data.contactId == Guid.Empty) ? new Contact() : context.Contacts
                        .Where(i => i.contactId == data.contactId
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (contact == null)
                        throw new InvalidOperationException("Layer Not Found");
                
                    contact.name = data.name;
                    contact.title = data.title;
                    contact.companyName = data.companyName;
                    contact.phone1 = data.phone1;
                    contact.phone2 = data.phone2;
                    contact.skypeId = data.skypeId;
                    contact.email = data.email;
                    contact.companyTemp = data.companyTemp;
                    contact.resume = data.resume;
                    contact.portfolio = data.portfolio;
                    contact.personalWebsite = data.personalWebsite;
                    contact.skills = data.skills;
                    contact.isEdcFamily = data.isEdcFamily;
                    contact.isPotentialStaffing = data.isPotentialStaffing;
                    contact.isDeleted = data.isDeleted;

                    if (data.contactId == Guid.Empty)
                        context.Contacts.InsertOnSubmit(contact);

                    context.SubmitChanges();

                    var activity = (data.contactId == Guid.Empty) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
                    LogController.Add(a.member.memberId, String.Format("Contact {0} was {1}", contact.name, activity), "Contact", "AddEditDelete", contact.contactId, "Contacts");

                    var vm = new AddEditDeleteReturnViewModel()
                    {
                        id = contact.contactId,
                        state = (data.contactId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
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
        public HttpResponseMessage Upload([FromBody] ContactUploadViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();
                
                    foreach (var contactViewModel in data.contactViewModels)
                    {

                        Contact contact = new Contact();

                        contact.name = contactViewModel.name;
                        contact.title = contactViewModel.title;
                        contact.companyName = contactViewModel.companyName;
                        contact.phone1 = contactViewModel.phone1;
                        contact.phone2 = contactViewModel.phone2;
                        contact.skypeId = contactViewModel.skypeId;
                        contact.email = contactViewModel.email;
                        contact.companyTemp = contactViewModel.companyTemp;
                        contact.resume = contactViewModel.resume;
                        contact.portfolio = contactViewModel.portfolio;
                        contact.personalWebsite = contactViewModel.personalWebsite;
                        contact.skills = contactViewModel.skills;
                        contact.isEdcFamily = contactViewModel.isEdcFamily;
                        contact.isPotentialStaffing = contactViewModel.isPotentialStaffing;
                        //contact.dateCreated = contactViewModel.dateCreated;

                        context.Contacts.InsertOnSubmit(contact);
                        context.SubmitChanges();

                    }

                    var vm = new { };

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

                    List<string> arrSearch = data.search.Split(',').ToList();
                    string query = "isDeleted = false";

                    if (arrSearch.Count > 1)
                    {

                        List<string> q = new List<string>();

                        foreach(var str in arrSearch)
                        {

                            if (str == "")
                                continue;

                            q.Add(String.Format("skills.Contains(\"{0}\")", str));

                        }

                        query += " AND " + String.Join(" OR ", q.ToArray());

                    }
                    else
                    {

                        query += @" AND " + String.Format("name.Contains(\"{0}\")", data.search) + @"
                            OR " + String.Format("companyName.Contains(\"{0}\")", data.search) + @"
                            OR " + String.Format("personalWebsite.Contains(\"{0}\")", data.search) + @"
                            OR " + String.Format("companyTemp.Contains(\"{0}\")", data.search) + @"
                            OR " + String.Format("title.Contains(\"{0}\")", data.search) + @"
                            OR " + String.Format("skills.Contains(\"{0}\")", data.search) + @"
                            OR " + String.Format("email.Contains(\"{0}\")", data.search) + @"";

                    }

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = context.Contacts.Where(query).Count();
                    var arr = context.Contacts
                        .Where(query)
                        .Select(obj => new ContactViewModel
                        {
                            contactId = obj.contactId,
                            name = (obj.name == "") ? obj.companyName : obj.name,
                            email = obj.email,
                            skills = obj.skills,
                            title = obj.title,
                            personalWebsite = obj.personalWebsite
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

        [HttpPost]
        public HttpResponseMessage GetById([FromBody] GetByIdViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    var vm = context.Contacts
                        .Where(i => i.contactId == data.id
                            && !i.isDeleted)
                        .Select(obj => new ContactViewModel
                        {
                            contactId = obj.contactId,
                            name = obj.name,
                            email = obj.email,
                            companyName = obj.companyName,
                            companyTemp = obj.companyTemp,
                            dateCreated = obj.dateCreated,
                            isEdcFamily = obj.isEdcFamily,
                            isDeleted = obj.isDeleted,
                            isPotentialStaffing = obj.isPotentialStaffing,
                            personalWebsite = obj.personalWebsite,
                            contactFiles = obj.ContactFiles.Select(contactFile => new ContactFileViewModel() {
                                contactFileId = contactFile.contactFileId,
                                contactId = contactFile.contactId,
                                name = contactFile.name,
                                path = contactFile.path,
                                originalFileName = contactFile.originalFileName,
                                isDeleted = contactFile.isDeleted
                            }).ToList(),
                            phone1 = obj.phone1,
                            phone2 = obj.phone2,
                            portfolio = obj.portfolio,
                            resume = obj.resume,
                            skills = obj.skills,
                            skypeId = obj.skypeId,
                            title = obj.title
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
        public HttpResponseMessage GetReport([FromBody] EmptyAuthenticationViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();
                    
                    var arr = context.Contacts
                        .Where(i => !i.isDeleted)
                        .Select(obj => new ContactViewModel
                        {
                            contactId = obj.contactId,
                            name = (obj.name == "") ? obj.companyName : obj.name,
                            email = obj.email
                        })
                        .OrderBy(i => i.name)
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

    }
}
