using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("Materials")]
    public class Material
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid materialId { get; set; }

        [ForeignKey("discipline")]
        public Guid disciplineId { get; set; }
        public virtual Discipline discipline { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string manufacturer { get; set; }

        public string modelNumber { get; set; }

        public string tags { get; set; }
        
        public bool isDeleted { get; set; }

        public Material()
        {
            disciplineId = Guid.Empty;
            name = "";
            description = "";
            manufacturer = "";
            modelNumber = "";
            tags = "";
            isDeleted = false;
        }

    }
}