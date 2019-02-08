using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class Authentication
    {

        public bool isAuthenticated { get; set; } = false;

        public MemberViewModel member { get; set; } = new MemberViewModel();
        
    }
    
    public class AuthenticationViewModel
    {

        public Guid apiId { get; set; } = Guid.Empty;
        
        public Guid token { get; set; } = Guid.Empty;

    }
    
}