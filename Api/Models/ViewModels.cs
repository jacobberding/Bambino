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
    
    public class GetByIdViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid id { get; set; }

    }

    public class SearchViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public int page { get; set; }

        public int records { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string search { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string sort { get; set; }

        public Guid id { get; set; }

        public SearchViewModel()
        {
            page = 0;
            records = 0;
            sort = "";
            id = Guid.Empty;
        }

    }

    public class ListViewModel
    {

        public string value { get; set; }

        public string name { get; set; }

    }

    public class AutomateViewModel
    {

        public string authenticationId { get; set; }

    }

}