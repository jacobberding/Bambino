using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Api.Models
{
    
    public class EmptyViewModel
    {

    }

    public class EmptyAuthenticationViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

    }

    public class AddEditDeleteReturnViewModel
    {
    
        public Guid id { get; set; }

        public string state { get; set; }

    }

    public class AddDeleteManyToManyViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid tableId { get; set; }

        public Guid manyId { get; set; }

        public string name { get; set; }

    }

    public class GetByIdViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid id { get; set; }

    }

    public class SearchViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public int page { get; set; } = 0;

        public int records { get; set; } = 0;

        
        public string search { get; set; } = "";

        
        public string sort { get; set; } = "";

        public Guid id { get; set; } = Guid.Empty;

        public Guid projectPhaseId { get; set; } = Guid.Empty;
        
    }

    public class ListViewModel
    {

        public string value { get; set; }

        public string name { get; set; }

    }

    public class DateTimeOffsetViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public DateTimeOffset startDate { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset endDate { get; set; } = DateTimeOffset.UtcNow;

    }

    public class AutomateViewModel
    {

        public string authenticationId { get; set; }

    }

    public class Upload
    {

        public Guid id { get; set; }

        
        public string path { get; set; } = "";

        
        public string originalFileName { get; set; } = "";

        public int width { get; set; } = 0;

        public int height { get; set; } = 0;

        public int resolutionHorizontal { get; set; } = 0;

        public int resolutionVertical { get; set; } = 0;

        public int contentLength { get; set; } = 0;

        public int type { get; set; } = 0;

    }

}