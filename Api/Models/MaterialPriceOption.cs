using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("MaterialPriceOptions")]
    public class MaterialPriceOption
    {
        [Key]
        public int materialPriceOptionKey { get; set; }
        
        public string name { get; set; }

        public string abbreviation { get; set; }

        public string description { get; set; }
        
        public bool isDeleted { get; set; }

        public virtual ICollection<Material> materials { get; set; }

        public MaterialPriceOption()
        {
            name = "";
            abbreviation = "";
            description = "";
            isDeleted = false;
            materials = new List<Material>();
        }

    }

    public class MaterialPriceOptionViewModel
    {

        public int materialPriceOptionKey { get; set; }

        public string name { get; set; }

        public string abbreviation { get; set; }

        public string description { get; set; }

        public bool isDeleted { get; set; }
        
        public MaterialPriceOptionViewModel()
        {
            materialPriceOptionKey = 0;
            name = "";
            abbreviation = "";
            description = "";
            isDeleted = false;
        }

    }

}