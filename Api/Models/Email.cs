using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    [Table("Emails")]
    public class Email
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid emailId { get; set; }
        
        public string subject { get; set; }

        public string body { get; set; }
        
        public bool isDeleted { get; set; }

        public Email()
        {
            subject             = "";
            body                = "";
            isDeleted           = false;
        }

    }

    public class EmailViewModel
    {

        public Guid emailId { get; set; }
        
        public string subject { get; set; }

        public string body { get; set; }

    }

}