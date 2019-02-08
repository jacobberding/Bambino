using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    
    public class ApiTokenViewModel
    {

        public Guid apiTokenId { get; set; } = Guid.Empty;

        public int accessLevel { get; set; } = 0; //0 User - 1 Admin

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string companyName { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string adminName { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string adminEmail { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string adminPhone { get; set; } = "";

        public bool isDeleted { get; set; } = false;
        
    }
    
    public class ApiTokenAddEditDeleteViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid apiTokenId { get; set; } = Guid.Empty;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string companyName { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string adminName { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string adminEmail { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string adminPhone { get; set; } = "";

        public bool isDeleted { get; set; } = false;
        
    }

}