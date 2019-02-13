using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    
    public class CompanyViewModel
    {

        public Guid companyId { get; set; } = Guid.Empty;

        public int type { get; set; } = 0;

        public int terms { get; set; } = 0;

        
        public string name { get; set; } = "";

        public int pin { get; set; } = 0;

        
        public string email { get; set; } = "";

        
        public string phone { get; set; } = "";

        
        public string website { get; set; } = "";

        
        public string billingAddressLine1 { get; set; } = "";

        
        public string billingAddressLine2 { get; set; } = "";

        
        public string billingCity { get; set; } = "";

        
        public string billingState { get; set; } = "";

        
        public string billingZip { get; set; } = "";

        
        public string shippingAddressLine1 { get; set; } = "";

        
        public string shippingAddressLine2 { get; set; } = "";

        
        public string shippingCity { get; set; } = "";

        
        public string shippingState { get; set; } = "";

        
        public string shippingZip { get; set; } = "";

        public bool isApproved { get; set; } = false;

        public bool isDeleted { get; set; } = false;

        public List<MemberViewModel> members { get; set; } = new List<MemberViewModel>();
        
    }
    
    public class CompanyListViewModel
    {

        public Guid companyId { get; set; } = Guid.Empty;

        
        public string name { get; set; } = "";
        
    }
    
    public class CompanyAddEditDeleteViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid companyId { get; set; } = Guid.Empty;

        public int type { get; set; } = 0;

        public int terms { get; set; } = 0;

        
        public string name { get; set; } = "";

        public int pin { get; set; } = 0;

        
        public string email { get; set; } = "";

        
        public string phone { get; set; } = "";

        
        public string website { get; set; } = "";

        
        public string billingAddressLine1 { get; set; } = "";

        
        public string billingAddressLine2 { get; set; } = "";

        
        public string billingCity { get; set; } = "";

        
        public string billingState { get; set; } = "";

        
        public string billingZip { get; set; } = "";

        
        public string shippingAddressLine1 { get; set; } = "";

        
        public string shippingAddressLine2 { get; set; } = "";

        
        public string shippingCity { get; set; } = "";

        
        public string shippingState { get; set; } = "";

        
        public string shippingZip { get; set; } = "";

        public bool isApproved { get; set; } = false;

        public bool isDeleted { get; set; } = false;
        
    }

    public class CompanyAddCardViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid companyId { get; set; } = Guid.Empty;

        
        public string number { get; set; } = "";

        
        public string expMonth { get; set; } = "";

        
        public string expYear { get; set; } = "";

        
        public string name { get; set; } = "";

        
        public string cvc { get; set; } = "";

        
        public string streetAddress { get; set; } = "";

        
        public string city { get; set; } = "";

        
        public string region { get; set; } = "";

        
        public string postalCode { get; set; } = "";
        
    }

    public class CompanyEditSettingsViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid companyId { get; set; } = Guid.Empty;

        public int terms { get; set; } = 0;

        public int pin { get; set; } = 0;
        
    }

    public class CompanyDeleteCardViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid companyId { get; set; } = Guid.Empty;

        
        public string cardId { get; set; } = "";
        
    }

}