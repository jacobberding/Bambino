using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ProjectReferenceAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public long projectReferenceKey { get; set; } = 0;

        public long projectAttractionKey { get; set; } = 0;

        public long disciplineKey { get; set; } = 0;

        public string name { get; set; } = "";

        public string description { get; set; } = "";

        public string path { get; set; } = "";

        public string originalFileName { get; set; } = "";
        
        public bool isDeleted { get; set; } = false;

    }

}