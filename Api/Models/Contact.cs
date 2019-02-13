using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ContactViewModel
    {

        public Guid contactId { get; set; } = Guid.Empty;

        
        public string name { get; set; } = "";

        
        public string title { get; set; } = "";

        
        public string companyName { get; set; } = "";

        
        public string phone1 { get; set; } = "";

        
        public string phone2 { get; set; } = "";

        
        public string skypeId { get; set; } = "";

        
        public string email { get; set; } = "";

        
        public string companyTemp { get; set; } = "";

        
        public string resume { get; set; } = "";

        
        public string portfolio { get; set; } = "";

        
        public string personalWebsite { get; set; } = "";

        
        public string skills { get; set; } = "";

        public bool isEdcFamily { get; set; } = false;

        public bool isPotentialStaffing { get; set; } = false;

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isDeleted { get; set; } = false;

        public List<ContactFileViewModel> contactFiles { get; set; } = new List<ContactFileViewModel>();

    }

    public class ContactAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid contactId { get; set; } = Guid.Empty;

        
        public string name { get; set; } = "";

        
        public string title { get; set; } = "";

        
        public string companyName { get; set; } = "";

        
        public string phone1 { get; set; } = "";

        
        public string phone2 { get; set; } = "";

        
        public string skypeId { get; set; } = "";

        
        public string email { get; set; } = "";

        
        public string companyTemp { get; set; } = "";

        
        public string resume { get; set; } = "";

        
        public string portfolio { get; set; } = "";

        
        public string personalWebsite { get; set; } = "";

        
        public string skills { get; set; } = "";

        public bool isEdcFamily { get; set; } = false;

        public bool isPotentialStaffing { get; set; } = false;

        public bool isDeleted { get; set; } = false;
        
    }

    public class ContactUploadViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public List<ContactViewModel> contactViewModels = new List<ContactViewModel>();
        
    }

}