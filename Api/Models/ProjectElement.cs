using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ProjectElementViewModel
    {

        public Guid projectElementId { get; set; } = Guid.Empty;
        
        public Guid projectAttractionId { get; set; } = Guid.Empty;
        public ProjectAttractionViewModel projectAttraction { get; set; } = new ProjectAttractionViewModel();
        
        public Guid disciplineId { get; set; } = Guid.Empty;
        public DisciplineViewModel discipline { get; set; } = new DisciplineViewModel();

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public int quantity { get; set; } = 0;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string unit { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string notes { get; set; } = "";

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

    }

}