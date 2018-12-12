using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("TagAreaTypes")]
    public class TagAreaType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid tagAreaTypeId { get; set; }

        public string name { get; set; }

        public bool isDeleted { get; set; }

        public TagAreaType() {
            name = "";
            isDeleted = false;
        }

    }

    public class TagAreaTypeViewModel
    {

        public Guid tagAreaTypeId { get; set; }

        public string name { get; set; }

        public bool isDeleted { get; set; }

        public TagAreaTypeViewModel()
        {
            tagAreaTypeId = Guid.Empty;
            name = "";
            isDeleted = false;
        }

    }

    public class TagAreaTypeAddEditDeleteViewModel
    {

        public Guid tagAreaTypeId { get; set; }

        public string name { get; set; }

        public bool isDeleted { get; set; }

        public TagAreaTypeAddEditDeleteViewModel()
        {
            tagAreaTypeId = Guid.Empty;
            name = "";
            isDeleted = false;
        }

    }

}