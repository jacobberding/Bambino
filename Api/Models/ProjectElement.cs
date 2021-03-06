﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ProjectElementViewModel
    {

        public long projectElementId { get; set; } = 0;
        
        public long projectAttractionKey { get; set; } = 0;
        public ProjectAttractionViewModel projectAttraction { get; set; } = new ProjectAttractionViewModel();
        
        public long disciplineKey { get; set; } = 0;
        public DisciplineViewModel discipline { get; set; } = new DisciplineViewModel();
        
        public string name { get; set; } = "";
        
        public string description { get; set; } = "";

        public int code { get; set; } = 0;

        public int quantity { get; set; } = 0;
        
        public string unit { get; set; } = "";
        
        public string notes { get; set; } = "";

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

    }

}