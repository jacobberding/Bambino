using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class Authentication
    {

        public bool isAuthenticated { get; set; }

        public MemberViewModel member { get; set; }

        public Authentication()
        {
            isAuthenticated = false;
        }

    }
    
    public class AuthenticationViewModel
    {
        
        public Guid apiId { get; set; }
        
        public Guid token { get; set; }

    }
    
}