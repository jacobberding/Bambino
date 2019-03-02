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

        public long contactFileKey { get; set; } = 0;

        public long contactKey { get; set; } = 0;
        public ContactViewModel contact { get; set; } = new ContactViewModel();
        
        public string name { get; set; } = "";
        
        public string path { get; set; } = "";
        
        public string originalFileName { get; set; } = "";

        public bool isDeleted { get; set; } = false;

    }
    
    public class ContactFileAddManyViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public List<ContactFileViewModel> contactFiles { get; set; } = new List<ContactFileViewModel>();
        
    }

    public class ContactFileDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public long contactFileKey { get; set; } = 0;

        public bool isDeleted { get; set; } = false;

    }

}