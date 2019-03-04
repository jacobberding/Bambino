using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
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

            BambinoDataContext context = new BambinoDataContext();

            ACLayer acLayer = (data.acLayerId == Guid.Empty) ? new ACLayer() : context.ACLayers
                .Where(i => i.acLayerId == data.acLayerId
                    && !i.isDeleted)
                .FirstOrDefault();

            if (acLayer == null)
                throw new InvalidOperationException("Layer Not Found");

            string value = context.ACLayerCategories
                .Where(i => i.acLayerCategoryId == data.acLayerCategoryId
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
            acLayer.lineType = data.lineType;
            acLayer.transparency = data.transparency;
            acLayer.measurement = m;
            acLayer.code = data.code;
            acLayer.keywords = data.keywords;
            acLayer.description = data.description;
            acLayer.isPlottable = data.isPlottable;
            acLayer.isDeleted = data.isDeleted;

            if (data.acLayerId == Guid.Empty)
                context.ACLayers.InsertOnSubmit(acLayer);

            context.SubmitChanges();

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

            BambinoDataContext context = new BambinoDataContext();

            foreach (var l in data)
            {

                ACLayer acLayer = new ACLayer();
                
                Guid acLayerCategoryId = context.ACLayerCategories
                    .Where(i => i.value == l.categoryValue
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
                
                context.ACLayers.InsertOnSubmit(acLayer);

            }

            context.SubmitChanges();

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

                    Expression<Func<ACLayer, bool>> query = i => (i.name.Contains(data.search)
                            || i.measurement.Contains(data.search)
                            || i.keywords.Contains(data.search))
                            && !i.isDeleted;

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = context.ACLayers.Where(query).ToList().Count;
                    var arr = context.ACLayers
                        .Where(query)
                        .Select(obj => new
                        {
                            obj.acLayerId,
                            obj.code,
                            obj.name,
                            obj.color,
                            obj.description,
                            obj.keywords,
                            obj.isPlottable,
                            obj.lineWeight,
                            obj.lineType,
                            obj.measurement,
                            obj.transparency,
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

                    var vm = context.ACLayers
                        .Where(i => i.acLayerId == data.id
                            && !i.isDeleted)
                        .Select(obj => new 
                        {
                            obj.acLayerId,
                            obj.acLayerCategoryId,
                            acLayerCategory = new
                            {
                                obj.ACLayerCategory.acLayerCategoryId,
                                obj.ACLayerCategory.name,
                                obj.ACLayerCategory.value,
                                obj.ACLayerCategory.description
                            },
                            obj.code,
                            obj.name,
                            obj.color,
                            obj.description,
                            obj.keywords,
                            obj.isPlottable,
                            obj.lineWeight,
                            obj.lineType,
                            obj.measurement,
                            obj.transparency,
                            obj.isDeleted
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
        public HttpResponseMessage _GetByKeyword([FromBody] ACLayerGetByKeywordViewModel data)
        {
            //Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            //if (a.isAuthenticated)
            //{

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    string m = (data.measurement == "English") ? "Imperical" : data.measurement;

                    var arr = context.ACLayers
                        .Where(i => i.keywords.Contains(data.keyword)
                            && i.measurement == m
                            && !i.isDeleted)
                        .Select(obj => new ACLayerViewModel()
                        {
                            acLayerId = obj.acLayerId,
                            acLayerCategoryId = obj.acLayerCategoryId,
                            acLayerCategory = new ACLayerCategoryViewModel()
                            {
                                acLayerCategoryId = obj.ACLayerCategory.acLayerCategoryId,
                                name = obj.ACLayerCategory.name,
                                value = obj.ACLayerCategory.value,
                                description = obj.ACLayerCategory.description
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
        public HttpResponseMessage _GetByCategory([FromBody] ACLayerGetByCategoryViewModel data)
        {

            try
            {

                BambinoDataContext context = new BambinoDataContext();

                string m = (data.measurement == "English") ? "Imperical" : data.measurement;

                Expression<Func<ACLayer, bool>> query = i => i.ACLayerCategory.name == data.category && i.measurement == m && !i.isDeleted;

                if (data.category == "All")
                    query = i => i.measurement == m && !i.isDeleted;

                var arr = context.ACLayers
                    .Where(query)
                    .Select(obj => new ACLayerViewModel()
                    {
                        acLayerId = obj.acLayerId,
                        acLayerCategoryId = obj.acLayerCategoryId,
                        acLayerCategory = new ACLayerCategoryViewModel()
                        {
                            acLayerCategoryId = obj.ACLayerCategory.acLayerCategoryId,
                            name = obj.ACLayerCategory.name,
                            value = obj.ACLayerCategory.value,
                            description = obj.ACLayerCategory.description
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

                return Request.CreateResponse(HttpStatusCode.OK, arr);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage _GetAreaTag([FromBody] ACLayerGetByCategoryViewModel data)
        {

            try
            {

                BambinoDataContext context = new BambinoDataContext();

                string initial = data.measurement.Substring(0, 1);
                Expression<Func<ACLayer, bool>> query = i => (i.name == "EDC-" + initial + "-G-AREA-NOTE"
                        || i.name == "EDC-" + initial + "-G-NOTE"
                        || i.name == "EDC-" + initial + "-G-AREA-GROS-NPLT"
                        || i.name == "EDC-" + initial + "-G-AREA-NET1-NPLT")
                        && i.measurement == data.measurement
                        && !i.isDeleted;

                var arr = context.ACLayers
                    .Where(query)
                    .Select(obj => new ACLayerViewModel()
                    {
                        acLayerId = obj.acLayerId,
                        acLayerCategoryId = obj.acLayerCategoryId,
                        acLayerCategory = new ACLayerCategoryViewModel()
                        {
                            acLayerCategoryId = obj.ACLayerCategory.acLayerCategoryId,
                            name = obj.ACLayerCategory.name,
                            value = obj.ACLayerCategory.value,
                            description = obj.ACLayerCategory.description
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

                return Request.CreateResponse(HttpStatusCode.OK, arr);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage _GetByArea(string measurement)
        {

            try
            {

                BambinoDataContext context = new BambinoDataContext();
            
                Expression<Func<ACLayer, bool>> query = i => i.name.Contains("AREA")
                        && i.name.Contains("NPLT")
                        && i.measurement == measurement
                        && !i.isDeleted;

                var arr = context.ACLayers
                    .Where(query)
                    .Select(obj => obj.name)
                    .OrderBy(i => i)
                    .ToArray();

                return Request.CreateResponse(HttpStatusCode.OK, arr);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

    }
}
