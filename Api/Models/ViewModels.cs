using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ViewModels
    {
        
    }

    public class Empty
    {

    }

    public class AddEditDeleteReturnViewModel
    {
        public Guid id { get; set; }
        public string state { get; set; }
    }

    public class GetByIdViewModel
    {
        public Guid id { get; set; }
    }

    public class Search
    {

        //public AuthenticationViewModel authentication { get; set; }

        public int page { get; set; }

        public int records { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string search { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string sort { get; set; }

        public Guid id { get; set; }

        public Search()
        {
            page = 0;
            records = 0;
            sort = "";
            id = Guid.Empty;
        }

    }

}