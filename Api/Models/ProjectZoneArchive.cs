using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class ProjectZoneArchiveViewModel
    {

        public long projectZoneArchiveKey { get; set; } = 0;

        public long projectZoneKey { get; set; } = 0;
        public ProjectZoneViewModel projectZone { get; set; }

        public DateTimeOffset dateStart { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateEnd { get; set; } = DateTimeOffset.MinValue;
        
        public Guid memberStartId { get; set; } = Guid.Empty;
        public MemberViewModel memberStart { get; set; }
        
        public Guid memberEndId { get; set; } = Guid.Empty;
        public MemberViewModel memberEnd { get; set; }

    }

}