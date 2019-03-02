using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class SubTskViewModel
    {

        public long subTskId { get; set; } = 0;

        public long tskKey { get; set; } = 0;
        public TskViewModel tsk { get; set; } = new TskViewModel();

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