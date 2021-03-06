﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class MaterialTagViewModel
    {

        public long materialTagKey { get; set; } = 0;
        
        public string name { get; set; } = "";

        public bool isDeleted { get; set; } = false;

        public List<MaterialViewModel> materials { get; set; } = new List<MaterialViewModel>();

    }

}