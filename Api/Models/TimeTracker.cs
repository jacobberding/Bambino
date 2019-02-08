using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{

    public class TimeTrackerViewModel
    {

        public Guid timeTrackerId { get; set; } = Guid.Empty;
        
        public Guid memberId { get; set; } = Guid.Empty;
        public MemberViewModel member { get; set; } = new MemberViewModel();

        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateIn { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateOut { get; set; } = DateTimeOffset.MinValue;

        public decimal totalHours { get; set; } = 0.0000m;

        public bool isActive { get; set; } = true;

        public bool isDeleted { get; set; } = false;

        public List<TimeTrackerProjectViewModel> timeTrackerProjects { get; set; } = new List<TimeTrackerProjectViewModel>();

    }

    public class TimeTrackerAddEditDeleteViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public Guid timeTrackerId { get; set; } = Guid.Empty;

        public DateTimeOffset dateIn { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset dateOut { get; set; } = DateTimeOffset.MinValue;

        public decimal totalHours { get; set; } = 0.0000m;

        public bool isDeleted { get; set; } = false;

        public List<TimeTrackerProjectViewModel> projects { get; set; } = new List<TimeTrackerProjectViewModel>();

    }

    public class TimeTrackerOutProjectViewModel
    {

        public Guid projectId { get; set; } = Guid.Empty;

        public decimal totalHours { get; set; } = 0.0000m;

    }

    public class TimeTrackerOutViewModel
    {

        public AuthenticationViewModel authentication { get; set; }

        public decimal totalHours { get; set; } = 0.0000m;

        public List<TimeTrackerOutProjectViewModel> projects { get; set; } = new List<TimeTrackerOutProjectViewModel>();

    }

}