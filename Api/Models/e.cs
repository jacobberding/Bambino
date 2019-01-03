using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class e
    {
        
        public Guid mT { get; set; } //memberToken
        
        public Guid mCI { get; set; } //membercompanyId
        
        public bool mIC { get; set; } //memberIsContractor

        public bool mIE { get; set; } //memberIsEmployee

        public bool mIA { get; set; } //memberIsAdmin
        
        public bool mIS { get; set; } //memberIsSuperAdmin
        
        public string mE { get; set; } //memberEmail
        
        public string mFN { get; set; } //memberFirstName
        
        public string mLN { get; set; } //memberLastName
        
        public string mP { get; set; } //memberPhone
        
        public bool iV { get; set; } //isValidated
        
        public string v { get; set; } //view
        
        public e()
        {
            mT = Guid.Empty;
            mCI = Guid.Empty;
            mIC = false;
            mIE = false;
            mIA = false; 
            mIS = false; 
            mE = ""; 
            mFN = ""; 
            mLN = ""; 
            mP = ""; 
            iV = false; 
            v = "icons"; 
        }

    }
}