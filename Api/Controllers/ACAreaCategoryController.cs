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
    public class ACAreaCategoryController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] ACAreaCategoryAddEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _AddEditDelete(data));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public static AddEditDeleteReturnViewModel _AddEditDelete(ACAreaCategoryAddEditDeleteViewModel data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            ACAreaCategory acAreaCategory = (data.acAreaCategoryId == Guid.Empty) ? new ACAreaCategory() : unitOfWork.ACAreaCategoryRepository
                .GetBy(i => i.acAreaCategoryId == data.acAreaCategoryId
                    && !i.isDeleted)
                .FirstOrDefault();

            if (acAreaCategory == null)
                throw new InvalidOperationException("Not Found");

            acAreaCategory.name = data.name;
            acAreaCategory.isDeleted = data.isDeleted;

            if (data.acAreaCategoryId == Guid.Empty)
                unitOfWork.ACAreaCategoryRepository.Insert(acAreaCategory);
            else
                unitOfWork.ACAreaCategoryRepository.Update(acAreaCategory);

            unitOfWork.Save();

            var activity = (data.acAreaCategoryId == Guid.Empty) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
            //LogController.Add(a.member.memberId, "Template Category " + category.name + " " + activity, "Category", "AddEditDelete", category.acLayerCategoryId, "Categories");

            return new AddEditDeleteReturnViewModel()
            {
                id = acAreaCategory.acAreaCategoryId,
                state = (data.acAreaCategoryId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
            };
            
        }

        [HttpPost]
        public HttpResponseMessage Get([FromBody] EmptyAuthenticationViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _Get());
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public static List<ACAreaCategoryViewModel> _Get()
        {

            //return new List<TagAreaTypeViewModel>() {
            //    new TagAreaTypeViewModel() { name = "GUEST" },
            //    new TagAreaTypeViewModel() { name = "BOH" }
            //};

            UnitOfWork unitOfWork = new UnitOfWork();

            return unitOfWork.ACAreaCategoryRepository
                .GetBy(i => !i.isDeleted)
                .Select(obj => new ACAreaCategoryViewModel
                {
                    acAreaCategoryId = obj.acAreaCategoryId,
                    name = obj.name
                })
                .OrderBy(i => i.name)
                .ToList();

        }

        [HttpPost]
        public HttpResponseMessage GetByPage([FromBody] SearchViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _GetByPage(data));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public class GetByPageReturnViewModel
        {
            public int totalRecords { get; set; }
            public double totalPages { get; set; }
            public List<ACAreaCategoryViewModel> acAreaCategories { get; set; }
        }

        public static GetByPageReturnViewModel _GetByPage(SearchViewModel data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            var query = unitOfWork.ACAreaCategoryRepository
                .GetBy(i => !i.isDeleted
                    && i.name.Contains(data.search));

            int currentPage = data.page - 1;
            int skip = currentPage * data.records;
            int totalRecords = query.ToList().Count;
            var arr = query
                .Select(obj => new ACAreaCategoryViewModel
                {
                    acAreaCategoryId = obj.acAreaCategoryId,
                    name = obj.name
                })
                .OrderBy(data.sort)
                .Skip(skip)
                .Take(data.records)
                .ToList();

            if (arr == null)
                throw new InvalidOperationException("Not Found");

            return new GetByPageReturnViewModel()
            {
                totalRecords = totalRecords,
                totalPages = Math.Ceiling((double)totalRecords / data.records),
                acAreaCategories = arr
            };

        }

        [HttpPost]
        public HttpResponseMessage GetById([FromBody] GetByIdViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _GetById(data));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public static ACAreaCategoryViewModel _GetById(GetByIdViewModel data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            return unitOfWork.ACAreaCategoryRepository
                .GetBy(i => i.acAreaCategoryId == data.id
                    && !i.isDeleted)
                .Select(obj => new ACAreaCategoryViewModel
                {
                    acAreaCategoryId = obj.acAreaCategoryId,
                    name = obj.name,
                    isDeleted = obj.isDeleted
                })
                .FirstOrDefault();
            
        }

    }
}