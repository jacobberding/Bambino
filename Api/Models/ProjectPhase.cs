using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("ProjectPhases")]
    public class ProjectPhase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid projectPhaseId { get; set; }

        [ForeignKey("project")]
        public Guid projectId { get; set; } = Guid.Empty;
        public virtual Project project { get; set; }
        
        public string name { get; set; } = "";

        public string description { get; set; } = "";

        public int sortOrder { get; set; } = 0;

        public DateTimeOffset dateStart { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateEnd { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;
        
    }

    public class ProjectPhaseViewModel
    {

        public Guid projectPhaseId { get; set; }
        
        public Guid projectId { get; set; } = Guid.Empty;
        public ProjectViewModel project { get; set; } = new ProjectViewModel();

        public string name { get; set; } = "";

        public string description { get; set; } = "";

        public int sortOrder { get; set; } = 0;

        public DateTimeOffset dateStart { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateEnd { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

    }
    
    public class ProjectPhaseAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid projectPhaseId { get; set; } = Guid.Empty;

        public Guid projectId { get; set; } = Guid.Empty;

        public string name { get; set; } = "";

        public string description { get; set; } = "";
        
        public DateTimeOffset dateStart { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateEnd { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

    }

}