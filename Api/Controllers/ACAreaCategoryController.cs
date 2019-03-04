using Api.Models;
using System;
using System.Collections.Generic;
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

            BambinoDataContext context = new BambinoDataContext();

            ACAreaCategory acAreaCategory = (data.acAreaCategoryId == Guid.Empty) ? new ACAreaCategory() : context.ACAreaCategories
                .Where(i => i.acAreaCategoryId == data.acAreaCategoryId
                    && !i.isDeleted)
                .FirstOrDefault();

            if (acAreaCategory == null)
                throw new InvalidOperationException("Not Found");

            acAreaCategory.name = data.name;
            acAreaCategory.isDeleted = data.isDeleted;

            if (data.acAreaCategoryId == Guid.Empty)
                context.ACAreaCategories.InsertOnSubmit(acAreaCategory);

            context.SubmitChanges();

            var activity = (data.acAreaCategoryId == Guid.Empty) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
            //LogController.Add(a.member.memberId, "Template Category " + category.name + " " + activity, "Category", "AddEditDelete", category.acLayerCategoryId, "Categories");

            return new AddEditDeleteReturnViewModel()
            {
                id = acAreaCategory.acAreaCategoryId,
                state = (data.acAreaCategoryId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
            };
            
        }

        [HttpPost]
        public HttpResponseMessage _Get([FromBody] EmptyAuthenticationViewModel data)
        {
            //Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            //if (a.isAuthenticated)
            //{

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    var arr = context.ACAreaCategories
                        .Where(i => !i.isDeleted)
                        .Select(obj => new ACAreaCategoryViewModel
                        {
                            acAreaCategoryId = obj.acAreaCategoryId,
                            name = obj.name
                        })
                        .OrderBy(i => i.name)
                        .ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, arr);

                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            //}
            //return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
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

            BambinoDataContext context = new BambinoDataContext();
            
            Expression<Func<ACAreaCategory, bool>> query = i => !i.isDeleted
                    && i.name.Contains(data.search);
            
            int currentPage = data.page - 1;
            int skip = currentPage * data.records;
            int totalRecords = context.ACAreaCategories.Where(query).Count();
            var arr = context.ACAreaCategories
                .Where(query)
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

            BambinoDataContext context = new BambinoDataContext();

            return context.ACAreaCategories
                .Where(i => i.acAreaCategoryId == data.id
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
