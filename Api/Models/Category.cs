using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("Categories")]
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid categoryId { get; set; }

        public string name { get; set; }

        public string value { get; set; }

        public string description { get; set; }

        public bool isDeleted { get; set; }

        public virtual ICollection<Layer> layers { get; set; }

        public Category()
        {
            name = "";
            value = "";
            description = "";
            isDeleted = false;
            layers = new List<Layer>();
        }

    }

    public class CategoryViewModel
    {

        public Guid categoryId { get; set; }

        public string name { get; set; }

        public string value { get; set; }

        public string description { get; set; }

        public bool isDeleted { get; set; }
        
        public CategoryViewModel()
        {
            categoryId = new Guid();
            name = "";
            value = "";
            description = "";
            isDeleted = false;
        }

    }

}