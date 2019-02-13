using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{

    public class EmailViewModel
    {

        public Guid emailId { get; set; } = Guid.Empty;

        
        public string subject { get; set; } = "";

        
        public string body { get; set; } = "";

    }

}