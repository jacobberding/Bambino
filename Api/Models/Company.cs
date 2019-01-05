using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    [Table("Companies")]
    public class Company
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid companyId { get; set; }
        
        public int type { get; set; }
        
        public int terms { get; set; }
        
        public string name { get; set; }

        public int pin { get; set; }

        public string email { get; set; }
        
        public string phone { get; set; }
        
        public string website { get; set; }
        
        public string billingAddressLine1 { get; set; }

        public string billingAddressLine2 { get; set; }

        public string billingCity { get; set; }

        public string billingState { get; set; }

        public string billingZip { get; set; }
        
        public string shippingAddressLine1 { get; set; }

        public string shippingAddressLine2 { get; set; }

        public string shippingCity { get; set; }

        public string shippingState { get; set; }

        public string shippingZip { get; set; }
        
        public bool isApproved { get; set; }

        public bool isDeleted { get; set; }
        
        public virtual ICollection<Member> members { get; set; }

        public Company()
        {
            type                    = 0;
            terms                   = 0;
            name                    = "";
            pin                     = 0;
            email                   = "";
            phone                   = "";
            website                 = "";
            billingAddressLine1     = "";
            billingAddressLine2     = "";
            billingCity             = "";
            billingState            = "";
            billingZip              = "";
            shippingAddressLine1    = "";
            shippingAddressLine2    = "";
            shippingCity            = "";
            shippingState           = "";
            shippingZip             = "";
            isApproved              = false;
            isDeleted               = false;
            members                 = new List<Member>();
        }

    }
    
    public class CompanyViewModel
    {

        public Guid companyId { get; set; }
        
        public int type { get; set; }
        
        public int terms { get; set; }
        
        public string name { get; set; }

        public int pin { get; set; }

        public string email { get; set; }
        
        public string phone { get; set; }
        
        public string website { get; set; }
        
        public string billingAddressLine1 { get; set; }

        public string billingAddressLine2 { get; set; }

        public string billingCity { get; set; }

        public string billingState { get; set; }

        public string billingZip { get; set; }
        
        public string shippingAddressLine1 { get; set; }

        public string shippingAddressLine2 { get; set; }

        public string shippingCity { get; set; }

        public string shippingState { get; set; }

        public string shippingZip { get; set; }
        
        public bool isApproved { get; set; }

        public bool isDeleted { get; set; }
        
        public List<MemberViewModel> members { get; set; }

        public CompanyViewModel()
        {
            companyId                   = Guid.Empty;
            type                        = 0;
            terms                       = 0;
            name                        = "";
            pin                         = 0;
            email                       = "";
            phone                       = "";
            website                     = "";
            billingAddressLine1         = "";
            billingAddressLine2         = "";
            billingCity                 = "";
            billingState                = "";
            billingZip                  = "";
            shippingAddressLine1        = "";
            shippingAddressLine2        = "";
            shippingCity                = "";
            shippingState               = "";
            shippingZip                 = "";
            isApproved                  = false;
            isDeleted                   = false;
            members                     = new List<MemberViewModel>();
        }

    }
    
    public class CompanyListViewModel
    {

        public Guid companyId { get; set; }
        
        public string name { get; set; }
        
        public CompanyListViewModel()
        {
            companyId                   = Guid.Empty;
            name                        = "";
        }

    }
    
    public class CompanyAddEditDeleteViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid companyId { get; set; }
        
        public int type { get; set; }
        
        public int terms { get; set; }
        
        public string name { get; set; }

        public int pin { get; set; }

        public string email { get; set; }
        
        public string phone { get; set; }
        
        public string website { get; set; }
        
        public string billingAddressLine1 { get; set; }

        public string billingAddressLine2 { get; set; }

        public string billingCity { get; set; }

        public string billingState { get; set; }

        public string billingZip { get; set; }
        
        public string shippingAddressLine1 { get; set; }

        public string shippingAddressLine2 { get; set; }

        public string shippingCity { get; set; }

        public string shippingState { get; set; }

        public string shippingZip { get; set; }
        
        public bool isApproved { get; set; }

        public bool isDeleted { get; set; }
        
        public CompanyAddEditDeleteViewModel()
        {
            companyId               = Guid.Empty;
            type                    = 0;
            terms                   = 0;
            name                    = "";
            pin                     = 0;
            email                   = "";
            phone                   = "";
            website                 = "";
            billingAddressLine1     = "";
            billingAddressLine2     = "";
            billingCity             = "";
            billingState            = "";
            billingZip              = "";
            shippingAddressLine1    = "";
            shippingAddressLine2    = "";
            shippingCity            = "";
            shippingState           = "";
            shippingZip             = "";
            isApproved              = false;
            isDeleted               = false;
        }

    }

    public class CompanyAddCardViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid companyId { get; set; }
        
        public string number { get; set; }
        
        public string expMonth { get; set; }
        
        public string expYear { get; set; }
        
        public string name { get; set; }
        
        public string cvc { get; set; }
        
        public string streetAddress { get; set; }
        
        public string city { get; set; }
        
        public string region { get; set; }
        
        public string postalCode { get; set; }
        
        public CompanyAddCardViewModel()
        {
            companyId       = Guid.Empty;
            number          = "";
            expMonth        = "";
            expYear         = "";
            name            = "";
            cvc             = "";
            streetAddress   = "";
            city            = "";
            region          = "";
            postalCode      = "";
        }

    }

    public class CompanyEditSettingsViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid companyId { get; set; }
        
        public int terms { get; set; }

        public int pin { get; set; }

        public CompanyEditSettingsViewModel()
        {
            companyId       = Guid.Empty;
            terms           = 0;
            pin             = 0;
        }

    }

    public class CompanyDeleteCardViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public Guid companyId { get; set; }
        
        public string cardId { get; set; }
        
        public CompanyDeleteCardViewModel()
        {
            companyId       = Guid.Empty;
            cardId          = "";
        }

    }

}