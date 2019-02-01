using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("ProjectElements")]
    public class ProjectElement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid projectElementId { get; set; }

        [ForeignKey("projectAttraction")]
        public Guid projectAttractionId { get; set; } = Guid.Empty;
        public virtual ProjectAttraction projectAttraction { get; set; }

        [ForeignKey("discipline")]
        public Guid disciplineId { get; set; } = Guid.Empty;
        public virtual Discipline discipline { get; set; }

        public string name { get; set; } = "";

        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public int quantity { get; set; } = 0;

        public string unit { get; set; } = "";

        public string notes { get; set; } = "";

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

    }

    public class ProjectElementViewModel
    {

        public Guid projectElementId { get; set; } = Guid.Empty;
        
        public Guid projectAttractionId { get; set; } = Guid.Empty;
        public ProjectAttractionViewModel projectAttraction { get; set; } = new ProjectAttractionViewModel();
        
        public Guid disciplineId { get; set; } = Guid.Empty;
        public DisciplineViewModel discipline { get; set; } = new DisciplineViewModel();

        public string name { get; set; } = "";

        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public int quantity { get; set; } = 0;

        public string unit { get; set; } = "";

        public string notes { get; set; } = "";

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

    }

}