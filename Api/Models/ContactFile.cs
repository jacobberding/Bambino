using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("ContactFiles")]
    public class ContactFile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid contactFileId { get; set; }

        [ForeignKey("contact")]
        public Guid contactId { get; set; } = Guid.Empty;
        public virtual Contact contact { get; set; }

        public string name { get; set; } = "";

        public string path { get; set; } = "";

        public string originalFileName { get; set; } = "";

        public bool isDeleted { get; set; } = false;

    }

    public class ContactFileViewModel
    {

        public Guid contactFileId { get; set; } = Guid.Empty;

        public Guid contactId { get; set; } = Guid.Empty;
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

}