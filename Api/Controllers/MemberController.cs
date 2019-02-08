using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;
using Api.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using System.Web.Security;
using System.Threading.Tasks;
//using Stripe;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MemberController : ApiController
    {

        //[HttpPost]
        //public HttpResponseMessage AddCard([FromBody] MemberAddCardViewModel data)
        //{
        //    Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
        //    if (a.isAuthenticated)
        //    {

        //        try
        //        {

        //            BambinoDataContext context = new BambinoDataContext();

        //            string stripeId = context.MemberRepository
        //                .Where(i => i.memberId == a.member.memberId
        //                    && !i.isDeleted)
        //                .Select(str => str.stripeId)
        //                .FirstOrDefault();

        //            if (stripeId == null)
        //                throw new InvalidOperationException("Not Found");

        //            Stripe.Card card = StripeHelper.AddCard(new StripeHelper.AddCardViewModel() { customerId = stripeId, token = data.token });

        //            return Request.CreateResponse(HttpStatusCode.OK, card);

        //        }
        //        catch (Exception ex)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
        //        }

        //    }
        //    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        //}

        [HttpPost]
        public HttpResponseMessage AddRole([FromBody] AddDeleteManyToManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Member member = context.Members.Where(i => i.memberId == data.tableId).FirstOrDefault();
                    Role role = context.Roles.Where(i => i.name == data.name).FirstOrDefault();

                    MemberRole memberRole = new MemberRole();

                    memberRole.memberId = member.memberId;
                    memberRole.roleId = role.roleId;

                    context.MemberRoles.InsertOnSubmit(memberRole);
                    context.SubmitChanges();

                    LogController.Add(a.member.memberId, "Member " + member.email + " Added Role " + role.name, "Member", "AddRole", member.memberId, "Members");

                    var vm = new
                    {
                        manyId = role.roleId,
                        name = role.name,
                        isAdmin = role.isAdmin,
                        isContractor = role.isContractor,
                        isEmployee = role.isEmployee,
                        isSuperAdmin = role.isSuperAdmin
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
        public HttpResponseMessage AddCompany([FromBody] AddDeleteManyToManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Member member = context.Members.Where(i => i.memberId == data.tableId && !i.isDeleted).FirstOrDefault();
                    Company company = context.Companies.Where(i => i.name == data.name).FirstOrDefault();

                    MemberCompany memberCompany = new MemberCompany();

                    memberCompany.memberId = member.memberId;
                    memberCompany.companyId = company.companyId;
                    
                    context.MemberCompanies.InsertOnSubmit(memberCompany);
                    context.SubmitChanges();

                    LogController.Add(a.member.memberId, "Member " + member.email + " Added Company " + company.name, "Member", "AddCompany", member.memberId, "Members");

                    var vm = new
                    {
                        manyId = company.companyId,
                        name = company.name
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
        public HttpResponseMessage EditDelete([FromBody] MemberEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    
                    BambinoDataContext context = new BambinoDataContext();

                    Guid id = (data.memberId == Guid.Empty) ? a.member.memberId : data.memberId;
                    Member member = context.Members
                        .Where(i => i.memberId == id
                            && !i.isDeleted)
                        .FirstOrDefault();
                    
                    if (member == null)
                        throw new InvalidOperationException("Not Found");
                    
                    member.firstName = data.firstName;
                    member.lastName = data.lastName;
                    member.email = data.email;
                    member.phone = data.phone;
                    member.isDeleted = data.isDeleted;
                    
                    context.SubmitChanges();
                    
                    var activity = (data.memberId == Guid.Empty) ? "Added" : (data.isDeleted) ? "Deleted" : "Edited"; 
                    LogController.Add(a.member.memberId, "Member " + member.email + " " + activity, "Member", "EditDelete", member.memberId, "Members");
                    
                    var vm = new {
                        memberId = member.memberId,
                        state = (data.isDeleted) ? "delete" : "edit"
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
        public HttpResponseMessage EditPassword([FromBody] MemberEditPasswordViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    AESGCM decrypt = new AESGCM();
                    Member member = context.Members
                        .Where(i => i.memberId == a.member.memberId
                            && !i.isDeleted)
                        .FirstOrDefault();
                    string dbPassword = decrypt.DecryptStringFromBytes(member.password.ToArray(), member.keyValue.ToArray(), member.iVValue.ToArray());

                    if (member == null || !dbPassword.Equals(data.passwordOld)) //Check if old password matches one in DB
                        throw new InvalidOperationException("Current Password is incorrect.");

                    AESGCM encrypt = new AESGCM(data.passwordNew); //encrypt new password

                    member.password = encrypt.password;
                    member.keyValue = encrypt.keyBytes;
                    member.iVValue = encrypt.ivBytes;
                    
                    context.SubmitChanges();
                    
                    LogController.Add(member.memberId, "Member " + member.email + " Edited Password", "Member", "EditPassword", member.memberId, "Members");
                    
                    var vm = new { };

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
        public HttpResponseMessage EditResetPassword([FromBody] MemberEditResetPasswordViewModel data)
        {
            Authentication a = AuthenticationController.GetApiAuthenticated(data.authentication.apiId, 1);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    AESGCM encrypt = new AESGCM(data.password);
                    Member member = context.Members
                        .Where(i => i.email == data.email
                            && i.forgotPasswordToken == data.forgotPasswordToken
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (member == null || member.forgotPasswordDateTime == null)
                        throw new InvalidOperationException("Not Valid");

                    if (DateTimeOffset.UtcNow > member.forgotPasswordDateTime.Value.AddMinutes(30))
                        throw new InvalidOperationException("Token expired. Please request another reset password.");

                    member.password = encrypt.password;
                    member.keyValue = encrypt.keyBytes;
                    member.iVValue = encrypt.ivBytes;
                    member.forgotPasswordToken = null;
                    member.forgotPasswordDateTime = null;
                    
                    context.SubmitChanges();
                    
                    LogController.Add(member.memberId, "Member " + member.email + " Reset Password", "Member", "EditResetPassword", member.memberId, "Members");

                    var vm = new { };

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
        public HttpResponseMessage EditValidated([FromBody] MemberEditValidatedViewModel data)
        {
            Authentication a = AuthenticationController.GetApiAuthenticated(data.authentication.apiId, 1);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Member member = context.Members
                        .Where(i => i.token == data.token
                            && i.keyCode == data.keyCode
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (member == null)
                        throw new InvalidOperationException("Not Valid");

                    if (member.keyCodeDateTime == null)
                        throw new InvalidOperationException("Not Valid");

                    if (DateTimeOffset.UtcNow > member.keyCodeDateTime.Value.AddMinutes(60))
                        throw new InvalidOperationException("Token expired. Please request another key code.");

                    member.isValidated = true;
                    member.keyCode = null;
                    member.keyCodeDateTime = null;
                    
                    context.SubmitChanges();
                    
                    LogController.Add(member.memberId, "Member " + member.email + " Validated Account", "Member", "EditValidated", member.memberId, "Members");

                    var vm = new e()
                    {
                        mT = member.token,
                        mCI = member.activeCompanyId,
                        mIC = member.MemberRoles.Any(i => i.Role.isContractor),
                        mIE = member.MemberRoles.Any(i => i.Role.isEmployee),
                        mIM = member.MemberRoles.Any(i => i.Role.isManager),
                        mIA = member.MemberRoles.Any(i => i.Role.isAdmin),
                        mIS = member.MemberRoles.Any(i => i.Role.isSuperAdmin),
                        mE = member.email,
                        mFN = member.firstName,
                        mLN = member.lastName,
                        mP = member.phone
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
        public HttpResponseMessage EditTokenApi([FromBody] EmptyAuthenticationViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    
                    BambinoDataContext context = new BambinoDataContext();
                    
                    Member member = context.Members
                        .Where(i => i.memberId == a.member.memberId
                            && !i.isDeleted)
                        .FirstOrDefault();
                    
                    if (member == null)
                        throw new InvalidOperationException("Not Found");
                    
                    member.tokenApi = Guid.NewGuid();
                    
                    context.SubmitChanges();
                    
                    LogController.Add(a.member.memberId, "Member " + member.email + " Api Token Reset", "Member", "EditTokenApi", member.memberId, "Members");
                    
                    var vm = new {
                        tokenApi = member.tokenApi
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
        public HttpResponseMessage DeleteRole([FromBody] AddDeleteManyToManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Member member = context.Members.Where(i => i.memberId == data.tableId).FirstOrDefault();
                    Role role = context.Roles.Where(i => i.roleId == data.manyId).FirstOrDefault();

                    MemberRole memberRole = context.MemberRoles.Where(i => i.memberId == data.tableId && i.roleId == data.manyId).FirstOrDefault();
                    
                    context.MemberRoles.DeleteOnSubmit(memberRole);
                    context.SubmitChanges();
                    
                    LogController.Add(a.member.memberId, "Member " + member.email + " Removed Role " + role.name, "Member", "DeleteRole", member.memberId, "Members");

                    var vm = new RoleViewModel
                    {
                        roleId = role.roleId,
                        name = role.name,
                        isAdmin = role.isAdmin,
                        isContractor = role.isContractor,
                        isEmployee = role.isEmployee,
                        isSuperAdmin = role.isSuperAdmin
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
        public HttpResponseMessage DeleteCompany([FromBody] AddDeleteManyToManyViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Member member = context.Members.Where(i => i.memberId == data.tableId && !i.isDeleted).FirstOrDefault();
                    Company company = context.Companies.Where(i => i.companyId == data.manyId).FirstOrDefault();

                    MemberCompany memberCompany = context.MemberCompanies.Where(i => i.memberId == data.tableId && i.companyId == data.manyId).FirstOrDefault();

                    context.MemberCompanies.DeleteOnSubmit(memberCompany);
                    context.SubmitChanges();

                    LogController.Add(a.member.memberId, "Member " + member.email + " Removed Company " + company.name, "Member", "DeleteCompany", member.memberId, "Members");

                    var vm = new CompanyViewModel
                    {
                        companyId = company.companyId,
                        name = company.name
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

        //[HttpPost]
        //public HttpResponseMessage DeleteCard([FromBody] MemberDeleteCardViewModel data)
        //{
        //    Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
        //    if (a.isAuthenticated)
        //    {

        //        try
        //        {

        //            BambinoDataContext context = new BambinoDataContext();

        //            string customerId = context.MemberRepository
        //                .Where(i => i.memberId == a.member.memberId
        //                    && !i.isDeleted)
        //                .Select(str => str.customerId)
        //                .FirstOrDefault();

        //            if (customerId == null)
        //                throw new InvalidOperationException("Not Found");

        //            Stripe.Card card = StripeHelper.DeleteCard(customerId, data.cardId);

        //            return Request.CreateResponse(HttpStatusCode.OK, card);

        //        }
        //        catch (Exception ex)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
        //        }

        //    }
        //    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        //}

        [HttpPost]
        public HttpResponseMessage GetTokenApi([FromBody] EmptyAuthenticationViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    
                    BambinoDataContext context = new BambinoDataContext();
                    
                    Member member = context.Members
                        .Where(i => i.memberId == a.member.memberId
                            && !i.isDeleted)
                        .FirstOrDefault();
                    
                    if (member == null)
                        throw new InvalidOperationException("Not Found");
                    
                    var vm = new {
                        tokenApi = member.tokenApi
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

                    var query = context.Members
                        .Where(i => !i.isDeleted
                            && (i.MemberCompanies.Any(x => x.Company.name.Contains(data.search))
                            || i.email.Contains(data.search) 
                            || String.Concat(i.firstName, " ", i.lastName).Contains(data.search)
                            || i.phone.Contains(data.search)
                            || i.MemberRoles.Select(x => x.Role.name).FirstOrDefault().Contains(data.search)));

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = query.ToList().Count;
                    var arr = query
                        .Select(obj => new MemberViewModel
                        {
                            memberId            = obj.memberId,
                            activeCompanyId     = obj.activeCompanyId,
                            companies           = obj.MemberCompanies.Select(memberCompany => new CompanyViewModel() 
                            {
                                companyId           = memberCompany.companyId,
                                name                = memberCompany.Company.name
                            }).ToList(),
                            firstName           = obj.firstName,
                            lastName            = obj.lastName,
                            email               = obj.email,
                            originalEmail       = obj.originalEmail,
                            phone               = obj.phone,
                            isDeleted           = obj.isDeleted
                        })
                        .OrderBy(i => i.email)
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
                    
                    var vm = context.Members
                        .Where(i => i.memberId == data.id
                            && !i.isDeleted)
                        .Select(obj => new MemberViewModel
                        {
                            memberId            = obj.memberId,
                            activeCompanyId     = obj.activeCompanyId,
                            companies           = obj.MemberCompanies.Select(memberCompany => new CompanyViewModel()
                            {
                                companyId           = memberCompany.companyId,
                                name                = memberCompany.Company.name
                            }).ToList(),
                            firstName           = obj.firstName,
                            lastName            = obj.lastName,
                            email               = obj.email,
                            originalEmail       = obj.originalEmail,
                            phone               = obj.phone,
                            isValidated         = obj.isValidated,
                            isDeleted           = obj.isDeleted,
                            roles           = obj.MemberRoles.Select(memberRole => new RoleViewModel() {
                                roleId          = memberRole.Role.roleId,
                                name            = memberRole.Role.name,
                                isAdmin         = memberRole.Role.isAdmin,
                                isContractor    = memberRole.Role.isContractor,
                                isEmployee      = memberRole.Role.isEmployee,
                                isSuperAdmin    = memberRole.Role.isSuperAdmin
                            }).ToList()
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
        public HttpResponseMessage GetKeyCode([FromBody] MemberGetKeyCodeViewModel data)
        {
            Authentication a = AuthenticationController.GetApiAuthenticated(data.authentication.apiId, 1);
            if (a.isAuthenticated)
            {

                try
                {
                    
                    BambinoDataContext context = new BambinoDataContext();

                    int keyCode = new Random().Next(90000) + 10000;
                    Member member = context.Members
                        .Where(i => i.token == data.token
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (member == null)
                        throw new InvalidOperationException("Not Valid");

                    member.keyCode = keyCode;
                    member.keyCodeDateTime = DateTimeOffset.UtcNow;
                    
                    context.SubmitChanges();
                    
                    LogController.Add(member.memberId, "Member " + member.email + " Requested a New Key Code", "Member", "GetKeyCode", member.memberId, "Members");

                    new Thread(() =>
                    {
                        EmailViewModel e = EmailController.GetEmail(new Guid("c7b08448-9abe-4e3e-b68a-755b968761fe"));
                        EmailController.Send(new MailAddress(EmailController.email),
                            member.email,
                            EmailController.email,
                            EmailController.email,
                            e.subject,
                            EmailController.GetSignUpEmailText(e, member, keyCode));
                    }).Start();

                    var vm = new
                    {
                        validToken = member.memberId
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

        [HttpPost] //Used to validate cookie
        public HttpResponseMessage GetByAuthTokenValidation([FromBody] MemberGetByAuthTokenValidationViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.apiId, 1, data.token);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Member member = context.Members
                        .Where(i => i.memberId == a.member.memberId
                            && i.email == data.email
                            && !i.isDeleted)
                        .FirstOrDefault();

                    //if (member.customerId == "")
                    //{

                    //    Stripe.Customer customer = StripeHelper.AddCustomer(new StripeHelper.AddEditCustomerViewModel() { email = member.email.Trim(), memberId = member.memberId });

                    //    member.customerId = customer.Id;

                    //    context.MemberRepository.Update(member);
                    //    context.SubmitChanges();

                    //}

                    if (member == null || data.email == "" || data.token == Guid.Empty)
                        return Request.CreateResponse(HttpStatusCode.OK, false);
                    else if (member != null && data.isAdmin && member.MemberRoles.Any(i => i.Role.isAdmin))
                        return Request.CreateResponse(HttpStatusCode.OK, true);
                    else if (member != null && data.isAdmin && member.MemberRoles.Any(i => !i.Role.isAdmin))
                        return Request.CreateResponse(HttpStatusCode.OK, false);
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, true);

                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }

            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        }
        
        public string GetIPAddress()
        {
            IPHostEntry Host = default(IPHostEntry);
            string ip = "";
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList) {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                    ip = Convert.ToString(IP);
                }
            }
            return ip;
        }
        
        //[HttpPost]
        //public HttpResponseMessage GetCards([FromBody] GetByIdViewModel data)
        //{
        //    Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
        //    if (a.isAuthenticated)
        //    {

        //        try
        //        {
                    
        //            BambinoDataContext context = new BambinoDataContext();
                    
        //            string customerId = context.MemberRepository
        //                .Where(i => i.memberId == a.member.memberId
        //                    && !i.isDeleted)
        //                .Select(obj => obj.customerId)
        //                .FirstOrDefault();
                    
        //            if (customerId == null)
        //                throw new InvalidOperationException("Not Found");
                    
        //            return Request.CreateResponse(HttpStatusCode.OK, StripeHelper.GetCards(customerId));

        //        }
        //        catch (Exception ex)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
        //        }

        //    }
        //    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = "Invalid Token" });
        //}
        
        [HttpPost]
        public HttpResponseMessage SignIn([FromBody] MemberSignInViewModel data)
        {
            Authentication a = AuthenticationController.GetApiAuthenticated(data.authentication.apiId, 1);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();
                    
                    AESGCM encrypt = new AESGCM(data.password);
                    MemberIpAddress memberIpAddress = new MemberIpAddress();
                    string email = data.email.ToLower();
                    MailAddress address = new MailAddress(email);
                    string emailExtension = address.Host; // host contains yahoo.com

                    Member member = context.Members
                        .Where(i => i.email == email
                            && !i.isDeleted)
                        .FirstOrDefault();

                    var date = DateTimeOffset.UtcNow.AddHours(-1);
                    var memberIpAddresses = context.MemberIpAddresses
                        .Where(i => i.email.ToLower() == data.email.ToLower()
                            && !i.isSuccess
                            && i.createdDate > date)
                        .ToList();

                    if (memberIpAddresses.Count > 6)
                        throw new InvalidOperationException("Too many invalid attempts please try again later.");
                    
                    if (member == null)
                        throw new InvalidOperationException("Not Found");
            
                    memberIpAddress.memberId = member.memberId;
                    memberIpAddress.email = member.email;
                    memberIpAddress.ipAddress = GetIPAddress();
                    memberIpAddress.password = encrypt.password;
                    memberIpAddress.keyValue = encrypt.keyBytes;
                    memberIpAddress.iVValue = encrypt.ivBytes;
                    
                    string dbPassword = encrypt.DecryptStringFromBytes(member.password.ToArray(), member.keyValue.ToArray(), member.iVValue.ToArray());
                    if (dbPassword.Equals(data.password)) //Valid
                    {
                
                        memberIpAddress.isSuccess = true;

                        member.lastLoginDateTime = DateTimeOffset.UtcNow;
                       
                        context.SubmitChanges();
                
                    }
                    else
                    {
                        memberIpAddress.isSuccess = false;
                    }
            
                    context.MemberIpAddresses.InsertOnSubmit(memberIpAddress);
                    context.SubmitChanges();
            
                    if (!dbPassword.Equals(data.password))
                        throw new InvalidOperationException("Invalid Password");
                    
                    var vm = new e()
                    {
                        mT = member.token,
                        mCI = member.activeCompanyId,
                        mIC = member.MemberRoles.Any(i => i.Role.isContractor),
                        mIE = member.MemberRoles.Any(i => i.Role.isEmployee),
                        mIM = member.MemberRoles.Any(i => i.Role.isManager),
                        mIA = member.MemberRoles.Any(i => i.Role.isAdmin),
                        mIS = member.MemberRoles.Any(i => i.Role.isSuperAdmin),
                        mE = member.email,
                        mFN = member.firstName,
                        mLN = member.lastName,
                        mP = member.phone,
                        iV = member.isValidated,
                        v = data.v
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
        public async Task<HttpResponseMessage> SignUp([FromBody] MemberSignUpViewModel data)
        {
            Authentication a = AuthenticationController.GetApiAuthenticated(data.authentication.apiId, 1);
            if (a.isAuthenticated)
            {

                try
                {
                    
                    BambinoDataContext context = new BambinoDataContext();
                    
                    Member member = new Member();
                    AESGCM encrypt = new AESGCM(data.password);
                    string email = data.email.ToLower();
                    int keyCode = new Random().Next(90000) + 10000;
                    Member memberExist = context.Members
                        .Where(i => i.email == email
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (memberExist != null)
                        throw new InvalidOperationException("Email already exists.");

                    Role role = context.Roles
                        .Where(i => i.isContractor)
                        .FirstOrDefault();
                    
                    if (role == null)
                        throw new InvalidOperationException("Role does not exist.");
                    
                    Company company = context.Companies
                        .Where(i => i.companyId == data.companyId)
                        .FirstOrDefault();

                    if (company == null)
                        throw new InvalidOperationException("Company does not exist.");
                    
                    if (company.pin != data.pin)
                        throw new InvalidOperationException("Company pin does not match.");

                    MemberCompany memberCompany = new MemberCompany();

                    memberCompany.companyId = company.companyId;
                    memberCompany.memberId = member.memberId;

                    context.MemberCompanies.InsertOnSubmit(memberCompany);

                    MemberRole memberRole = new MemberRole();

                    memberRole.roleId = role.roleId;
                    memberRole.memberId = member.memberId;

                    context.MemberRoles.InsertOnSubmit(memberRole);

                    member.activeCompanyId = data.companyId;
                    member.email = email;
                    member.originalEmail = email;
                    member.password = encrypt.password;
                    member.keyValue = encrypt.keyBytes;
                    member.iVValue = encrypt.ivBytes;
                    member.token = Guid.NewGuid();
                    member.tokenApi = Guid.NewGuid();
                    member.keyCode = keyCode;
                    member.keyCodeDateTime = DateTimeOffset.UtcNow;
                    
                    context.Members.InsertOnSubmit(member);
                    context.SubmitChanges();

                    LogController.Add(member.memberId, "Member " + member.email + " has signed up", "Member", "SignUp", member.memberId, "Members");

                    new Thread(() =>
                    {
                        EmailViewModel e = EmailController.GetEmail(new Guid("c1acbd9b-cb51-4a4f-8f76-a71789ac4863"));
                        EmailController.Send(new MailAddress(EmailController.email),
                            member.email,
                            EmailController.email,
                            EmailController.email,
                            e.subject,
                            EmailController.GetSignUpEmailText(e, member, keyCode));
                    }).Start();

                    new Thread(() =>
                    {
                        EmailController.Send(new MailAddress(EmailController.email),
                            EmailController.email,
                            EmailController.email,
                            EmailController.email,
                            "Bambino: Member Sign Up",
                            String.Format("Email: {0} / Company: {1}", data.email, company.name));
                    }).Start();

                    var vm = new {
                        token = member.token
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
        public HttpResponseMessage ForgotPassword([FromBody] MemberForgotPasswordViewModel data)
        {
            Authentication a = AuthenticationController.GetApiAuthenticated(data.authentication.apiId, 1);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();

                    Member member = context.Members
                        .Where(i => i.email == data.email.ToLower()
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (member == null)
                        throw new InvalidOperationException("Member does not exist, please sign up to continue.");

                    member.forgotPasswordToken = Guid.NewGuid();
                    member.forgotPasswordDateTime = DateTimeOffset.UtcNow;
                    
                    context.SubmitChanges();
                    
                    LogController.Add(member.memberId, "Member " + member.email + " Requested a Forgot Password", "Member", "ForgotPassword", member.memberId, "Members");

                    new Thread(() =>
                    {
                        EmailViewModel e = EmailController.GetEmail(new Guid("cc8b611a-2576-4ef4-91ac-19969b4cb4b2"));
                        EmailController.Send(new MailAddress(EmailController.email),
                            member.email,
                            EmailController.email,
                            EmailController.email,
                            e.subject,
                            EmailController.GetForgotPasswordEmailText(e, member));
                    }).Start();

                    var vm = new { };

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
        public HttpResponseMessage SendKeyCode([FromBody] GetByIdViewModel data)
        {
            Authentication a = AuthenticationController.GetApiAuthenticated(data.authentication.apiId, 1);
            if (a.isAuthenticated)
            {

                try
                {
                    
                    BambinoDataContext context = new BambinoDataContext();

                    int keyCode = new Random().Next(90000) + 10000;
                    Member member = context.Members
                        .Where(i => i.memberId == data.id
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (member == null)
                        throw new InvalidOperationException("Not Valid");

                    member.keyCode = keyCode;
                    member.keyCodeDateTime = DateTimeOffset.UtcNow;
                    
                    context.SubmitChanges();
                    
                    LogController.Add(member.memberId, "Member " + member.email + " has been sent a Key Code", "Member", "SendKeyCode", member.memberId, "Members");

                    new Thread(() =>
                    {
                        EmailViewModel e = EmailController.GetEmail(new Guid("c1acbd9b-cb51-4a4f-8f76-a71789ac4863"));
                        EmailController.Send(new MailAddress(EmailController.email),
                            member.email,
                            EmailController.email,
                            EmailController.email,
                            e.subject,
                            EmailController.GetSignUpEmailText(e, member, keyCode));
                    }).Start();

                    var vm = new
                    {
                        validToken = member.memberId
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
        public HttpResponseMessage Invite([FromBody] MemberInviteViewModel data)
        {
            Authentication a = AuthenticationController.GetApiAuthenticated(data.authentication.apiId, 1);
            if (a.isAuthenticated)
            {

                try
                {

                    BambinoDataContext context = new BambinoDataContext();
                    
                    new Thread(() =>
                    {
                        EmailController.Send(new MailAddress(EmailController.email),
                            data.email,
                            EmailController.email,
                            EmailController.email,
                            "Bambino: Invitation to Sign Up",
                            "Thank you for your interest in Bambino you can sign up by navigating to <a href='https://desktop.bambino.software/'>desktop.bambino.software</a>");
                    }).Start();

                    var vm = new
                    {

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

    }
}
