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

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        public int pin { get; set; } = 0;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string email { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string phone { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string website { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string billingAddressLine1 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string billingAddressLine2 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string billingCity { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string billingState { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string billingZip { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string shippingAddressLine1 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string shippingAddressLine2 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string shippingCity { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string shippingState { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string shippingZip { get; set; } = "";

        public bool isApproved { get; set; } = false;

        public bool isDeleted { get; set; } = false;

        public List<MemberViewModel> members { get; set; } = new List<MemberViewModel>();
        
    }
    
    public class CompanyListViewModel
    {

        public Guid companyId { get; set; } = Guid.Empty;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";
        
    }
    
    public class CompanyAddEditDeleteViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid companyId { get; set; } = Guid.Empty;

        public int type { get; set; } = 0;

        public int terms { get; set; } = 0;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        public int pin { get; set; } = 0;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string email { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string phone { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string website { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string billingAddressLine1 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string billingAddressLine2 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string billingCity { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string billingState { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string billingZip { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string shippingAddressLine1 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string shippingAddressLine2 { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string shippingCity { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string shippingState { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string shippingZip { get; set; } = "";

        public bool isApproved { get; set; } = false;

        public bool isDeleted { get; set; } = false;
        
    }

    public class CompanyAddCardViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid companyId { get; set; } = Guid.Empty;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string number { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string expMonth { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string expYear { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string cvc { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string streetAddress { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string city { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string region { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
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

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string cardId { get; set; } = "";
        
    }

}