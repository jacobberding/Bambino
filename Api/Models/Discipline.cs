using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("Disciplines")]
    public class Discipline
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid disciplineId { get; set; }

        public string name { get; set; }

        public string value { get; set; }

        public string description { get; set; }

        public bool isDeleted { get; set; }

        //public virtual ICollection<Layer> layers { get; set; }

        public Discipline()
        {
            name = "";
            value = "";
            description = "";
            isDeleted = false;
            //layers = new List<Layer>();
        }

    }

    public class DisciplineViewModel
    {

        public Guid disciplineId { get; set; }

        public string name { get; set; }

        public string value { get; set; }

        public string description { get; set; }

        public bool isDeleted { get; set; }
        
        public DisciplineViewModel()
        {
            disciplineId = new Guid();
            name = "";
            value = "";
            description = "";
            isDeleted = false;
        }

    }

}