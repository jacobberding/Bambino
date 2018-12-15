using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("Contacts")]
    public class Contact
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid contactId { get; set; }

        public string name { get; set; }

        public string title { get; set; }

        public string companyName { get; set; }

        public string phone1 { get; set; }

        public string phone2 { get; set; }

        public string skypeId { get; set; }

        public string email { get; set; }

        public string companyTemp { get; set; }

        public string resume { get; set; }

        public string portfolio { get; set; }

        public string personalWebsite { get; set; }

        public string skills { get; set; }

        public bool isEdcFamily { get; set; }

        public bool isPotentialStaffing { get; set; }

        public DateTimeOffset dateCreated { get; set; }

        public bool isDeleted { get; set; }
        
        public Contact()
        {
            name = "";
            title = "";
            companyName = "";
            phone1 = "";
            phone2 = "";
            skypeId = "";
            email = "";
            companyTemp = "";
            resume = "";
            portfolio = "";
            personalWebsite = "";
            skills = "";
            isEdcFamily = false;
            isPotentialStaffing = false;
            dateCreated = DateTimeOffset.UtcNow;
            isDeleted = false;
        }

    }

    public class ContactViewModel
    {

        public Guid contactId { get; set; }

        public string name { get; set; }

        public string title { get; set; }

        public string companyName { get; set; }

        public string phone1 { get; set; }

        public string phone2 { get; set; }

        public string skypeId { get; set; }

        public string email { get; set; }

        public string companyTemp { get; set; }

        public string resume { get; set; }

        public string portfolio { get; set; }

        public string personalWebsite { get; set; }

        public string skills { get; set; }

        public bool isEdcFamily { get; set; }

        public bool isPotentialStaffing { get; set; }

        public DateTimeOffset dateCreated { get; set; }

        public bool isDeleted { get; set; }

        public ContactViewModel()
        {
            contactId = new Guid();
            name = "";
            title = "";
            companyName = "";
            phone1 = "";
            phone2 = "";
            skypeId = "";
            email = "";
            companyTemp = "";
            resume = "";
            portfolio = "";
            personalWebsite = "";
            skills = "";
            isEdcFamily = false;
            isPotentialStaffing = false;
            dateCreated = DateTimeOffset.UtcNow;
            isDeleted = false;
        }

    }

    public class ContactAddEditDeleteViewModel
    {

        public Guid contactId { get; set; }

        public string name { get; set; }

        public string title { get; set; }

        public string companyName { get; set; }

        public string phone1 { get; set; }

        public string phone2 { get; set; }

        public string skypeId { get; set; }

        public string email { get; set; }

        public string companyTemp { get; set; }

        public string resume { get; set; }

        public string portfolio { get; set; }

        public string personalWebsite { get; set; }

        public string skills { get; set; }

        public bool isEdcFamily { get; set; }

        public bool isPotentialStaffing { get; set; }
        
        public bool isDeleted { get; set; }

        public ContactAddEditDeleteViewModel()
        {
            contactId = new Guid();
            name = "";
            title = "";
            companyName = "";
            phone1 = "";
            phone2 = "";
            skypeId = "";
            email = "";
            companyTemp = "";
            resume = "";
            portfolio = "";
            personalWebsite = "";
            skills = "";
            isEdcFamily = false;
            isPotentialStaffing = false;
            isDeleted = false;
        }

    }

    public class ContactUploadViewModel
    {

        public List<ContactViewModel> contactViewModels = new List<ContactViewModel>();

        public ContactUploadViewModel()
        {
            contactViewModels = new List<ContactViewModel>();
        }

    }

}