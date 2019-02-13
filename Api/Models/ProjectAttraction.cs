using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ProjectAttractionViewModel
    {

        public Guid projectAttractionId { get; set; } = Guid.Empty;
        
        public Guid projectZoneId { get; set; } = Guid.Empty;
        public ProjectZoneViewModel projectZone { get; set; } = new ProjectZoneViewModel();

        
        public string name { get; set; } = "";

        
        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

        public List<ProjectElementViewModel> projectElements { get; set; } = new List<ProjectElementViewModel>();
        public List<ProjectWritingDocumentViewModel> projectWritingDocuments { get; set; } = new List<ProjectWritingDocumentViewModel>();

    }

}