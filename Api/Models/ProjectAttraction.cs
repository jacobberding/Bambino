using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ProjectAttractionViewModel
    {

        public long projectAttractionKey { get; set; } = 0;

        public long projectZoneKey { get; set; } = 0;
        public ProjectZoneViewModel projectZone { get; set; } = new ProjectZoneViewModel();
        
        public string name { get; set; } = "";
        
        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isArchived { get; set; } = false;

        public bool isDeleted { get; set; } = false;

        public List<ProjectElementViewModel> projectElements { get; set; } = new List<ProjectElementViewModel>();
        public List<ProjectWritingDocumentViewModel> projectWritingDocuments { get; set; } = new List<ProjectWritingDocumentViewModel>();

    }

    public class ProjectAttractionAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public long projectAttractionKey { get; set; } = 0;

        public long projectZoneKey { get; set; } = 0;

        public string name { get; set; } = "";
        
        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public bool isDeleted { get; set; } = false;

    }

}