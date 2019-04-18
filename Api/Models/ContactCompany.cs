using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ContactCompanyViewModel
    {

        public long contactCompanyKey { get; set; } = 0;
        
        public string name { get; set; } = "";
        
        public string email { get; set; } = "";

        public string phone { get; set; } = "";

        public string website { get; set; } = "";

        public string addressLine1 { get; set; } = "";

        public string addressLine2 { get; set; } = "";

        public string city { get; set; } = "";

        public string state { get; set; } = "";

        public string country { get; set; } = "";

        public string zip { get; set; } = "";
        
        public bool isVendorDesign { get; set; } = false;

        public bool isVendorIntegration { get; set; } = false;

        public bool isClient { get; set; } = false;

        public bool isDeleted { get; set; } = false;

        public List<ContactViewModel> contacts { get; set; } = new List<ContactViewModel>();

    }

    public class ContactCompanyAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public long contactCompanyKey { get; set; } = 0;

        public string name { get; set; } = "";

        public string email { get; set; } = "";

        public string phone { get; set; } = "";

        public string website { get; set; } = "";

        public string addressLine1 { get; set; } = "";

        public string addressLine2 { get; set; } = "";

        public string city { get; set; } = "";

        public string state { get; set; } = "";

        public string country { get; set; } = "";

        public string zip { get; set; } = "";

        public bool isVendorDesign { get; set; } = false;

        public bool isVendorIntegration { get; set; } = false;

        public bool isClient { get; set; } = false;

        public bool isDeleted { get; set; } = false;
        
    }

}