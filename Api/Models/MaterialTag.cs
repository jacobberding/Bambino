using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("MaterialTags")]
    public class MaterialTag
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid materialTagId { get; set; }
        
        public string name { get; set; } = "";
        
        public bool isDeleted { get; set; } = false;

        public virtual ICollection<Material> materials { get; set; } = new List<Material>();

    }

    public class MaterialTagViewModel
    {

        public Guid materialTagId { get; set; } = Guid.Empty;

        public string name { get; set; } = "";

        public bool isDeleted { get; set; } = false;

        public List<MaterialViewModel> materials { get; set; } = new List<MaterialViewModel>();

    }

}