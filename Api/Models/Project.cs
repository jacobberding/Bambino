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

        [ForeignKey("company")]
        public Guid companyId { get; set; } = Guid.Empty;
        public virtual Company company { get; set; }

        public string code { get; set; } = "";

        public string name { get; set; } = "";

        public string addressLine1 { get; set; } = "";

        public string addressLine2 { get; set; } = "";

        public string city { get; set; } = "";

        public string state { get; set; } = "";

        public string zip { get; set; } = "";

        public string country { get; set; } = "";

        public string scale { get; set; } = "Imperial";

        public bool isDefault { get; set; } = false;

        public bool isDeleted { get; set; } = false;

        public virtual ICollection<TimeTrackerProject> timeTrackerProjects { get; set; } = new List<TimeTrackerProject>();
        public virtual ICollection<Tsk> tsks { get; set; } = new List<Tsk>();
        public virtual ICollection<Member> members { get; set; } = new List<Member>();
        public virtual ICollection<ProjectPhase> projectPhases { get; set; } = new List<ProjectPhase>();
        public virtual ICollection<ProjectZone> projectZones { get; set; } = new List<ProjectZone>();

    }

    public class ProjectViewModel
    {

        public Guid projectId { get; set; }
        
        public Guid companyId { get; set; } = Guid.Empty;
        public CompanyViewModel company { get; set; }

        public Guid projectPhaseId { get; set; }

        public string code { get; set; } = "";

        public string name { get; set; } = "";

        public string addressLine1 { get; set; } = "";

        public string addressLine2 { get; set; } = "";

        public string city { get; set; } = "";

        public string state { get; set; } = "";

        public string zip { get; set; } = "";

        public string country { get; set; } = "";

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

        public string code { get; set; } = "";

        public string name { get; set; } = "";

        public string addressLine1 { get; set; } = "";

        public string addressLine2 { get; set; } = "";

        public string city { get; set; } = "";

        public string state { get; set; } = "";

        public string zip { get; set; } = "";

        public string country { get; set; } = "";

        public string scale { get; set; } = "Imperial";

        public bool isDefault { get; set; } = false;

        public bool isDeleted { get; set; } = false;
        
    }

    public class ProjectAddDeleteMemberViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid projectId { get; set; } = Guid.Empty;

        public Guid token { get; set; } = Guid.Empty;

        public string email { get; set; } = "";

    }
    
    public class ProjectGetByCodeViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public string code { get; set; } = "";
        
    }

}