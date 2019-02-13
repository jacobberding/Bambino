﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class MaterialViewModel
    {
        
        public Guid materialId { get; set; } = Guid.Empty;

        public Guid disciplineId { get; set; } = Guid.Empty;
        public DisciplineViewModel discipline { get; set; } = new DisciplineViewModel();

        
        public string name { get; set; } = "";

        
        public string description { get; set; } = "";

        
        public string website { get; set; } = "";

        public decimal priceMin { get; set; } = 0.0000m;

        public decimal priceMax { get; set; } = 0.0000m;

        public int materialPriceOptionKey { get; set; } = 0;
        public MaterialPriceOptionViewModel materialPriceOption { get; set; } = new MaterialPriceOptionViewModel();

        
        public string manufacturer { get; set; } = "";

        
        public string modelNumber { get; set; } = "";

        
        public string notes { get; set; } = "";

        public bool isDeleted { get; set; } = false;

        public List<MaterialTagViewModel> materialTags { get; set; } = new List<MaterialTagViewModel>();
        
    }

    public class MaterialAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid materialId { get; set; } = Guid.Empty;

        public Guid disciplineId { get; set; } = Guid.Empty;

        
        public string name { get; set; } = "";

        
        public string description { get; set; } = "";

        
        public string website { get; set; } = "";

        public decimal priceMin { get; set; } = 0.0000m;

        public decimal priceMax { get; set; } = 0.0000m;

        public int materialPriceOptionKey { get; set; } = 0;

        
        public string manufacturer { get; set; } = "";

        
        public string modelNumber { get; set; } = "";

        
        public string notes { get; set; } = "";

        public bool isDeleted { get; set; } = false;
        
    }

    public class MaterialAddDeleteMaterialTagViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid materialId { get; set; } = Guid.Empty;

        public Guid materialTagId { get; set; } = Guid.Empty;

        
        public string name { get; set; } = "";

    }

}