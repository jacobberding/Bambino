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
                    
        //            UnitOfWork unitOfWork = new UnitOfWork();
                    
        //            string stripeId = unitOfWork.MemberRepository
        //                .GetBy(i => i.memberId == a.member.memberId
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
        public HttpResponseMessage EditDelete([FromBody] MemberEditDeleteViewModel data)
        {
            Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
            if (a.isAuthenticated)
            {

                try
                {
                    
                    UnitOfWork unitOfWork = new UnitOfWork();

                    Guid id = (data.memberId == Guid.Empty) ? a.member.memberId : data.memberId;
                    Member member = unitOfWork.MemberRepository
                        .GetBy(i => i.memberId == id
                            && !i.isDeleted)
                        .FirstOrDefault();
                    
                    if (member == null)
                        throw new InvalidOperationException("Not Found");
                    
                    member.companyId = data.companyId;
                    member.firstName = data.firstName;
                    member.lastName = data.lastName;
                    member.email = data.email;
                    member.phone = data.phone;
                    member.isDeleted = data.isDeleted;
                    
                    unitOfWork.MemberRepository.Update(member);
                    unitOfWork.Save();
                    
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

                    UnitOfWork unitOfWork = new UnitOfWork();

                    AESGCM decrypt = new AESGCM();
                    Member member = unitOfWork.MemberRepository
                        .GetBy(i => i.memberId == a.member.memberId
                            && !i.isDeleted)
                        .FirstOrDefault();
                    string dbPassword = decrypt.DecryptStringFromBytes(member.password, member.keyValue, member.iVValue);

                    if (member == null || !dbPassword.Equals(data.passwordOld)) //Check if old password matches one in DB
                        throw new InvalidOperationException("Current Password is incorrect.");

                    AESGCM encrypt = new AESGCM(data.passwordNew); //encrypt new password

                    member.password = encrypt.password;
                    member.keyValue = encrypt.keyBytes;
                    member.iVValue = encrypt.ivBytes;

                    unitOfWork.MemberRepository.Update(member);
                    unitOfWork.Save();
                    
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

                    UnitOfWork unitOfWork = new UnitOfWork();

                    AESGCM encrypt = new AESGCM(data.password);
                    Member member = unitOfWork.MemberRepository
                        .GetBy(i => i.email == data.email
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

                    unitOfWork.MemberRepository.Update(member);
                    unitOfWork.Save();
                    
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

                    UnitOfWork unitOfWork = new UnitOfWork();

                    Member member = unitOfWork.MemberRepository
                        .GetBy(i => i.token == data.token
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

                    unitOfWork.MemberRepository.Update(member);
                    unitOfWork.Save();
                    
                    LogController.Add(member.memberId, "Member " + member.email + " Validated Account", "Member", "EditValidated", member.memberId, "Members");

                    var vm = new e()
                    {
                        mT = member.token,
                        mCI = member.companyId,
                        mIC = member.roles.Any(i => i.isContractor),
                        mIE = member.roles.Any(i => i.isEmployee),
                        mIA = member.roles.Any(i => i.isAdmin),
                        mIS = member.roles.Any(i => i.isSuperAdmin),
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
                    
                    UnitOfWork unitOfWork = new UnitOfWork();
                    
                    Member member = unitOfWork.MemberRepository
                        .GetBy(i => i.memberId == a.member.memberId
                            && !i.isDeleted)
                        .FirstOrDefault();
                    
                    if (member == null)
                        throw new InvalidOperationException("Not Found");
                    
                    member.tokenApi = Guid.NewGuid();
                    
                    unitOfWork.MemberRepository.Update(member);
                    unitOfWork.Save();
                    
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
        
        //[HttpPost]
        //public HttpResponseMessage DeleteCard([FromBody] MemberDeleteCardViewModel data)
        //{
        //    Authentication a = AuthenticationController.GetMemberAuthenticated(data.authentication.apiId, 1, data.authentication.token);
        //    if (a.isAuthenticated)
        //    {

        //        try
        //        {
                    
        //            UnitOfWork unitOfWork = new UnitOfWork();
                    
        //            string customerId = unitOfWork.MemberRepository
        //                .GetBy(i => i.memberId == a.member.memberId
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
                    
                    UnitOfWork unitOfWork = new UnitOfWork();
                    
                    Member member = unitOfWork.MemberRepository
                        .GetBy(i => i.memberId == a.member.memberId
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
                    
                    UnitOfWork unitOfWork = new UnitOfWork();

                    var query = unitOfWork.MemberRepository
                        .GetBy(i => !i.isDeleted
                            && (i.company.name.Contains(data.search) 
                            || i.email.Contains(data.search) 
                            || String.Concat(i.firstName, " ", i.lastName).Contains(data.search)
                            || i.phone.Contains(data.search)
                            || i.roles.Select(x => x.name).FirstOrDefault().Contains(data.search)));

                    int currentPage = data.page - 1;
                    int skip = currentPage * data.records;
                    int totalRecords = query.ToList().Count;
                    var arr = query
                        .Select(obj => new MemberViewModel
                        {
                            memberId            = obj.memberId,
                            companyId           = obj.companyId,
                            company             = new CompanyViewModel()
                            {
                                companyId   = obj.company.companyId,
                                name        = obj.company.name
                            },
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
                        arr = arr.Select(obj => new MemberViewModel
                        {
                            memberId            = obj.memberId,
                            companyId           = obj.companyId,
                            company             = new CompanyViewModel()
                            {
                                companyId   = obj.company.companyId,
                                name        = obj.company.name
                            },
                            firstName           = obj.firstName,
                            lastName            = obj.lastName,
                            email               = obj.email,
                            originalEmail       = obj.originalEmail,
                            phone               = obj.phone,
                            isDeleted           = obj.isDeleted
                        }).ToList()
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
                    
                    var vm = unitOfWork.MemberRepository
                        .GetBy(i => i.memberId == data.id
                            && !i.isDeleted)
                        .Select(obj => new MemberViewModel
                        {
                            memberId            = obj.memberId,
                            companyId           = obj.companyId,
                            company             = new CompanyViewModel()
                            {
                                companyId   = obj.company.companyId,
                                name        = obj.company.name
                            },
                            firstName           = obj.firstName,
                            lastName            = obj.lastName,
                            email               = obj.email,
                            originalEmail       = obj.originalEmail,
                            phone               = obj.phone,
                            isValidated         = obj.isValidated,
                            isDeleted           = obj.isDeleted
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
                    
                    UnitOfWork unitOfWork = new UnitOfWork();

                    int keyCode = new Random().Next(90000) + 10000;
                    Member member = unitOfWork.MemberRepository
                        .GetBy(i => i.token == data.token
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (member == null)
                        throw new InvalidOperationException("Not Valid");

                    member.keyCode = keyCode;
                    member.keyCodeDateTime = DateTimeOffset.UtcNow;

                    unitOfWork.MemberRepository.Update(member);
                    unitOfWork.Save();
                    
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

                    UnitOfWork unitOfWork = new UnitOfWork();

                    Member member = unitOfWork.MemberRepository
                        .GetBy(i => i.memberId == a.member.memberId
                            && i.email == data.email
                            && !i.isDeleted)
                        .FirstOrDefault();

                    //if (member.customerId == "")
                    //{

                    //    Stripe.Customer customer = StripeHelper.AddCustomer(new StripeHelper.AddEditCustomerViewModel() { email = member.email.Trim(), memberId = member.memberId });

                    //    member.customerId = customer.Id;

                    //    unitOfWork.MemberRepository.Update(member);
                    //    unitOfWork.Save();

                    //}

                    if (member == null || data.email == "" || data.token == Guid.Empty)
                        return Request.CreateResponse(HttpStatusCode.OK, false);
                    else if (member != null && data.isAdmin && member.roles.Any(i => i.isAdmin))
                        return Request.CreateResponse(HttpStatusCode.OK, true);
                    else if (member != null && data.isAdmin && member.roles.Any(i => !i.isAdmin))
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
                    
        //            UnitOfWork unitOfWork = new UnitOfWork();
                    
        //            string customerId = unitOfWork.MemberRepository
        //                .GetBy(i => i.memberId == a.member.memberId
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

                    UnitOfWork unitOfWork = new UnitOfWork();
                    
                    AESGCM encrypt = new AESGCM(data.password);
                    MemberIpAddress memberIpAddress = new MemberIpAddress();
                    string email = data.email.ToLower();
                    MailAddress address = new MailAddress(email);
                    string emailExtension = address.Host; // host contains yahoo.com

                    Member member = unitOfWork.MemberRepository
                        .GetBy(i => i.email == email
                            && !i.isDeleted)
                        .FirstOrDefault();

                    var date = DateTimeOffset.UtcNow.AddHours(-1);
                    var memberIpAddresses = unitOfWork.MemberIpAddressRepository
                        .GetBy(i => i.email.ToLower() == data.email.ToLower()
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
                    
                    string dbPassword = encrypt.DecryptStringFromBytes(member.password, member.keyValue, member.iVValue);
                    if (dbPassword.Equals(data.password)) //Valid
                    {
                
                        memberIpAddress.isSuccess = true;

                        member.lastLoginDateTime = DateTimeOffset.UtcNow;

                        unitOfWork.MemberRepository.Update(member);
                        unitOfWork.Save();
                
                    }
                    else
                    {
                        memberIpAddress.isSuccess = false;
                    }
            
                    unitOfWork.MemberIpAddressRepository.Insert(memberIpAddress);
                    unitOfWork.Save();
            
                    if (!dbPassword.Equals(data.password))
                        throw new InvalidOperationException("Invalid Password");
                    
                    var vm = new e()
                    {
                        mT = member.token,
                        mCI = member.companyId,
                        mIC = member.roles.Any(i => i.isContractor),
                        mIE = member.roles.Any(i => i.isEmployee),
                        mIA = member.roles.Any(i => i.isAdmin),
                        mIS = member.roles.Any(i => i.isSuperAdmin),
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
                    
                    UnitOfWork unitOfWork = new UnitOfWork();
                    
                    Member member = new Member();
                    AESGCM encrypt = new AESGCM(data.password);
                    string email = data.email.ToLower();
                    Member memberExist = unitOfWork.MemberRepository
                        .GetBy(i => i.email == email
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (memberExist != null)
                        throw new InvalidOperationException("Email already exists.");

                    Role memberRole = unitOfWork.RoleRepository
                        .GetBy(i => i.isContractor)
                        .FirstOrDefault();
                    
                    if (memberRole == null)
                        throw new InvalidOperationException("Role does not exist.");
                    
                    Company company = new Company();

                    if (data.companyId == Guid.Empty)
                    {
                        
                        company.name = data.companyName;
                        company.email = data.email;
                        //company.billingAddressLine1 = data.addressLine1;
                        //company.billingAddressLine2 = data.addressLine2;
                        //company.billingCity = data.city;
                        //company.billingState = data.state;
                        //company.billingZip = data.zip;
                        //company.shippingAddressLine1 = data.addressLine1;
                        //company.shippingAddressLine2 = data.addressLine2;
                        //company.shippingCity = data.city;
                        //company.shippingState = data.state;
                        //company.shippingZip = data.zip;

                        unitOfWork.CompanyRepository.Insert(company);
                        unitOfWork.Save();
                        
                        member.companyId = company.companyId;
                    
                    } else {
                        member.companyId = data.companyId;
                    }

                    member.email = email;
                    member.originalEmail = email;
                    member.phone = data.phone;
                    member.password = encrypt.password;
                    member.keyValue = encrypt.keyBytes;
                    member.iVValue = encrypt.ivBytes;
                    member.token = Guid.NewGuid();
                    member.tokenApi = Guid.NewGuid();
                    member.roles.Add(memberRole);
                    
                    unitOfWork.MemberRepository.Insert(member);
                    unitOfWork.Save();
                    
                    new Thread(() =>
                    {
                        EmailController.Send(new MailAddress(EmailController.email),
                            EmailController.email,
                            EmailController.email,
                            EmailController.email,
                            "Ciclops: Member Sign Up",
                            String.Format("Email: {0} / Company: {1} / Phone: {2}", data.email, data.companyName, data.phone));
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

                    UnitOfWork unitOfWork = new UnitOfWork();

                    Member member = unitOfWork.MemberRepository
                        .GetBy(i => i.email == data.email.ToLower()
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (member == null)
                        throw new InvalidOperationException("Member does not exist, please sign up to continue.");

                    member.forgotPasswordToken = Guid.NewGuid();
                    member.forgotPasswordDateTime = DateTimeOffset.UtcNow;

                    unitOfWork.MemberRepository.Update(member);
                    unitOfWork.Save();
                    
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
                    
                    UnitOfWork unitOfWork = new UnitOfWork();

                    int keyCode = new Random().Next(90000) + 10000;
                    Member member = unitOfWork.MemberRepository
                        .GetBy(i => i.memberId == data.id
                            && !i.isDeleted)
                        .FirstOrDefault();

                    if (member == null)
                        throw new InvalidOperationException("Not Valid");

                    member.keyCode = keyCode;
                    member.keyCodeDateTime = DateTimeOffset.UtcNow;

                    unitOfWork.MemberRepository.Update(member);
                    unitOfWork.Save();
                    
                    LogController.Add(member.memberId, "Member " + member.email + " has been sent a Key Code", "Member", "SendKeyCode", member.memberId, "Members");

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

    }
}
