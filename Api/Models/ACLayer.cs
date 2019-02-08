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

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string color { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string lineWeight { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string lineType { get; set; } = "";

        public int transparency { get; set; } = 0;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string measurement { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string code { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string keywords { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string description { get; set; } = "";

        public bool isPlottable { get; set; } = false;

        public bool isDeleted { get; set; } = false;
        
    }

    public class ACLayerAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid acLayerId { get; set; } = Guid.Empty;

        public Guid acLayerCategoryId { get; set; } = Guid.Empty;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string color { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string lineWeight { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string lineType { get; set; } = "";

        public int transparency { get; set; } = 0;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string measurement { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string code { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string keywords { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string description { get; set; } = "";

        public bool isPlottable { get; set; } = false;

        public bool isDeleted { get; set; } = false;
        
    }

    public class ACLayerAddManyViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string categoryValue { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string color { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string lineWeight { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string lineType { get; set; } = "";

        public int transparency { get; set; } = 0;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string measurement { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string code { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string description { get; set; } = "";

        public bool isPlottable { get; set; } = false;
        
    }

    public class ACLayerGetByKeywordViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string keyword { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string measurement { get; set; } = "";
        
    }

    public class ACLayerGetByCategoryViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string category { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string measurement { get; set; } = "";
        
    }

}