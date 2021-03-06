﻿using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ContactFileController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddMany([FromBody] ContactFileAddManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();
                    
                    foreach (var contactFileViewModel in data.contactFiles)
                    {

                        ContactFile contactFile = new ContactFile();

                        contactFile.contactFileKey = contactFileViewModel.contactFileKey;
                        contactFile.contactKey = contactFileViewModel.contactKey;
                        contactFile.name = "";
                        contactFile.path = contactFileViewModel.path;
                        contactFile.originalFileName = contactFileViewModel.originalFileName;
                        contactFile.isDeleted = contactFileViewModel.isDeleted;
                        
                        context.ContactFiles.InsertOnSubmit(contactFile);
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
        public HttpResponseMessage Delete([FromBody] ContactFileDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();
                    
                    ContactFile contactFile = context.ContactFiles
                        .Where(i => i.contactFileKey == data.contactFileKey
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (contactFile == null)
                        throw new InvalidOperationException("Not Found");
                    
                    contactFile.isDeleted = data.isDeleted;

                    context.SubmitChanges();
                    
                    var vm = new
                    {
                        contactFile.contactFileKey,
                        state = "delete"
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
