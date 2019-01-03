using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class ACLayerController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] ACLayerAddEditDeleteViewModel data)
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

        public static AddEditDeleteReturnViewModel _AddEditDelete(ACLayerAddEditDeleteViewModel data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            ACLayer acLayer = (data.acLayerId == Guid.Empty) ? new ACLayer() : unitOfWork.ACLayerRepository
                .GetBy(i => i.acLayerId == data.acLayerId
                    && !i.isDeleted)
                .FirstOrDefault();

            if (acLayer == null)
                throw new InvalidOperationException("Layer Not Found");

            string value = unitOfWork.ACLayerCategoryRepository
                .GetBy(i => i.acLayerCategoryId == data.acLayerCategoryId
                    && !i.isDeleted)
                .Select(i => i.value)
                .FirstOrDefault();

            if (value == null)
                throw new InvalidOperationException("Category Not Found");

            string m = (data.measurement == "English") ? "Imperical" : data.measurement;

            acLayer.acLayerCategoryId = data.acLayerCategoryId;
            acLayer.name = "EDC-" + m.Substring(0,1) + "-" + value + "-" + data.code;
            acLayer.color = data.color;
            acLayer.lineWeight = data.lineWeight;
            acLayer.transparency = data.transparency;
            acLayer.measurement = m;
            acLayer.code = data.code;
            acLayer.keywords = data.keywords;
            acLayer.description = data.description;
            acLayer.isPlottable = data.isPlottable;
            acLayer.isDeleted = data.isDeleted;

            if (data.acLayerId == Guid.Empty)
                unitOfWork.ACLayerRepository.Insert(acLayer);
            else
                unitOfWork.ACLayerRepository.Update(acLayer);

            unitOfWork.Save();

            var activity = (data.acLayerId == Guid.Empty) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
            //LogController.Add(a.member.memberId, "Template Category " + category.name + " " + activity, "Category", "AddEditDelete", category.acLayerCategoryId, "Categories");

            return new AddEditDeleteReturnViewModel()
            {
                id = acLayer.acLayerId,
                state = (data.acLayerId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
            };

        }

        public static void _AddMany(List<ACLayerAddManyViewModel> data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            foreach (var l in data)
            {

                ACLayer acLayer = new ACLayer();
                
                Guid acLayerCategoryId = unitOfWork.ACLayerCategoryRepository
                    .GetBy(i => i.value == l.categoryValue
                        && !i.isDeleted)
                    .Select(i => i.acLayerCategoryId)
                    .FirstOrDefault();

                if (acLayerCategoryId == null)
                    throw new InvalidOperationException("Category Not Found");

                string m = (l.measurement == "English") ? "Imperical" : l.measurement;

                acLayer.acLayerCategoryId = acLayerCategoryId;
                acLayer.name = "EDC-" + m.Substring(0, 1) + "-" + l.categoryValue + "-" + l.code;
                acLayer.color = l.color;
                acLayer.lineWeight = l.lineWeight;
                acLayer.transparency = l.transparency;
                acLayer.measurement = m;
                acLayer.code = l.code;
                acLayer.description = l.description;
                acLayer.isPlottable = l.isPlottable;
                
                unitOfWork.ACLayerRepository.Insert(acLayer);
                unitOfWork.Save();

            }

        }

        [HttpPost]
        public HttpResponseMessage GetByKeyword([FromBody] ACLayerGetByKeywordViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _GetByKeyword(new ACLayerGetByKeywordViewModel() { keyword = data.keyword, measurement = "Imperical" }));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public static List<ACLayerViewModel> _GetByKeyword(ACLayerGetByKeywordViewModel data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            string m = (data.measurement == "English") ? "Imperical" : data.measurement;

            return unitOfWork.ACLayerRepository
                .GetBy(i => i.keywords.Contains(data.keyword)
                    && i.measurement == m
                    && !i.isDeleted)
                .Select(obj => new ACLayerViewModel()
                {
                    acLayerId = obj.acLayerId,
                    acLayerCategoryId = obj.acLayerCategoryId,
                    acLayerCategory = new ACLayerCategoryViewModel()
                    {
                        acLayerCategoryId = obj.acLayerCategory.acLayerCategoryId,
                        name = obj.acLayerCategory.name,
                        value = obj.acLayerCategory.value,
                        description = obj.acLayerCategory.description
                    },
                    code = obj.code,
                    name = obj.name,
                    color = obj.color,
                    description = obj.description,
                    keywords = obj.keywords,
                    isPlottable = obj.isPlottable,
                    lineWeight = obj.lineWeight,
                    lineType = obj.lineType,
                    measurement = obj.measurement,
                    transparency = obj.transparency,
                    isDeleted = obj.isDeleted
                })
                .OrderBy(i => i.name)
                .ToList();

        }

        public static List<ACLayerViewModel> _GetByCategory(ACLayerGetByCategoryViewModel data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            string m = (data.measurement == "English") ? "Imperical" : data.measurement;

            var query = (data.category == "All") ? unitOfWork.ACLayerRepository.GetBy(i => i.measurement == m && !i.isDeleted) : 
                unitOfWork.ACLayerRepository.GetBy(i => i.acLayerCategory.name == data.category && i.measurement == m && !i.isDeleted);

            return query
                .Select(obj => new ACLayerViewModel()
                {
                    acLayerId = obj.acLayerId,
                    acLayerCategoryId = obj.acLayerCategoryId,
                    acLayerCategory = new ACLayerCategoryViewModel()
                    {
                        acLayerCategoryId = obj.acLayerCategory.acLayerCategoryId,
                        name = obj.acLayerCategory.name,
                        value = obj.acLayerCategory.value,
                        description = obj.acLayerCategory.description
                    },
                    code = obj.code,
                    name = obj.name,
                    color = obj.color,
                    description = obj.description,
                    keywords = obj.keywords,
                    isPlottable = obj.isPlottable,
                    lineWeight = obj.lineWeight,
                    lineType = obj.lineType,
                    measurement = obj.measurement,
                    transparency = obj.transparency,
                    isDeleted = obj.isDeleted
                })
                .OrderBy(i => i.name)
                .ToList();

        }

        public static List<ACLayerViewModel> _GetAreaTag(string measurement)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            string initial = measurement.Substring(0, 1);
            var query = unitOfWork.ACLayerRepository
                .GetBy(i => (i.name == "EDC-" + initial + "-G-AREA-NOTE"
                    || i.name == "EDC-" + initial + "-G-NOTE"
                    || i.name == "EDC-" + initial + "-G-AREA-GROS-NPLT"
                    || i.name == "EDC-" + initial + "-G-AREA-NET1-NPLT")
                    && i.measurement == measurement
                    && !i.isDeleted);

            return query
                .Select(obj => new ACLayerViewModel()
                {
                    acLayerId = obj.acLayerId,
                    acLayerCategoryId = obj.acLayerCategoryId,
                    acLayerCategory = new ACLayerCategoryViewModel()
                    {
                        acLayerCategoryId = obj.acLayerCategory.acLayerCategoryId,
                        name = obj.acLayerCategory.name,
                        value = obj.acLayerCategory.value,
                        description = obj.acLayerCategory.description
                    },
                    code = obj.code,
                    name = obj.name,
                    color = obj.color,
                    description = obj.description,
                    keywords = obj.keywords,
                    isPlottable = obj.isPlottable,
                    lineWeight = obj.lineWeight,
                    lineType = obj.lineType,
                    measurement = obj.measurement,
                    transparency = obj.transparency,
                    isDeleted = obj.isDeleted
                })
                .OrderBy(i => i.name)
                .ToList();

        }

        public static string[] _GetByArea(string measurement)
        {

            UnitOfWork unitOfWork = new UnitOfWork();
            
            var query = unitOfWork.ACLayerRepository
                .GetBy(i => i.name.Contains("AREA")
                    && i.name.Contains("NPLT")
                    && i.measurement == measurement
                    && !i.isDeleted);

            return query
                .Select(obj => obj.name)
                .OrderBy(i => i)
                .ToArray();

        }

    }
}
