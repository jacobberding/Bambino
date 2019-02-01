using Api.Models;
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
    public class ProjectPhaseController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage AddEditDelete([FromBody] ProjectPhaseAddEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    UnitOfWork unitOfWork = new UnitOfWork();

                    ProjectPhase projectPhase = (data.projectPhaseId == Guid.Empty) ? new ProjectPhase() : unitOfWork.ProjectPhaseRepository
                        .GetBy(i => i.projectPhaseId == data.projectPhaseId
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (projectPhase == null)
                        throw new InvalidOperationException("Not Found");

                    projectPhase.projectId = data.projectId;
                    projectPhase.name = data.name;
                    projectPhase.description = data.description;
                    projectPhase.dateStart = data.dateStart;
                    projectPhase.dateEnd = data.dateEnd;
                    projectPhase.isDeleted = data.isDeleted;

                    if (data.projectPhaseId == Guid.Empty)
                        unitOfWork.ProjectPhaseRepository.Insert(projectPhase);
                    else
                        unitOfWork.ProjectPhaseRepository.Update(projectPhase);

                    unitOfWork.Save();
                    
                    var vm = new AddEditDeleteReturnViewModel()
                    {
                        id = projectPhase.projectPhaseId,
                        state = (data.projectPhaseId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
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
        
        public static void AddDefaults(Guid projectId)
        {

            try
            {

                UnitOfWork unitOfWork = new UnitOfWork();

                ProjectPhase projectPhase = new ProjectPhase();

                projectPhase.projectId = projectId;
                projectPhase.name = "Business Development";
                projectPhase.dateStart = DateTimeOffset.UtcNow;
                projectPhase.dateEnd = DateTimeOffset.UtcNow.AddMonths(1);

                unitOfWork.ProjectPhaseRepository.Insert(projectPhase);
                unitOfWork.Save();
                
                projectPhase.name = "Blue Sky";
                projectPhase.dateStart = DateTimeOffset.UtcNow.AddMonths(1);
                projectPhase.dateEnd = DateTimeOffset.UtcNow.AddMonths(2);

                unitOfWork.ProjectPhaseRepository.Insert(projectPhase);
                unitOfWork.Save();
                
                projectPhase.name = "Concept";
                projectPhase.dateStart = DateTimeOffset.UtcNow.AddMonths(2);
                projectPhase.dateEnd = DateTimeOffset.UtcNow.AddMonths(3);

                unitOfWork.ProjectPhaseRepository.Insert(projectPhase);
                unitOfWork.Save();

                projectPhase.name = "Concept Refinement";
                projectPhase.dateStart = DateTimeOffset.UtcNow.AddMonths(3);
                projectPhase.dateEnd = DateTimeOffset.UtcNow.AddMonths(4);

                unitOfWork.ProjectPhaseRepository.Insert(projectPhase);
                unitOfWork.Save();

                projectPhase.name = "Schematic Design";
                projectPhase.dateStart = DateTimeOffset.UtcNow.AddMonths(4);
                projectPhase.dateEnd = DateTimeOffset.UtcNow.AddMonths(5);

                unitOfWork.ProjectPhaseRepository.Insert(projectPhase);
                unitOfWork.Save();

                projectPhase.name = "Design Development";
                projectPhase.dateStart = DateTimeOffset.UtcNow.AddMonths(5);
                projectPhase.dateEnd = DateTimeOffset.UtcNow.AddMonths(6);

                unitOfWork.ProjectPhaseRepository.Insert(projectPhase);
                unitOfWork.Save();

                projectPhase.name = "Production Design";
                projectPhase.dateStart = DateTimeOffset.UtcNow.AddMonths(6);
                projectPhase.dateEnd = DateTimeOffset.UtcNow.AddMonths(7);

                unitOfWork.ProjectPhaseRepository.Insert(projectPhase);
                unitOfWork.Save();

            }
            catch (Exception ex)
            {

            }
            
        }

        //[HttpPost]
        //public HttpResponseMessage EditSortOrder([FromBody] ProjectPhaseAddEditDeleteViewModel data)
        //{
        //    Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
        //    if (a.isAuthenticated)
        //    {

        //        try
        //        {

        //            UnitOfWork unitOfWork = new UnitOfWork();

        //            ProjectPhase projectPhase = (data.projectPhaseId == Guid.Empty) ? new ProjectPhase() : unitOfWork.ProjectPhaseRepository
        //                .GetBy(i => i.projectPhaseId == data.projectPhaseId
        //                    && !i.isDeleted)
        //                .FirstOrDefault();

        //            if (projectPhase == null)
        //                throw new InvalidOperationException("Not Found");

        //            projectPhase.name = data.name;
        //            projectPhase.description = data.description;
        //            projectPhase.dateStart = data.dateStart;
        //            projectPhase.dateEnd = data.dateEnd;
        //            projectPhase.isDeleted = data.isDeleted;

        //            if (data.projectId == Guid.Empty)
        //                unitOfWork.ProjectPhaseRepository.Insert(projectPhase);
        //            else
        //                unitOfWork.ProjectPhaseRepository.Update(projectPhase);

        //            unitOfWork.Save();

        //            var vm = new AddEditDeleteReturnViewModel()
        //            {
        //                id = projectPhase.projectPhaseId,
        //                state = (data.projectPhaseId == Guid.Empty) ? "add" : (data.isDeleted) ? "delete" : "edit"
        //            };

        //            return Request.CreateResponse(HttpStatusCode.OK, vm);

        //        }
        //        catch (Exception ex)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
        //        }

        //    }
        //    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        //}

        [HttpPost]
        public HttpResponseMessage GetByPage([FromBody] SearchViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    UnitOfWork unitOfWork = new UnitOfWork();

                    var query = unitOfWork.ProjectPhaseRepository
                        .GetBy(i => i.projectId == data.id
                            && !i.isDeleted
                            && i.name.Contains(data.search));

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = query.ToList().Count;
                    var arr = query
                        .Select(obj => new ProjectPhaseViewModel
                        {
                            projectPhaseId = obj.projectPhaseId,
                            projectId = obj.projectId,
                            name = obj.name,
                            description = obj.description,
                            sortOrder = obj.sortOrder,
                            dateStart = obj.dateStart,
                            dateEnd = obj.dateEnd,
                            isDeleted = obj.isDeleted
                        })
                        .OrderBy(i => i.name)
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

                    var vm = unitOfWork.ProjectPhaseRepository
                        .GetBy(i => i.projectPhaseId == data.id
                            && !i.isDeleted)
                        .Select(obj => new ProjectPhaseViewModel
                        {
                            projectPhaseId = obj.projectPhaseId,
                            projectId = obj.projectId,
                            name = obj.name,
                            description = obj.description,
                            sortOrder = obj.sortOrder,
                            dateStart = obj.dateStart,
                            dateEnd = obj.dateEnd,
                            isDeleted = obj.isDeleted
                        })
                        .FirstOrDefault();

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
