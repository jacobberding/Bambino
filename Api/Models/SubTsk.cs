using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("SubTsks")]
    public class SubTsk
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid subTskId { get; set; }
        
        [ForeignKey("member")]
        public Guid memberId { get; set; } = Guid.Empty;
        public virtual Member member { get; set; }

        [ForeignKey("memberCreated")]
        public Guid memberIdCreated { get; set; } = Guid.Empty;
        public virtual Member memberCreated { get; set; }

        public string name { get; set; } = "";

        public string description { get; set; } = "";
        
        public DateTimeOffset dateDue { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateOriginalDue { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateCompleted { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isCompleted { get; set; } = false;
        
        public bool isDeleted { get; set; } = false;
        
    }

    public class SubTskViewModel
    {

        public Guid subTskId { get; set; } = Guid.Empty;

        public Guid memberId { get; set; } = Guid.Empty;
        public MemberViewModel member { get; set; } = new MemberViewModel();

        public Guid memberIdCreated { get; set; } = Guid.Empty;
        public MemberViewModel memberCreated { get; set; } = new MemberViewModel();

        public string name { get; set; } = "";

        public string description { get; set; } = "";

        public DateTimeOffset dateDue { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateOriginalDue { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateCompleted { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isCompleted { get; set; } = false;

        public bool isDeleted { get; set; } = false;

    }

}