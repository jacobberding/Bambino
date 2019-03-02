using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{

    public class EmailViewModel
    {

        public long emailKey { get; set; } = 0;
        
        public string subject { get; set; } = "";
        
        public string body { get; set; } = "";

    }

}