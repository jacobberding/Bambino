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
        
        public string name { get; set; } = "";
        
        public string abbreviation { get; set; } = "";
        
        public string description { get; set; } = "";

        public bool isDeleted { get; set; } = false;
        
    }

}