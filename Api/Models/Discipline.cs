using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class DisciplineViewModel
    {

        public Guid disciplineId { get; set; } = Guid.Empty;

        
        public string name { get; set; } = "";

        
        public string value { get; set; } = "";

        
        public string description { get; set; } = "";

        public bool isDeleted { get; set; } = false;
        
    }

}