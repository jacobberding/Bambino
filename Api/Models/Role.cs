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
        
        public string name { get; set; } = "";

        public bool isContractor { get; set; } = false;

        public bool isEmployee { get; set; } = false;

        public bool isManager { get; set; } = false;

        public bool isAdmin { get; set; } = false;

        public bool isSuperAdmin { get; set; } = false;

        public virtual ICollection<Member> members { get; set; } = new List<Member>();
        
    }
    
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