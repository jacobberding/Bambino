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
            
            BambinoDataContext context = new BambinoDataContext();
            Authentication authentication = new Authentication();
            
            var api = context.ApiTokens
                .Where(i => i.apiTokenId == apiId
                    && i.accessLevel >= accessLevel
                    && !i.isDeleted)
                .Select(obj => new
                {
                    apiId = obj.apiTokenId
                })
                .FirstOrDefault();

            var member = context.Members
                .Where(i => (i.token == token || (i.tokenApi == token && accessLevel == 0))
                    && !i.isDeleted)
                .Select(obj => new MemberViewModel()
                {
                    memberId            = obj.memberId,
                    activeCompanyId     = obj.activeCompanyId,
                    roles               = obj.MemberRoles.Select(memberRole => new RoleViewModel()
                    {
                        isContractor        = memberRole.Role.isContractor,
                        isEmployee          = memberRole.Role.isEmployee,
                        isManager           = memberRole.Role.isManager,
                        isAdmin             = memberRole.Role.isAdmin,
                        isSuperAdmin        = memberRole.Role.isSuperAdmin,
                    }).ToList()
                })
                .FirstOrDefault();
            
            authentication.isAuthenticated = (api != null && member != null) ? true : false;
            authentication.member = (member != null) ? new MemberViewModel() { memberId = member.memberId, activeCompanyId = member.activeCompanyId, roles = member.roles } : new MemberViewModel();
            
            return authentication;
                
        }
        
        public static Authentication GetApiAuthenticated(Guid apiId, int accessLevel)
        {

            BambinoDataContext context = new BambinoDataContext();
            Authentication authentication = new Authentication();
            
            var api = context.ApiTokens
                .Where(i => i.apiTokenId == apiId
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
