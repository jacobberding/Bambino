using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ACLayerViewModel
    {

        public Guid acLayerId { get; set; } = Guid.Empty;

        public Guid acLayerCategoryId { get; set; } = Guid.Empty;
        public ACLayerCategoryViewModel acLayerCategory { get; set; } = new ACLayerCategoryViewModel();

        
        public string name { get; set; } = "";

        
        public string color { get; set; } = "";

        
        public string lineWeight { get; set; } = "";

        
        public string lineType { get; set; } = "";

        public int transparency { get; set; } = 0;

        
        public string measurement { get; set; } = "";

        
        public string code { get; set; } = "";

        
        public string keywords { get; set; } = "";

        
        public string description { get; set; } = "";

        public bool isPlottable { get; set; } = false;

        public bool isDeleted { get; set; } = false;
        
    }

    public class ACLayerAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid acLayerId { get; set; } = Guid.Empty;

        public Guid acLayerCategoryId { get; set; } = Guid.Empty;

        
        public string color { get; set; } = "";

        
        public string lineWeight { get; set; } = "";

        
        public string lineType { get; set; } = "";

        public int transparency { get; set; } = 0;

        
        public string measurement { get; set; } = "";

        
        public string code { get; set; } = "";

        
        public string keywords { get; set; } = "";

        
        public string description { get; set; } = "";

        public bool isPlottable { get; set; } = false;

        public bool isDeleted { get; set; } = false;
        
    }

    public class ACLayerAddManyViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        
        public string categoryValue { get; set; } = "";

        
        public string color { get; set; } = "";

        
        public string lineWeight { get; set; } = "";

        
        public string lineType { get; set; } = "";

        public int transparency { get; set; } = 0;

        
        public string measurement { get; set; } = "";

        
        public string code { get; set; } = "";

        
        public string description { get; set; } = "";

        public bool isPlottable { get; set; } = false;
        
    }

    public class ACLayerGetByKeywordViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        
        public string keyword { get; set; } = "";

        
        public string measurement { get; set; } = "";
        
    }

    public class ACLayerGetByCategoryViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        
        public string category { get; set; } = "";

        
        public string measurement { get; set; } = "";
        
    }

}