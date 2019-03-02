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

        public long projectZoneKey { get; set; } = 0;

        public Guid projectId { get; set; } = Guid.Empty;
        public ProjectViewModel project { get; set; } = new ProjectViewModel();
        
        public string name { get; set; } = "";
        
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

        public long projectZoneKey { get; set; } = 0;

        public Guid projectId { get; set; } = Guid.Empty;
        
        public string name { get; set; } = "";
        
        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public bool isDeleted { get; set; } = false;
        
    }
    
}