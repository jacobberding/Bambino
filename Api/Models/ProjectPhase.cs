using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ProjectPhaseViewModel
    {

        public Guid projectPhaseId { get; set; }
        
        public Guid projectId { get; set; } = Guid.Empty;
        public ProjectViewModel project { get; set; } = new ProjectViewModel();

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
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

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string description { get; set; } = "";
        
        public DateTimeOffset dateStart { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateEnd { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

    }

}