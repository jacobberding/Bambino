using Api.Models;
using LMB.PredicateBuilderExtension;
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

                    Contact contact = (data.contactKey == 0) ? new Contact() : context.Contacts
                        .Where(i => i.contactKey == data.contactKey
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (contact == null)
                        throw new InvalidOperationException("Layer Not Found");

                    contact.contactCompanyKey = data.contactCompanyKey;
                    contact.name = data.name;
                    contact.title = data.title;
                    contact.companyName = "";
                    contact.phone1 = data.phone1;
                    contact.phone2 = data.phone2;
                    contact.skypeId = data.skypeId;
                    contact.email = data.email;
                    contact.companyTemp = "";
                    contact.resume = "";
                    contact.portfolio = "";
                    contact.personalWebsite = data.personalWebsite;
                    contact.skills = data.skills;
                    contact.dateCreated = DateTimeOffset.UtcNow;
                    contact.isEdcFamily = data.isEdcFamily;
                    contact.isPotentialStaffing = data.isPotentialStaffing;
                    contact.isDeleted = data.isDeleted;

                    if (data.contactKey == 0)
                        context.Contacts.InsertOnSubmit(contact);

                    context.SubmitChanges();

                    var activity = (data.contactKey == 0) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
                    LogController.Add(a.member.memberId, String.Format("Contact {0} was {1}", contact.name, activity), "Contact", "AddEditDelete", Guid.Empty, "Contacts", contact.contactKey);

                    var vm = new 
                    {
                        contact.contactKey,
                        state = (data.contactKey == 0) ? "add" : (data.isDeleted) ? "delete" : "edit"
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
                    //var predicate = PredicateBuilderExtension.True<Contact>();
                    //predicate.And(i => !i.isDeleted);

                    if (arrSearch.Count > 1)
                    {

                        List<string> q = new List<string>();
                        
                        foreach (var str in arrSearch)
                        {

                            if (str == "")
                                continue;

                            q.Add(String.Format("skills.Contains(\"{0}\")", str));

                            //predicate.Or(i => i.skills.Contains(str));

                        }

                        query += " AND (" + String.Join(" OR ", q.ToArray()) + ")";

                    }
                    else
                    {

                        //predicate.Or(i => i.name.Contains(data.search));
                        //predicate.Or(i => i.companyName.Contains(data.search));
                        //predicate.Or(i => i.personalWebsite.Contains(data.search));
                        //predicate.Or(i => i.companyTemp.Contains(data.search));
                        //predicate.Or(i => i.title.Contains(data.search));
                        //predicate.Or(i => i.skills.Contains(data.search));
                        //predicate.Or(i => i.email.Contains(data.search));
                        //query = i => i.name.Contains(data.search)
                        //    && i.companyName.Contains(data.search)
                        //    && i.personalWebsite.Contains(data.search)
                        //    && i.companyTemp.Contains(data.search)
                        //    && i.title.Contains(data.search)
                        //    && i.skills.Contains(data.search)
                        //    && i.email.Contains(data.search);

                        query += @" AND (" + String.Format("name.Contains(\"{0}\")", data.search) + @"
                            OR " + String.Format("ContactCompany.name.Contains(\"{0}\")", data.search) + @"
                            OR " + String.Format("personalWebsite.Contains(\"{0}\")", data.search) + @"
                            OR " + String.Format("companyTemp.Contains(\"{0}\")", data.search) + @"
                            OR " + String.Format("title.Contains(\"{0}\")", data.search) + @"
                            OR " + String.Format("skills.Contains(\"{0}\")", data.search) + @"
                            OR " + String.Format("email.Contains(\"{0}\")", data.search) + @")";

                    }

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = context.Contacts.Where(query).Count();
                    var arr = context.Contacts
                        .Where(query)
                        .Select(obj => new 
                        {
                            obj.contactKey,
                            obj.name,
                            contactCompany = new
                            {
                                obj.ContactCompany.name
                            },
                            obj.email,
                            obj.skills,
                            obj.title,
                            obj.personalWebsite,
                            obj.isDeleted
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
                        .Where(i => i.contactKey == data.key
                            && !i.isDeleted)
                        .Select(obj => new
                        {
                            obj.contactKey,
                            obj.contactCompanyKey,
                            obj.name,
                            obj.email,
                            obj.dateCreated,
                            obj.isEdcFamily,
                            obj.isDeleted,
                            obj.isPotentialStaffing,
                            obj.personalWebsite,
                            contactCompany = new
                            {
                                obj.ContactCompany.contactCompanyKey,
                                obj.ContactCompany.addressLine1,
                                obj.ContactCompany.addressLine2,
                                obj.ContactCompany.city,
                                obj.ContactCompany.email,
                                obj.ContactCompany.isClient,
                                obj.ContactCompany.isDeleted,
                                obj.ContactCompany.isVendorDesign,
                                obj.ContactCompany.isVendorIntegration,
                                obj.ContactCompany.name,
                                obj.ContactCompany.phone,
                                obj.ContactCompany.state,
                                obj.ContactCompany.country,
                                obj.ContactCompany.website,
                                obj.ContactCompany.zip
                            },
                            contactFiles = obj.ContactFiles.Where(i => !i.isDeleted).Select(contactFile => new
                            {
                                contactFile.contactFileKey,
                                contactFile.contactKey,
                                contactFile.name,
                                contactFile.path,
                                contactFile.originalFileName,
                                contactFile.isDeleted
                            }).ToList(),
                            obj.phone1,
                            obj.phone2,
                            obj.skills,
                            obj.skypeId,
                            obj.title
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
                        .Where(i => (i.ContactCompany.name.Contains("Thinkwell")
                            || i.ContactCompany.name.Contains("BRC")
                            || i.ContactCompany.name.Contains("MYCOTOO")
                            || i.ContactCompany.name.Contains("Hettema")
                            || i.ContactCompany.name.Contains("Nassel")
                            || i.ContactCompany.name.Contains("Rethink")
                            || i.ContactCompany.name.Contains("Theme Space")
                            || i.ContactCompany.name.Contains("Rhetroactive")
                            || i.ContactCompany.name.Contains("JRA")
                            || i.ContactCompany.name.Contains("Forrec")
                            || i.ContactCompany.name.Contains("Luc Studios")
                            || i.ContactCompany.name.Contains("Falcon Creative")
                            || i.ContactCompany.name.Contains("Five Currents")
                            || i.ContactCompany.name.Contains("Legacy Design & Entertainment")
                            || i.ContactCompany.name.Contains("Visual Terrain")
                            || i.ContactCompany.name.Contains("IdeAttack")
                            || i.ContactCompany.name.Contains("Dynamic Structures")
                            || i.ContactCompany.name.Contains("Dragone Studios")
                            || i.ContactCompany.name.Contains("Imagine Exhibits")
                            || i.ContactCompany.name.Contains("Ty Granaroli")
                            || i.ContactCompany.name.Contains("Daniel's Woodland")
                            || i.ContactCompany.name.Contains("Dynamic Attractions"))
                            && !i.isDeleted)
                        .Select(obj => new 
                        {
                            contactKey = obj.contactKey,
                            company = obj.ContactCompany.name,
                            name = obj.name,
                            email = obj.email,
                            skills = obj.skills
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
