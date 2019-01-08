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

        public string website { get; set; }

        public decimal priceMin { get; set; }

        public decimal priceMax { get; set; }

        [ForeignKey("materialPriceOption")]
        public int materialPriceOptionKey { get; set; }
        public virtual MaterialPriceOption materialPriceOption { get; set; }

        public string manufacturer { get; set; }

        public string modelNumber { get; set; }

        public string tags { get; set; }

        public string notes { get; set; }

        public bool isDeleted { get; set; }

        public Material()
        {
            disciplineId = Guid.Empty;
            name = "";
            description = "";
            website = "";
            priceMin = 0.0000m;
            priceMax = 0.0000m;
            materialPriceOptionKey = 0;
            manufacturer = "";
            modelNumber = "";
            tags = "";
            notes = "";
            isDeleted = false;
        }

    }

    public class MaterialViewModel
    {
        
        public Guid materialId { get; set; }

        public Guid disciplineId { get; set; }
        public DisciplineViewModel discipline { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string website { get; set; }

        public decimal priceMin { get; set; }

        public decimal priceMax { get; set; }

        public int materialPriceOptionKey { get; set; }
        public MaterialPriceOptionViewModel materialPriceOption { get; set; }

        public string manufacturer { get; set; }

        public string modelNumber { get; set; }

        public string tags { get; set; }

        public string notes { get; set; }

        public bool isDeleted { get; set; }

        public MaterialViewModel()
        {
            materialId = Guid.Empty;
            disciplineId = Guid.Empty;
            discipline = new DisciplineViewModel();
            name = "";
            description = "";
            website = "";
            priceMin = 0.0000m;
            priceMax = 0.0000m;
            materialPriceOptionKey = 0;
            materialPriceOption = new MaterialPriceOptionViewModel();
            manufacturer = "";
            modelNumber = "";
            tags = "";
            notes = "";
            isDeleted = false;
        }

    }

    public class MaterialAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid materialId { get; set; }
        
        public Guid disciplineId { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string website { get; set; }

        public decimal priceMin { get; set; }

        public decimal priceMax { get; set; }
        
        public int materialPriceOptionKey { get; set; }

        public string manufacturer { get; set; }

        public string modelNumber { get; set; }

        public string tags { get; set; }

        public string notes { get; set; }

        public bool isDeleted { get; set; }

        public MaterialAddEditDeleteViewModel()
        {
            materialId = Guid.Empty;
            disciplineId = Guid.Empty;
            name = "";
            description = "";
            website = "";
            priceMin = 0.0000m;
            priceMax = 0.0000m;
            materialPriceOptionKey = 0;
            manufacturer = "";
            modelNumber = "";
            tags = "";
            notes = "";
            isDeleted = false;
        }

    }

}