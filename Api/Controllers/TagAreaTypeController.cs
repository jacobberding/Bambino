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
    public class TagAreaTypeController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] TagAreaTypeAddEditDeleteViewModel data)
        {
            //Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            //if (a.isAuthenticated)
            //{

                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _AddEditDelete(data));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            //}
            //return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public static AddEditDeleteReturnViewModel _AddEditDelete(TagAreaTypeAddEditDeleteViewModel data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            TagAreaType tagAreaType = (data.tagAreaTypeId == Guid.Empty) ? new TagAreaType() : unitOfWork.TagAreaTypeRepository
                .GetBy(i => i.tagAreaTypeId == data.tagAreaTypeId
                    && !i.isDeleted)
                .FirstOrDefault();

            if (tagAreaType == null)
                throw new InvalidOperationException("Not Found");

            tagAreaType.name = data.name;
            tagAreaType.isDeleted = data.isDeleted;

            if (data.tagAreaTypeId == Guid.Empty)
                unitOfWork.TagAreaTypeRepository.Insert(tagAreaType);
            else
                unitOfWork.TagAreaTypeRepository.Update(tagAreaType);

            unitOfWork.Save();

            var activity = (data.tagAreaTypeId == Guid.Empty) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
            //LogController.Add(a.member.memberId, "Template Category " + category.name + " " + activity, "Category", "AddEditDelete", category.categoryId, "Categories");

            return new AddEditDeleteReturnViewModel()
            {
                id = tagAreaType.tagAreaTypeId,
                state = (data.tagAreaTypeId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
            };
            
        }

        [HttpPost]
        public HttpResponseMessage Get([FromBody] Empty data)
        {
            //Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            //if (a.isAuthenticated)
            //{

                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _Get());
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            //}
            //return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public static List<TagAreaTypeViewModel> _Get()
        {

            //return new List<TagAreaTypeViewModel>() {
            //    new TagAreaTypeViewModel() { name = "GUEST" },
            //    new TagAreaTypeViewModel() { name = "BOH" }
            //};

            UnitOfWork unitOfWork = new UnitOfWork();

            return unitOfWork.TagAreaTypeRepository
                .GetBy(i => !i.isDeleted)
                .Select(obj => new TagAreaTypeViewModel
                {
                    tagAreaTypeId = obj.tagAreaTypeId,
                    name = obj.name
                })
                .OrderBy(i => i.name)
                .ToList();

        }

        [HttpPost]
        public HttpResponseMessage GetByPage([FromBody] Search data)
        {
            //Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            //if (a.isAuthenticated)
            //{

                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _GetByPage(data));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            //}
            //return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public class GetByPageReturnViewModel
        {
            public int totalRecords { get; set; }
            public double totalPages { get; set; }
            public List<TagAreaTypeViewModel> tagAreaTypes { get; set; }
        }

        public static GetByPageReturnViewModel _GetByPage(Search data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            var query = unitOfWork.TagAreaTypeRepository
                .GetBy(i => !i.isDeleted
                    && i.name.Contains(data.search));

            int currentPage = data.page - 1;
            int skip = currentPage * data.records;
            int totalRecords = query.ToList().Count;
            var arr = query
                .Select(obj => new TagAreaTypeViewModel
                {
                    tagAreaTypeId = obj.tagAreaTypeId,
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
                tagAreaTypes = arr
            };

        }

        [HttpPost]
        public HttpResponseMessage GetById([FromBody] Search data)
        {
            //Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            //if (a.isAuthenticated)
            //{

                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _GetById(data));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            //}
            //return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public static TagAreaTypeViewModel _GetById(Search data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            return unitOfWork.TagAreaTypeRepository
                .GetBy(i => i.tagAreaTypeId == data.id
                    && !i.isDeleted)
                .Select(obj => new TagAreaTypeViewModel
                {
                    tagAreaTypeId = obj.tagAreaTypeId,
                    name = obj.name,
                    isDeleted = obj.isDeleted
                })
                .FirstOrDefault();
            
        }

    }
}
