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

                    UnitOfWork unitOfWork = new UnitOfWork();

                    Contact contact = (data.contactId == Guid.Empty) ? new Contact() : unitOfWork.ContactRepository
                        .GetBy(i => i.contactId == data.contactId
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
                        unitOfWork.ContactRepository.Insert(contact);
                    else
                        unitOfWork.ContactRepository.Update(contact);

                    unitOfWork.Save();

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

                    UnitOfWork unitOfWork = new UnitOfWork();
                
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

                        unitOfWork.ContactRepository.Insert(contact);
                        unitOfWork.Save();

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

                    UnitOfWork unitOfWork = new UnitOfWork();

                    List<string> arrSearch = data.search.Split(',').ToList();
                    var query = unitOfWork.ContactRepository
                        .GetBy(i => !i.isDeleted);

                    if (arrSearch.Count > 1)
                    {

                        List<string> q = new List<string>();

                        foreach(var str in arrSearch)
                        {

                            if (str == "")
                                continue;

                            q.Add(String.Format("skills.Contains(\"{0}\")", str));

                        }

                        query = query.Where(String.Join(" OR ", q.ToArray()));

                    }
                    else
                    {

                        query = query.Where(i => i.name.Contains(data.search)
                            || i.companyName.Contains(data.search)
                            || i.personalWebsite.Contains(data.search)
                            || i.companyTemp.Contains(data.search)
                            || i.title.Contains(data.search)
                            || i.skills.Contains(data.search)
                            || i.email.Contains(data.search));

                    }

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = query.ToList().Count;
                    var arr = query
                        .Select(obj => new ContactViewModel
                        {
                            contactId = obj.contactId,
                            name = (obj.name == "") ? obj.companyName : obj.name,
                            email = obj.email,
                            skills = obj.skills,
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

                    UnitOfWork unitOfWork = new UnitOfWork();

                    var vm = unitOfWork.ContactRepository
                        .GetBy(i => i.contactId == data.id
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

    }
}
