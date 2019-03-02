using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class TskViewModel
    {

        public long tskId { get; set; } = 0;

        public Guid projectId { get; set; } = Guid.Empty;
        public ProjectViewModel project { get; set; } = new ProjectViewModel();
        
        public Guid memberId { get; set; } = Guid.Empty;
        public MemberViewModel member { get; set; } = new MemberViewModel();
        
        public Guid memberIdCreated { get; set; } = Guid.Empty;
        public MemberViewModel memberCreated { get; set; } = new MemberViewModel();
        
        public string name { get; set; } = "";
        
        public string description { get; set; } = "";

        public int numOfSubTsks { get; set; } = 0;

        public int numOfSubTsksCompleted { get; set; } = 0;

        public DateTimeOffset dateDue { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateOriginalDue { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateCompleted { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public bool isCompleted { get; set; } = false;

        public bool isPersonal { get; set; } = false;

        public bool isInOffice { get; set; } = false;

        public bool isProject { get; set; } = false;

        public bool isDeleted { get; set; } = false;

        public List<SubTskViewModel> subTsks { get; set; } = new List<SubTskViewModel>();

    }

}