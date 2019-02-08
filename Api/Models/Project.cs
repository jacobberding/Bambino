using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ProjectViewModel
    {

        public Guid projectId { get; set; }
        
        public Guid companyId { get; set; } = Guid.Empty;
        public CompanyViewModel company { get; set; }

        public Guid projectPhaseId { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string code { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string addressLine1 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string addressLine2 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string city { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string state { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string zip { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string country { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string scale { get; set; } = "Imperial";

        public int numOfMembers { get; set; } = 0;

        public decimal numOfHours { get; set; } = 0;

        public bool isDefault { get; set; } = false;

        public bool isDeleted { get; set; } = false;

        public List<TimeTrackerProjectViewModel> timeTrackerProjects { get; set; } = new List<TimeTrackerProjectViewModel>();
        public List<TskViewModel> tsks { get; set; } = new List<TskViewModel>();
        public List<MemberViewModel> members { get; set; } = new List<MemberViewModel>();
        public List<ProjectPhaseViewModel> projectPhases { get; set; } = new List<ProjectPhaseViewModel>();

    }

    public class ProjectAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid projectId { get; set; } = Guid.Empty;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string code { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string addressLine1 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string addressLine2 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string city { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string state { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string zip { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string country { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string scale { get; set; } = "Imperial";

        public bool isDefault { get; set; } = false;

        public bool isDeleted { get; set; } = false;
        
    }

    public class ProjectAddDeleteMemberViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid projectId { get; set; } = Guid.Empty;

        public Guid token { get; set; } = Guid.Empty;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string email { get; set; } = "";

    }
    
    public class ProjectGetByCodeViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string code { get; set; } = "";
        
    }

}