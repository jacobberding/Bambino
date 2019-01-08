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

                    UnitOfWork unitOfWork = new UnitOfWork();

                    Material material = (data.materialId == Guid.Empty) ? new Material() : unitOfWork.MaterialRepository
                        .GetBy(i => i.materialId == data.materialId
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (material == null)
                        throw new InvalidOperationException("Layer Not Found");

                    material.disciplineId = data.disciplineId;
                    material.name = data.name;
                    material.description = data.description;
                    material.website = data.website;
                    material.priceMin = data.priceMin;
                    material.priceMax = data.priceMax;
                    material.materialPriceOptionKey = data.materialPriceOptionKey;
                    material.manufacturer = data.manufacturer;
                    material.modelNumber = data.modelNumber;
                    material.tags = data.tags;
                    material.notes = data.notes;
                    material.isDeleted = data.isDeleted;

                    if (data.materialId == Guid.Empty)
                        unitOfWork.MaterialRepository.Insert(material);
                    else
                        unitOfWork.MaterialRepository.Update(material);

                    unitOfWork.Save();

                    var activity = (data.materialId == Guid.Empty) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited";
                    LogController.Add(a.member.memberId, String.Format("Material {0} was {1}", material.name, activity), "Material", "AddEditDelete", material.materialId, "Materials");

                    var vm = new AddEditDeleteReturnViewModel()
                    {
                        id = material.materialId,
                        state = (data.materialId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
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

                    UnitOfWork unitOfWork = new UnitOfWork();
                    
                    var query = unitOfWork.MaterialRepository
                        .GetBy(i => (i.name.Contains(data.search)
                            || i.manufacturer.Contains(data.search)
                            || i.modelNumber.Contains(data.search)
                            || i.tags.Contains(data.search))
                            && !i.isDeleted);
                    
                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = query.ToList().Count;
                    var arr = query
                        .Select(obj => new MaterialViewModel
                        {
                            materialId = obj.materialId,
                            disciplineId = obj.disciplineId,
                            discipline = new DisciplineViewModel()
                            {
                                disciplineId = obj.discipline.disciplineId,
                                description = obj.discipline.description,
                                name = obj.discipline.name,
                                value = obj.discipline.value
                            },
                            name = obj.name,
                            description = obj.description,
                            website = obj.website,
                            priceMin = obj.priceMin,
                            priceMax = obj.priceMax,
                            materialPriceOptionKey = obj.materialPriceOptionKey,
                            materialPriceOption = new MaterialPriceOptionViewModel()
                            {
                                materialPriceOptionKey = obj.materialPriceOption.materialPriceOptionKey,
                                abbreviation = obj.materialPriceOption.abbreviation,
                                description = obj.materialPriceOption.description,
                                name = obj.materialPriceOption.name
                            },
                            manufacturer = obj.manufacturer,
                            modelNumber = obj.modelNumber,
                            tags = obj.tags,
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

                    UnitOfWork unitOfWork = new UnitOfWork();

                    var vm = unitOfWork.MaterialRepository
                        .GetBy(i => i.materialId == data.id
                            && !i.isDeleted)
                        .Select(obj => new MaterialViewModel
                        {
                            materialId = obj.materialId,
                            disciplineId = obj.disciplineId,
                            discipline = new DisciplineViewModel()
                            {
                                disciplineId = obj.discipline.disciplineId,
                                description = obj.discipline.description,
                                name = obj.discipline.name,
                                value = obj.discipline.value
                            },
                            name = obj.name,
                            description = obj.description,
                            website = obj.website,
                            priceMin = obj.priceMin,
                            priceMax = obj.priceMax,
                            materialPriceOptionKey = obj.materialPriceOptionKey,
                            materialPriceOption = new MaterialPriceOptionViewModel()
                            {
                                materialPriceOptionKey = obj.materialPriceOption.materialPriceOptionKey,
                                abbreviation = obj.materialPriceOption.abbreviation,
                                description = obj.materialPriceOption.description,
                                name = obj.materialPriceOption.name
                            },
                            manufacturer = obj.manufacturer,
                            modelNumber = obj.modelNumber,
                            tags = obj.tags,
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
