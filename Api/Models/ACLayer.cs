using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("ACLayers")]
    public class ACLayer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid acLayerId { get; set; }

        [ForeignKey("acLayerCategory")]
        public Guid acLayerCategoryId { get; set; }
        public virtual ACLayerCategory acLayerCategory { get; set; }

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

        public ACLayer()
        {
            acLayerCategoryId = Guid.Empty;
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

    public class ACLayerViewModel
    {

        public Guid acLayerId { get; set; }
        
        public Guid acLayerCategoryId { get; set; }
        public ACLayerCategoryViewModel acLayerCategory { get; set; }

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

        public ACLayerViewModel()
        {
            acLayerId = Guid.Empty;
            acLayerCategoryId = Guid.Empty;
            acLayerCategory = new ACLayerCategoryViewModel();
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

    public class ACLayerAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid acLayerId { get; set; }

        public Guid acLayerCategoryId { get; set; }
        
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

        public ACLayerAddEditDeleteViewModel()
        {
            acLayerId = Guid.Empty;
            acLayerCategoryId = Guid.Empty;
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

    public class ACLayerAddManyViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public string categoryValue { get; set; }

        public string color { get; set; }

        public string lineWeight { get; set; }

        public string lineType { get; set; }

        public int transparency { get; set; }

        public string measurement { get; set; }

        public string code { get; set; }
        
        public string description { get; set; }

        public bool isPlottable { get; set; }
        
        public ACLayerAddManyViewModel()
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

    public class ACLayerGetByKeywordViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public string keyword { get; set; }

        public string measurement { get; set; }

        public ACLayerGetByKeywordViewModel()
        {
            keyword = "";
            measurement = "";
        }

    }

    public class ACLayerGetByCategoryViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public string category { get; set; }

        public string measurement { get; set; }

        public ACLayerGetByCategoryViewModel()
        {
            category = "";
            measurement = "";
        }

    }

}