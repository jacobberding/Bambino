using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class MaterialPriceOptionViewModel
    {

        public int materialPriceOptionKey { get; set; } = 0;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string abbreviation { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string description { get; set; } = "";

        public bool isDeleted { get; set; } = false;
        
    }

}