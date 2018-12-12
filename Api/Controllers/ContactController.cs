﻿using Api.Models;
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
        public HttpResponseMessage Upload([FromBody] ContactUploadViewModel data)
        {
            //Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            //if (a.isAuthenticated)
            //{

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

            //}
            //return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        [HttpPost]
        public HttpResponseMessage GetByPage([FromBody] Search data)
        {
            //Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            //if (a.isAuthenticated)
            //{

                try
                {

                    UnitOfWork unitOfWork = new UnitOfWork();

                    List<string> arrSearch = data.search.Split(',').ToList();
                    var query = unitOfWork.ContactRepository
                        .GetBy(i => !i.isDeleted);
                    //i => !i.isDeleted
                    //&& (i.name.Contains(data.search)
                    //|| i.companyName.Contains(data.search)
                    //|| i.companyTemp.Contains(data.search)
                    //|| i.title.Contains(data.search)
                    //|| i.skills.Contains(data.search)
                    //|| i.email.Contains(data.search)));

                    if (arrSearch.Count > 1)
                    {

                        string q = "";

                        foreach(var str in arrSearch)
                        {
                            q += String.Format("skills LIKE %{0}%", str);
                        }
                        query = query.Where(q);

                    }

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = query.ToList().Count;
                    var arr = query
                        .Select(obj => new ContactViewModel
                        {
                            contactId = obj.contactId,
                            name = obj.name,
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
                        arr = arr.Select(obj => new ContactViewModel
                        {
                            contactId = obj.contactId,
                            name = obj.name,
                            email = obj.email,
                            skills = obj.skills,
                            personalWebsite = obj.personalWebsite
                        }).ToList()
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, vm);

                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            //}
            //return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        [HttpPost]
        public HttpResponseMessage GetById([FromBody] GetByIdViewModel data)
        {
            //Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            //if (a.isAuthenticated)
            //{

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

            //}
            //return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

    }
}