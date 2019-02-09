using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

            BambinoDataContext context = new BambinoDataContext();

            string m = (data.measurement == "English") ? "Imperical" : data.measurement;

            return context.ACLayers
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

        }

        public static List<ACLayerViewModel> _GetByCategory(ACLayerGetByCategoryViewModel data)
        {

            BambinoDataContext context = new BambinoDataContext();

            string m = (data.measurement == "English") ? "Imperical" : data.measurement;

            Expression<Func<ACLayer, bool>> query = i => i.ACLayerCategory.name == data.category && i.measurement == m && !i.isDeleted;

            if (data.category == "All")
                query = i => i.measurement == m && !i.isDeleted;

            return context.ACLayers
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

        }

        public static List<ACLayerViewModel> _GetAreaTag(string measurement)
        {

            BambinoDataContext context = new BambinoDataContext();

            string initial = measurement.Substring(0, 1);
            Expression<Func<ACLayer, bool>> query = i => (i.name == "EDC-" + initial + "-G-AREA-NOTE"
                    || i.name == "EDC-" + initial + "-G-NOTE"
                    || i.name == "EDC-" + initial + "-G-AREA-GROS-NPLT"
                    || i.name == "EDC-" + initial + "-G-AREA-NET1-NPLT")
                    && i.measurement == measurement
                    && !i.isDeleted;

            return context.ACLayers
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

        }

        public static string[] _GetByArea(string measurement)
        {

            BambinoDataContext context = new BambinoDataContext();
            
            Expression<Func<ACLayer, bool>> query = i => i.name.Contains("AREA")
                    && i.name.Contains("NPLT")
                    && i.measurement == measurement
                    && !i.isDeleted;

            return context.ACLayers
                .Where(query)
                .Select(obj => obj.name)
                .OrderBy(i => i)
                .ToArray();

        }

    }
}
