using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ACAreaCategoryViewModel
    {

        public Guid acAreaCategoryId { get; set; } = Guid.Empty;

        
        public string name { get; set; } = "";

        public bool isDeleted { get; set; } = false;
        
    }

    public class ACAreaCategoryAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; } 

        public Guid acAreaCategoryId { get; set; } = Guid.Empty;

        
        public string name { get; set; } = "";

        public bool isDeleted { get; set; } = false;
        
    }

}