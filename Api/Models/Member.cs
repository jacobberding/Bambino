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

        public Guid activeCompanyId { get; set; } = Guid.Empty;

        public Guid token { get; set; } = Guid.Empty;

        public Guid tokenApi { get; set; } = Guid.Empty;

        public string stripeId { get; set; } = "";

        public string firstName { get; set; } = "";

        public string lastName { get; set; } = "";

        public string email { get; set; } = "";

        public string originalEmail { get; set; } = "";

        public string phone { get; set; } = "";

        public byte[] password { get; set; } = new byte[] { };

        public byte[] keyValue { get; set; } = new byte[] { };

        public byte[] iVValue { get; set; } = new byte[] { };

        public Guid? forgotPasswordToken { get; set; } = null;

        public DateTimeOffset? forgotPasswordDateTime { get; set; } = null;

        public int? keyCode { get; set; } = null;

        public DateTimeOffset? keyCodeDateTime { get; set; } = null;

        public DateTimeOffset lastLoginDateTime { get; set; } = DateTimeOffset.UtcNow;

        public bool isValidated { get; set; } = false;

        public bool isDeleted { get; set; } = false;

        public virtual ICollection<Company> companies { get; set; } = new List<Company>();
        public virtual ICollection<MemberIpAddress> memberIpAddresss { get; set; } = new List<MemberIpAddress>();
        public virtual ICollection<Log> logs { get; set; } = new List<Log>();
        public virtual ICollection<Role> roles { get; set; } = new List<Role>();
        public virtual ICollection<Project> projects { get; set; } = new List<Project>();
        public virtual ICollection<TimeTracker> timeTrackers { get; set; } = new List<TimeTracker>();

    }

    public class MemberViewModel
    {
        
        public Guid memberId { get; set; } = Guid.Empty;

        public Guid activeCompanyId { get; set; } = Guid.Empty;

        public Guid token { get; set; } = Guid.Empty;

        public Guid tokenApi { get; set; } = Guid.Empty;

        public string stripeId { get; set; } = "";

        public string firstName { get; set; } = "";

        public string lastName { get; set; } = "";

        public string email { get; set; } = "";

        public string originalEmail { get; set; } = "";

        public string phone { get; set; } = "";

        public bool isActive { get; set; } = false;

        public bool isValidated { get; set; } = false;

        public bool isDeleted { get; set; } = false;

        public List<CompanyViewModel> companies { get; set; } = new List<CompanyViewModel>();
        public List<RoleViewModel> roles { get; set; } = new List<RoleViewModel>();
        
    }
    
    public class MemberAddCardViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public string token { get; set; } = "";
        
    }

    public class MemberEditDeleteViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; } 
        
        public Guid memberId { get; set; } = Guid.Empty;

        public Guid companyId { get; set; } = Guid.Empty;

        public string firstName { get; set; } = "";

        public string lastName { get; set; } = "";

        public string email { get; set; } = "";

        public string phone { get; set; } = "";

        public bool isDeleted { get; set; } = false;
        
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

    public class MemberAddDeleteRoleViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid memberId { get; set; }

        public Guid roleId { get; set; }

        public string name { get; set; }

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

        public string email { get; set; }

        public string password { get; set; }

        public Guid companyId { get; set; }

        public int pin { get; set; }

    }

    public class MemberForgotPasswordViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public string email { get; set; }
        
    }

    public class MemberInviteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public string email { get; set; } = "";
        
    }

}