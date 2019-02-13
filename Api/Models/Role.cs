using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class RoleViewModel
    {

        public Guid roleId { get; set; } = Guid.Empty;

        
        public string name { get; set; } = "";

        public bool isContractor { get; set; } = false;

        public bool isEmployee { get; set; } = false;

        public bool isManager { get; set; } = false;

        public bool isAdmin { get; set; } = false;

        public bool isSuperAdmin { get; set; } = false;
        
    }

}