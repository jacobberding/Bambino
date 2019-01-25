using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("Tsks")]
    public class Tsk
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid tskId { get; set; }

        [ForeignKey("project")]
        public Guid projectId { get; set; } = Guid.Empty;
        public virtual Project project { get; set; }

        [ForeignKey("member")]
        public Guid memberId { get; set; } = Guid.Empty;
        public virtual Member member { get; set; }

        [ForeignKey("memberCreated")]
        public Guid memberIdCreated { get; set; } = Guid.Empty;
        public virtual Member memberCreated { get; set; }

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

        public virtual ICollection<SubTsk> subTsks { get; set; } = new List<SubTsk>();

    }

    public class TskViewModel
    {

        public Guid tskId { get; set; } = Guid.Empty;

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