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

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string title { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string companyName { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string phone1 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string phone2 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string skypeId { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string email { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string companyTemp { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string resume { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string portfolio { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string personalWebsite { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
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

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string title { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string companyName { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string phone1 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string phone2 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string skypeId { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string email { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string companyTemp { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string resume { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string portfolio { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string personalWebsite { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
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