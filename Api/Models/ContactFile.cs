using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ContactFileViewModel
    {

        public Guid contactFileId { get; set; } = Guid.Empty;

        public Guid contactId { get; set; } = Guid.Empty;
        public ContactViewModel contact { get; set; } = new ContactViewModel();

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string path { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string originalFileName { get; set; } = "";

        public bool isDeleted { get; set; } = false;

    }
    
    public class ContactFileAddManyViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public List<ContactFileViewModel> contactFiles { get; set; } = new List<ContactFileViewModel>();
        
    }

}