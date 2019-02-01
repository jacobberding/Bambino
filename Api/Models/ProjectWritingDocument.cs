using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("ProjectWritingDocuments")]
    public class ProjectWritingDocument
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid projectWritingDocumentId { get; set; }

        [ForeignKey("projectAttraction")]
        public Guid projectAttractionId { get; set; } = Guid.Empty;
        public virtual ProjectAttraction projectAttraction { get; set; }

        public string name { get; set; } = "";

        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

    }

    public class ProjectWritingDocumentViewModel
    {

        public Guid projectWritingDocumentId { get; set; } = Guid.Empty;

        public Guid projectAttractionId { get; set; } = Guid.Empty;
        public ProjectAttractionViewModel projectAttraction { get; set; } = new ProjectAttractionViewModel();

        public string name { get; set; } = "";

        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

    }

}