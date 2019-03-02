using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ProjectWritingDocumentViewModel
    {

        public long projectWritingDocumentKey { get; set; } = 0;

        public long projectAttractionKey { get; set; } = 0;
        public ProjectAttractionViewModel projectAttraction { get; set; } = new ProjectAttractionViewModel();
        
        public string name { get; set; } = "";
        
        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

    }

}