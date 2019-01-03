using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("Roles")]
    public class Role
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid roleId { get; set; }
        
        public string name { get; set; }

        public bool isContractor { get; set; }
        
        public bool isEmployee { get; set; }
        
        public bool isAdmin { get; set; }
        
        public bool isSuperAdmin { get; set; }
        
        public virtual ICollection<Member> members { get; set; }

        public Role()
        {
            name                = "";
            isContractor        = false;
            isEmployee          = false;
            isAdmin             = false;
            isSuperAdmin        = false;
            members             = new List<Member>();
        }

    }
    
    public class RoleViewModel
    {

        public Guid roleId { get; set; }
        
        public string name { get; set; }

        public bool isContractor { get; set; }

        public bool isEmployee { get; set; }

        public bool isAdmin { get; set; }

        public bool isSuperAdmin { get; set; }

        public RoleViewModel()
        {
            name                = "";
            isContractor        = false;
            isEmployee          = false;
            isAdmin             = false;
            isSuperAdmin        = false;
        }

    }

}