using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    [Table("ApiTokens")]
    public class ApiToken
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid apiTokenId { get; set; }

        public int accessLevel { get; set; }
        
        public string companyName { get; set; }

        public string adminName { get; set; }

        public string adminEmail { get; set; }

        public string adminPhone { get; set; }
        
        public bool isDeleted { get; set; }

        public ApiToken()
        {
            accessLevel         = 0; //0 User - 1 Admin
            companyName         = "";
            adminName           = "";
            adminEmail          = "";
            adminPhone          = "";
            isDeleted           = false;
        }

    }
    
    public class ApiTokenViewModel
    {

        public Guid apiTokenId { get; set; }

        public int accessLevel { get; set; }
        
        public string companyName { get; set; }

        public string adminName { get; set; }

        public string adminEmail { get; set; }

        public string adminPhone { get; set; }
        
        public bool isDeleted { get; set; }

        public ApiTokenViewModel()
        {
            apiTokenId          = Guid.Empty;
            accessLevel         = 0; //0 User - 1 Admin
            companyName         = "";
            adminName           = "";
            adminEmail          = "";
            adminPhone          = "";
            isDeleted           = false;
        }

    }
    
    public class ApiTokenAddEditDeleteViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid apiTokenId { get; set; }
        
        public string companyName { get; set; }

        public string adminName { get; set; }

        public string adminEmail { get; set; }

        public string adminPhone { get; set; }
        
        public bool isDeleted { get; set; }

        public ApiTokenAddEditDeleteViewModel()
        {
            apiTokenId          = Guid.Empty;
            companyName         = "";
            adminName           = "";
            adminEmail          = "";
            adminPhone          = "";
            isDeleted           = false;
        }

    }

}