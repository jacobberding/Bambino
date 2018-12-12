using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class LayerController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] LayerAddEditDeleteViewModel data)
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

        public static AddEditDeleteReturnViewModel _AddEditDelete(LayerAddEditDeleteViewModel data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            Layer layer = (data.layerId == Guid.Empty) ? new Layer() : unitOfWork.LayerRepository
                .GetBy(i => i.layerId == data.layerId
                    && !i.isDeleted)
                .FirstOrDefault();

            if (layer == null)
                throw new InvalidOperationException("Layer Not Found");

            string value = unitOfWork.CategoryRepository
                .GetBy(i => i.categoryId == data.categoryId
                    && !i.isDeleted)
                .Select(i => i.value)
                .FirstOrDefault();

            if (value == null)
                throw new InvalidOperationException("Category Not Found");

            string m = (data.measurement == "English") ? "Imperical" : data.measurement;

            layer.categoryId = data.categoryId;
            layer.name = "EDC-" + m.Substring(0,1) + "-" + value + "-" + data.code;
            layer.color = data.color;
            layer.lineWeight = data.lineWeight;
            layer.transparency = data.transparency;
            layer.measurement = m;
            layer.code = data.code;
            layer.keywords = data.keywords;
            layer.description = data.description;
            layer.isPlottable = data.isPlottable;
            layer.isDeleted = data.isDeleted;

            if (data.layerId == Guid.Empty)
                unitOfWork.LayerRepository.Insert(layer);
            else
                unitOfWork.LayerRepository.Update(layer);

            unitOfWork.Save();

            var activity = (data.layerId == Guid.Empty) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
            //LogController.Add(a.member.memberId, "Template Category " + category.name + " " + activity, "Category", "AddEditDelete", category.categoryId, "Categories");

            return new AddEditDeleteReturnViewModel()
            {
                id = layer.layerId,
                state = (data.layerId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
            };

        }

        public static void _AddMany(List<LayerAddManyViewModel> data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            foreach (var l in data)
            {

                Layer layer = new Layer();
                
                Guid categoryId = unitOfWork.CategoryRepository
                    .GetBy(i => i.value == l.categoryValue
                        && !i.isDeleted)
                    .Select(i => i.categoryId)
                    .FirstOrDefault();

                if (categoryId == null)
                    throw new InvalidOperationException("Category Not Found");

                string m = (l.measurement == "English") ? "Imperical" : l.measurement;

                layer.categoryId = categoryId;
                layer.name = "EDC-" + m.Substring(0, 1) + "-" + l.categoryValue + "-" + l.code;
                layer.color = l.color;
                layer.lineWeight = l.lineWeight;
                layer.transparency = l.transparency;
                layer.measurement = m;
                layer.code = l.code;
                layer.description = l.description;
                layer.isPlottable = l.isPlottable;
                
                unitOfWork.LayerRepository.Insert(layer);
                unitOfWork.Save();

            }

        }

        [HttpPost]
        public HttpResponseMessage GetByKeyword([FromBody] LayerGetByKeywordViewModel data)
        {
            //Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            //if (a.isAuthenticated)
            //{

            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _GetByKeyword(new LayerGetByKeywordViewModel() { keyword = data.keyword, measurement = "Imperical" }));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

            //}
            //return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }

        public static List<LayerViewModel> _GetByKeyword(LayerGetByKeywordViewModel data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            string m = (data.measurement == "English") ? "Imperical" : data.measurement;

            return unitOfWork.LayerRepository
                .GetBy(i => i.keywords.Contains(data.keyword)
                    && i.measurement == m
                    && !i.isDeleted)
                .Select(obj => new LayerViewModel()
                {
                    layerId = obj.layerId,
                    categoryId = obj.categoryId,
                    category = new CategoryViewModel()
                    {
                        categoryId = obj.category.categoryId,
                        name = obj.category.name,
                        value = obj.category.value,
                        description = obj.category.description
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

        public static List<LayerViewModel> _GetByCategory(LayerGetByCategoryViewModel data)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            string m = (data.measurement == "English") ? "Imperical" : data.measurement;

            var query = (data.category == "All") ? unitOfWork.LayerRepository.GetBy(i => i.measurement == m && !i.isDeleted) : 
                unitOfWork.LayerRepository.GetBy(i => i.category.name == data.category && i.measurement == m && !i.isDeleted);

            return query
                .Select(obj => new LayerViewModel()
                {
                    layerId = obj.layerId,
                    categoryId = obj.categoryId,
                    category = new CategoryViewModel()
                    {
                        categoryId = obj.category.categoryId,
                        name = obj.category.name,
                        value = obj.category.value,
                        description = obj.category.description
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

        public static List<LayerViewModel> _GetAreaTag(string measurement)
        {

            UnitOfWork unitOfWork = new UnitOfWork();

            var query = unitOfWork.LayerRepository
                .GetBy(i => (i.name == "EDC-I-G-AREA-NOTE"
                    || i.name == "EDC-I-G-NOTE"
                    || i.name == "EDC-I-G-AREA-GROS-NPLT"
                    || i.name == "EDC-I-G-AREA-NET1-NPLT")
                    && i.measurement == measurement
                    && !i.isDeleted);

            return query
                .Select(obj => new LayerViewModel()
                {
                    layerId = obj.layerId,
                    categoryId = obj.categoryId,
                    category = new CategoryViewModel()
                    {
                        categoryId = obj.category.categoryId,
                        name = obj.category.name,
                        value = obj.category.value,
                        description = obj.category.description
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
            
            var query = unitOfWork.LayerRepository
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
