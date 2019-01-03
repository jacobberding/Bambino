using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("MemberIpAddresses")]
    public class MemberIpAddress
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid memberIpAddressId { get; set; }
        
        [ForeignKey("member")]
        public Guid memberId { get; set; }
        public virtual Member member { get; set; }
        
        public string email { get; set; }

        public string ipAddress { get; set; }
        
        public byte[] password { get; set; }
        
        public byte[] keyValue { get; set; }
        
        public byte[] iVValue { get; set; }
        
        public DateTimeOffset createdDate { get; set; }

        public bool isSuccess { get; set; }

        public MemberIpAddress()
        {
            memberId                = Guid.Empty;
            email                   = "";
            ipAddress               = "";
            password                = new byte[] { };
            keyValue                = new byte[] { };
            iVValue                 = new byte[] { };
            createdDate             = DateTimeOffset.UtcNow;
            isSuccess               = false;
        }

    }
}