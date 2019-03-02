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
    public class MaterialController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] MaterialAddEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Material material = (data.materialKey == 0) ? new Material() : context.Materials
                        .Where(i => i.materialKey == data.materialKey
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (material == null)
                        throw new InvalidOperationException("Layer Not Found");

                    material.disciplineKey = data.disciplineKey;
                    material.name = data.name;
                    material.description = data.description;
                    material.website = data.website;
                    material.priceMin = data.priceMin;
                    material.priceMax = data.priceMax;
                    material.materialPriceOptionKey = data.materialPriceOptionKey;
                    material.manufacturer = data.manufacturer;
                    material.modelNumber = data.modelNumber;
                    material.notes = data.notes;
                    material.tags = "";
                    material.isDeleted = data.isDeleted;

                    if (data.materialKey == 0)
                        context.Materials.InsertOnSubmit(material);

                    context.SubmitChanges();

                    var activity = (data.materialKey == 0) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
                    LogController.Add(a.member.memberId, String.Format("Material {0} was {1}", material.name, activity), "Material", "AddEditDelete", Guid.Empty, "Materials", material.materialKey);

                    var vm = new 
                    {
                        material.materialKey,
                        state = (data.materialKey == 0) ? "add" : (data.isDeleted) ? "delete" : "edit"
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
        public HttpResponseMessage AddTag([FromBody] AddDeleteManyToManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();
                    
                    MaterialTagMaterial materialTagMaterial = new MaterialTagMaterial();

                    materialTagMaterial.materialKey = data.tableKey;
                    materialTagMaterial.materialTagKey = context.MaterialTags.Where(i => i.name == data.name).FirstOrDefault().materialTagKey;

                    context.MaterialTagMaterials.InsertOnSubmit(materialTagMaterial);
                    
                    context.SubmitChanges();
                    
                    var vm = new 
                    {
                        materialTagMaterial.materialTagKey,
                        data.name
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
        public HttpResponseMessage DeleteTag([FromBody] AddDeleteManyToManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();
                    
                    MaterialTagMaterial materialTagMaterial = context.MaterialTagMaterials
                        .Where(i => i.materialKey == data.tableKey
                            && i.materialTagKey == data.manyKey)
                        .FirstOrDefault();
                    
                    context.MaterialTagMaterials.DeleteOnSubmit(materialTagMaterial);

                    context.SubmitChanges();

                    var vm = new 
                    {
                        materialTagMaterial.materialKey,
                        materialTagMaterial.materialTagKey
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
        public HttpResponseMessage GetByPage([FromBody] SearchViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();
                    
                    Expression<Func<Material, bool>> query = i => (i.name.Contains(data.search)
                            || i.manufacturer.Contains(data.search)
                            || i.modelNumber.Contains(data.search)
                            || i.MaterialTagMaterials.Any(materialTagMaterial => materialTagMaterial.MaterialTag.name.Contains(data.search)))
                            && !i.isDeleted;
                    
                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = context.Materials.Where(query).ToList().Count;
                    var arr = context.Materials
                        .Where(query)
                        .Select(obj => new MaterialViewModel
                        {
                            materialKey = obj.materialKey,
                            disciplineKey = obj.disciplineKey,
                            discipline = new DisciplineViewModel()
                            {
                                disciplineKey = obj.Discipline.disciplineKey,
                                description = obj.Discipline.description,
                                name = obj.Discipline.name,
                                value = obj.Discipline.value
                            },
                            name = obj.name,
                            description = obj.description,
                            website = obj.website,
                            priceMin = obj.priceMin,
                            priceMax = obj.priceMax,
                            materialPriceOptionKey = obj.materialPriceOptionKey,
                            materialPriceOption = new MaterialPriceOptionViewModel()
                            {
                                materialPriceOptionKey = obj.MaterialPriceOption.materialPriceOptionKey,
                                abbreviation = obj.MaterialPriceOption.abbreviation,
                                description = obj.MaterialPriceOption.description,
                                name = obj.MaterialPriceOption.name
                            },
                            manufacturer = obj.manufacturer,
                            modelNumber = obj.modelNumber,
                            notes = obj.notes
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

                    var vm = context.Materials
                        .Where(i => i.materialKey == data.key
                            && !i.isDeleted)
                        .Select(obj => new MaterialViewModel
                        {
                            materialKey = obj.materialKey,
                            disciplineKey = obj.disciplineKey,
                            discipline = new DisciplineViewModel()
                            {
                                disciplineKey = obj.Discipline.disciplineKey,
                                description = obj.Discipline.description,
                                name = obj.Discipline.name,
                                value = obj.Discipline.value
                            },
                            name = obj.name,
                            description = obj.description,
                            website = obj.website,
                            priceMin = obj.priceMin,
                            priceMax = obj.priceMax,
                            materialPriceOptionKey = obj.materialPriceOptionKey,
                            materialPriceOption = new MaterialPriceOptionViewModel()
                            {
                                materialPriceOptionKey = obj.MaterialPriceOption.materialPriceOptionKey,
                                abbreviation = obj.MaterialPriceOption.abbreviation,
                                description = obj.MaterialPriceOption.description,
                                name = obj.MaterialPriceOption.name
                            },
                            manufacturer = obj.manufacturer,
                            modelNumber = obj.modelNumber,
                            materialTags = obj.MaterialTagMaterials.Select(materialTagMaterial => new MaterialTagViewModel()
                            {
                                materialTagKey = materialTagMaterial.materialTagKey,
                                name = materialTagMaterial.MaterialTag.name
                            })
                            .OrderBy(i => i.name)
                            .ToList(),
                            notes = obj.notes
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
