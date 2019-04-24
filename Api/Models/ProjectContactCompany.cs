using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ProjectContactCompanyViewModel
    {

        public long contactCompanyKey { get; set; } = 0;

        public string name { get; set; } = "";

        public long value { get; set; } = 0;

        public long disciplineKey { get; set; } = 0;

        public bool isClient { get; set; } = false;

        public bool isVendorDesign { get; set; } = false;

        public bool isVendorIntegration { get; set; } = false;

    }
}