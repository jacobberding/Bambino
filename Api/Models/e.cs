using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class e
    {
        
        public Guid mT { get; set; } = Guid.Empty; //memberToken

        public Guid mCI { get; set; } = Guid.Empty; //membercompanyId

        public bool mIC { get; set; } = false; //memberIsContractor

        public bool mIE { get; set; } = false; //memberIsEmployee

        public bool mIM { get; set; } = false; //memberIsManager

        public bool mIA { get; set; } = false; //memberIsAdmin

        public bool mIS { get; set; } = false; //memberIsSuperAdmin

        public string mE { get; set; } = ""; //memberEmail

        public string mFN { get; set; } = ""; //memberFirstName

        public string mLN { get; set; } = ""; //memberLastName

        public string mP { get; set; } = ""; //memberPhone

        public string mPA { get; set; } = ""; //memberPath

        public bool iV { get; set; } = false; //isValidated

        public string v { get; set; } = "icons";  //view
        
    }
}