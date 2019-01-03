using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("ACLayerCategories")]
    public class ACLayerCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid acLayerCategoryId { get; set; }

        public string name { get; set; }

        public string value { get; set; }

        public string description { get; set; }

        public bool isDeleted { get; set; }

        public virtual ICollection<ACLayer> layers { get; set; }

        public ACLayerCategory()
        {
            name = "";
            value = "";
            description = "";
            isDeleted = false;
            layers = new List<ACLayer>();
        }

    }

    public class ACLayerCategoryViewModel
    {

        public Guid acLayerCategoryId { get; set; }

        public string name { get; set; }

        public string value { get; set; }

        public string description { get; set; }

        public bool isDeleted { get; set; }
        
        public ACLayerCategoryViewModel()
        {
            acLayerCategoryId = new Guid();
            name = "";
            value = "";
            description = "";
            isDeleted = false;
        }

    }

}