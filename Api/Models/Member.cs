using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{

    public class MemberViewModel
    {
        
        public Guid memberId { get; set; } = Guid.Empty;

        public Guid activeCompanyId { get; set; } = Guid.Empty;

        public Guid token { get; set; } = Guid.Empty;

        public Guid tokenApi { get; set; } = Guid.Empty;
        
        public string stripeId { get; set; } = "";
        
        public string firstName { get; set; } = "";
        
        public string lastName { get; set; } = "";
        
        public string path { get; set; } = "";
        
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

    public class MemberEditPathViewModel
    {

        public AuthenticationViewModel authentication { get; set; }
        
        public string path { get; set; } = "";
        
        public string originalFileName { get; set; } = "";

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

    public class MemberSignInGoogleViewModel
    {

        public AuthenticationViewModel authentication { get; set; }
        
        public string email { get; set; } = "";
        
        public string path { get; set; } = "";
        
        public string firstName { get; set; } = "";
        
        public string lastName { get; set; } = "";
        
        public string token { get; set; } = "";
        
        public string v { get; set; } = "";

    }

}