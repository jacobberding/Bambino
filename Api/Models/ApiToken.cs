using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    
    public class ApiTokenViewModel
    {

        public Guid apiTokenId { get; set; } = Guid.Empty;

        public int accessLevel { get; set; } = 0; //0 User - 1 Admin

        
        public string companyName { get; set; } = "";

        
        public string adminName { get; set; } = "";

        
        public string adminEmail { get; set; } = "";

        
        public string adminPhone { get; set; } = "";

        public bool isDeleted { get; set; } = false;
        
    }
    
    public class ApiTokenAddEditDeleteViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid apiTokenId { get; set; } = Guid.Empty;

        
        public string companyName { get; set; } = "";

        
        public string adminName { get; set; } = "";

        
        public string adminEmail { get; set; } = "";

        
        public string adminPhone { get; set; } = "";

        public bool isDeleted { get; set; } = false;
        
    }

}