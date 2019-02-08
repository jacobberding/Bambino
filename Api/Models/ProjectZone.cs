using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ProjectZoneViewModel
    {

        public Guid projectZoneId { get; set; } = Guid.Empty;
        
        public Guid projectId { get; set; } = Guid.Empty;
        public ProjectViewModel project { get; set; } = new ProjectViewModel();

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isArchived { get; set; } = false;

        public bool isDeleted { get; set; } = false;

        public List<ProjectZoneArchiveViewModel> projectZoneArchives { get; set; } = new List<ProjectZoneArchiveViewModel>();
        public List<ProjectAttractionViewModel> projectAttractions { get; set; } = new List<ProjectAttractionViewModel>();

    }

    public class ProjectZoneAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid projectZoneId { get; set; } = Guid.Empty;

        public Guid projectId { get; set; } = Guid.Empty;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public bool isDeleted { get; set; } = false;
        
    }

    public class ProjectZoneEditArchiveViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid projectZoneId { get; set; } = Guid.Empty;
        
        public bool isArchived { get; set; } = false;

    }

}