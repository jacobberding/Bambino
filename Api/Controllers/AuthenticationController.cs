using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Api.Models;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuthenticationController : ApiController
    {
        
        public static Authentication GetMemberAuthenticated(Guid apiId, int accessLevel, Guid token)
        {
            
            UnitOfWork unitOfWork = new UnitOfWork();
            Authentication authentication = new Authentication();
            
            var api = unitOfWork.ApiTokenRepository
                .GetBy(i => i.apiTokenId == apiId
                    && i.accessLevel >= accessLevel
                    && !i.isDeleted)
                .Select(obj => new
                {
                    apiId = obj.apiTokenId
                })
                .FirstOrDefault();

            var member = unitOfWork.MemberRepository
                .GetBy(i => (i.token == token || (i.tokenApi == token && accessLevel == 0))
                    && !i.isDeleted)
                .Select(obj => new MemberViewModel()
                {
                    memberId    = obj.memberId,
                    companyId   = obj.companyId,
                    roles       = obj.roles.Select(role => new RoleViewModel()
                    {
                        isContractor    = role.isContractor,
                        isEmployee      = role.isEmployee,
                        isManager       = role.isManager,
                        isAdmin         = role.isAdmin,
                        isSuperAdmin    = role.isSuperAdmin,
                    }).ToList()
                })
                .FirstOrDefault();
            
            authentication.isAuthenticated = (api != null && member != null) ? true : false;
            authentication.member = (member != null) ? new MemberViewModel() { memberId = member.memberId, companyId = member.companyId, company = member.company, roles = member.roles } : new MemberViewModel();
            
            return authentication;
                
        }
        
        public static Authentication GetApiAuthenticated(Guid apiId, int accessLevel)
        {
            
            UnitOfWork unitOfWork = new UnitOfWork();
            Authentication authentication = new Authentication();
            
            var api = unitOfWork.ApiTokenRepository
                .GetBy(i => i.apiTokenId == apiId
                    && i.accessLevel >= accessLevel
                    && !i.isDeleted)
                .Select(obj => new
                {
                    apiId = obj.apiTokenId
                })
                .FirstOrDefault();
            
            authentication.isAuthenticated = (api != null) ? true : false;
            authentication.member = new MemberViewModel();
                
            return authentication;
                
        }
        
    }
}
