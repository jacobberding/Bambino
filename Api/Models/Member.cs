using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    [Table("Members")]
    public class Member
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid memberId { get; set; }
        
        [ForeignKey("company")]
        public Guid companyId { get; set; }
        public virtual Company company { get; set; }
        
        public Guid token { get; set; }
        
        public Guid tokenApi { get; set; }
        
        public string stripeId { get; set; }
        
        public string firstName { get; set; }
        
        public string lastName { get; set; }
        
        public string email { get; set; }
        
        public string originalEmail { get; set; }
        
        public string phone { get; set; }
        
        public byte[] password { get; set; }
        
        public byte[] keyValue { get; set; }
        
        public byte[] iVValue { get; set; }
        
        public Guid? forgotPasswordToken { get; set; }
        
        public DateTimeOffset? forgotPasswordDateTime { get; set; }
        
        public int? keyCode { get; set; }
        
        public DateTimeOffset? keyCodeDateTime { get; set; }
        
        public DateTimeOffset lastLoginDateTime { get; set; }
        
        public bool isValidated { get; set; }
        
        public bool isDeleted { get; set; }
        
        public virtual ICollection<MemberIpAddress> memberIpAddresss { get; set; }
        public virtual ICollection<Log> logs { get; set; }
        public virtual ICollection<Role> roles { get; set; }

        public Member()
        {
            companyId                   = Guid.Empty;
            token                       = Guid.Empty;
            tokenApi                    = Guid.Empty;
            stripeId                    = "";
            firstName                   = ""; 
            lastName                    = "";
            email                       = "";
            originalEmail               = "";
            phone                       = "";
            password                    = new byte[] { };
            keyValue                    = new byte[] { };
            iVValue                     = new byte[] { };
            forgotPasswordToken         = null;
            forgotPasswordDateTime      = null;
            keyCode                     = null;
            keyCodeDateTime             = null;
            lastLoginDateTime           = DateTimeOffset.UtcNow;
            isValidated                 = false;
            isDeleted                   = false;
            memberIpAddresss            = new List<MemberIpAddress>();
            logs                        = new List<Log>();
            roles                       = new List<Role>();
        }

    }

    public class MemberViewModel
    {
        
        public Guid memberId { get; set; }
        
        public Guid companyId { get; set; }
        public CompanyViewModel company { get; set; }
        
        public Guid token { get; set; }
        
        public Guid tokenApi { get; set; }
        
        public string stripeId { get; set; }
        
        public string firstName { get; set; }
        
        public string lastName { get; set; }
        
        public string email { get; set; }
        
        public string originalEmail { get; set; }
        
        public string phone { get; set; }
        
        public bool isValidated { get; set; }
        
        public bool isDeleted { get; set; }
        
        public List<RoleViewModel> roles { get; set; }

        public MemberViewModel()
        {
            memberId                    = Guid.Empty;
            companyId                   = Guid.Empty;
            company                     = new CompanyViewModel();
            token                       = Guid.Empty;
            tokenApi                    = Guid.Empty;
            stripeId                    = "";
            firstName                   = ""; 
            lastName                    = "";
            email                       = "";
            originalEmail               = "";
            phone                       = "";
            isValidated                 = false;
            isDeleted                   = false;
            roles                       = new List<RoleViewModel>();
        }

    }
    
    public class MemberAddCardViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public string token { get; set; }
        
        public MemberAddCardViewModel()
        {
            token                       = "";
        }

    }

    public class MemberEditDeleteViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid memberId { get; set; }
        
        public Guid companyId { get; set; }
        
        public string firstName { get; set; }
        
        public string lastName { get; set; }
        
        public string email { get; set; }
        
        public string phone { get; set; }
        
        public bool isDeleted { get; set; }
        
        public MemberEditDeleteViewModel()
        {
            memberId                    = Guid.Empty;
            companyId                   = Guid.Empty;
            firstName                   = ""; 
            lastName                    = "";
            email                       = "";
            phone                       = "";
            isDeleted                   = false;
        }

    }


    public class MemberEditPasswordViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public string passwordOld { get; set; }
        
        public string passwordNew { get; set; }
        
    }
    
    public class MemberEditResetPasswordViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public string email { get; set; }
        
        public string password { get; set; }

        public Guid forgotPasswordToken { get; set; }
        
    }

    public class MemberEditValidatedViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid token { get; set; }
        
        public int keyCode { get; set; }

    }
    
    public class MemberDeleteCardViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public string cardId { get; set; }
        
    }

    public class MemberGetByAuthTokenValidationViewModel
    {
        
        public Guid apiId { get; set; }
        
        public Guid token { get; set; }

        public string email { get; set; }
        
        public bool isAdmin { get; set; }
        
    }
    
    public class MemberGetKeyCodeViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid token { get; set; }

    }

    public class MemberSignInViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public string email { get; set; }
        
        public string password { get; set; }
        
        public string v { get; set; }
        
    }
    
    public class MemberSignUpViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }

        public Guid companyId { get; set; }

        public string companyName { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }
        
        public string phone { get; set; }
        
        public string password { get; set; }
        
    }
    
    public class MemberForgotPasswordViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public string email { get; set; }
        
    }
    
}