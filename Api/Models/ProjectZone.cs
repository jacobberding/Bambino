using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("ProjectZones")]
    public class ProjectZone
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid projectZoneId { get; set; }

        [ForeignKey("project")]
        public Guid projectId { get; set; } = Guid.Empty;
        public virtual Project project { get; set; }

        public string name { get; set; } = "";

        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

        public virtual ICollection<ProjectAttraction> projectAttractions { get; set; } = new List<ProjectAttraction>();

    }

    public class ProjectZoneViewModel
    {

        public Guid projectZoneId { get; set; } = Guid.Empty;
        
        public Guid projectId { get; set; } = Guid.Empty;
        public ProjectViewModel project { get; set; } = new ProjectViewModel();

        public string name { get; set; } = "";

        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

        public List<ProjectAttractionViewModel> projectAttractions { get; set; } = new List<ProjectAttractionViewModel>();

    }

    public class ProjectZoneAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid projectZoneId { get; set; } = Guid.Empty;

        public Guid projectId { get; set; } = Guid.Empty;

        public string name { get; set; } = "";

        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public bool isDeleted { get; set; } = false;
        
    }

}