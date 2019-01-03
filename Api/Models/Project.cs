using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("Projects")]
    public class Project
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid projectId { get; set; }

        public string code { get; set; }

        public string name { get; set; }

        public string addressLine1 { get; set; }

        public string addressLine2 { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        public string zip { get; set; }

        public string country { get; set; }

        public bool isDeleted { get; set; }

        public Project()
        {
            code = "";
            name = "";
            addressLine1 = "";
            addressLine2 = "";
            city = "";
            state = "";
            zip = "";
            country = "";
            isDeleted = false;
        }

    }

    public class ProjectViewModel
    {

        public Guid projectId { get; set; }

        public string code { get; set; }

        public string name { get; set; }

        public string addressLine1 { get; set; }

        public string addressLine2 { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        public string zip { get; set; }

        public string country { get; set; }

        public bool isDeleted { get; set; }

        public ProjectViewModel()
        {
            projectId = Guid.Empty;
            code = "";
            name = "";
            addressLine1 = "";
            addressLine2 = "";
            city = "";
            state = "";
            zip = "";
            country = "";
            isDeleted = false;
        }

    }

    public class ProjectAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid projectId { get; set; }

        public string code { get; set; }

        public string name { get; set; }

        public string addressLine1 { get; set; }

        public string addressLine2 { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        public string zip { get; set; }

        public string country { get; set; }

        public bool isDeleted { get; set; }

        public ProjectAddEditDeleteViewModel()
        {
            projectId = Guid.Empty;
            code = "";
            name = "";
            addressLine1 = "";
            addressLine2 = "";
            city = "";
            state = "";
            zip = "";
            country = "";
            isDeleted = false;
        }

    }

    public class ProjectGetByCodeViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public string code { get; set; }
        
        public ProjectGetByCodeViewModel()
        {
            code = "";
        }

    }

}