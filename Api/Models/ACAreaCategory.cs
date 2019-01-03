using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("ACAreaCategories")]
    public class ACAreaCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid acAreaCategoryId { get; set; }

        public string name { get; set; }

        public bool isDeleted { get; set; }

        public ACAreaCategory() {
            name = "";
            isDeleted = false;
        }

    }

    public class ACAreaCategoryViewModel
    {

        public Guid acAreaCategoryId { get; set; }

        public string name { get; set; }

        public bool isDeleted { get; set; }

        public ACAreaCategoryViewModel()
        {
            acAreaCategoryId = Guid.Empty;
            name = "";
            isDeleted = false;
        }

    }

    public class ACAreaCategoryAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid acAreaCategoryId { get; set; }

        public string name { get; set; }

        public bool isDeleted { get; set; }

        public ACAreaCategoryAddEditDeleteViewModel()
        {
            acAreaCategoryId = Guid.Empty;
            name = "";
            isDeleted = false;
        }

    }

}