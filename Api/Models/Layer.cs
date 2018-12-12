using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("Layers")]
    public class Layer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid layerId { get; set; }

        [ForeignKey("category")]
        public Guid categoryId { get; set; }
        public virtual Category category { get; set; }

        public string name { get; set; }

        public string color { get; set; }

        public string lineWeight { get; set; }

        public string lineType { get; set; }

        public int transparency { get; set; }

        public string measurement { get; set; }

        public string code { get; set; }

        public string keywords { get; set; }

        public string description { get; set; }

        public bool isPlottable { get; set; }

        public bool isDeleted { get; set; }

        public Layer()
        {
            categoryId = Guid.Empty;
            name = "";
            color = "";
            lineWeight = "";
            lineType = "";
            transparency = 0;
            measurement = "";
            code = "";
            keywords = "";
            description = "";
            isPlottable = false;
            isDeleted = false;
        }

    }

    public class LayerViewModel
    {

        public Guid layerId { get; set; }
        
        public Guid categoryId { get; set; }
        public CategoryViewModel category { get; set; }

        public string name { get; set; }

        public string color { get; set; }

        public string lineWeight { get; set; }

        public string lineType { get; set; }

        public int transparency { get; set; }

        public string measurement { get; set; }

        public string code { get; set; }

        public string keywords { get; set; }

        public string description { get; set; }

        public bool isPlottable { get; set; }

        public bool isDeleted { get; set; }

        public LayerViewModel()
        {
            layerId = Guid.Empty;
            categoryId = Guid.Empty;
            category = new CategoryViewModel();
            name = "";
            color = "";
            lineWeight = "";
            lineType = "";
            transparency = 0;
            measurement = "";
            code = "";
            keywords = "";
            description = "";
            isPlottable = false;
            isDeleted = false;
        }

    }

    public class LayerAddEditDeleteViewModel
    {

        public Guid layerId { get; set; }

        public Guid categoryId { get; set; }
        
        public string color { get; set; }

        public string lineWeight { get; set; }

        public string lineType { get; set; }

        public int transparency { get; set; }

        public string measurement { get; set; }

        public string code { get; set; }

        public string keywords { get; set; }

        public string description { get; set; }

        public bool isPlottable { get; set; }

        public bool isDeleted { get; set; }

        public LayerAddEditDeleteViewModel()
        {
            layerId = Guid.Empty;
            categoryId = Guid.Empty;
            color = "";
            lineWeight = "";
            lineType = "";
            transparency = 0;
            measurement = "";
            code = "";
            keywords = "";
            description = "";
            isPlottable = false;
            isDeleted = false;
        }

    }

    public class LayerAddManyViewModel
    {
        
        public string categoryValue { get; set; }

        public string color { get; set; }

        public string lineWeight { get; set; }

        public string lineType { get; set; }

        public int transparency { get; set; }

        public string measurement { get; set; }

        public string code { get; set; }
        
        public string description { get; set; }

        public bool isPlottable { get; set; }
        
        public LayerAddManyViewModel()
        {
            categoryValue = "";
            color = "";
            lineWeight = "";
            lineType = "";
            transparency = 0;
            measurement = "";
            code = "";
            description = "";
            isPlottable = false;
        }

    }

    public class LayerGetByKeywordViewModel
    {

        public string keyword { get; set; }

        public string measurement { get; set; }

        public LayerGetByKeywordViewModel()
        {
            keyword = "";
            measurement = "";
        }

    }

    public class LayerGetByCategoryViewModel
    {

        public string category { get; set; }

        public string measurement { get; set; }

        public LayerGetByCategoryViewModel()
        {
            category = "";
            measurement = "";
        }

    }

}