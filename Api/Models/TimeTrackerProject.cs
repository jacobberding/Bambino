﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [Table("TimeTrackerProjects")]
    public class TimeTrackerProject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid timeTrackerProjectId { get; set; }

        [ForeignKey("timeTracker")]
        public Guid timeTrackerId { get; set; } = Guid.Empty;
        public virtual TimeTracker timeTracker { get; set; }

        [ForeignKey("project")]
        public Guid projectId { get; set; } = Guid.Empty;
        public virtual Project project { get; set; }

        public string description { get; set; } = "";

        public decimal totalHours { get; set; } = 0.0000m;

        public bool isDeleted { get; set; } = false;
        
    }

    public class TimeTrackerProjectViewModel
    {

        public Guid timeTrackerProjectId { get; set; } = Guid.Empty;

        public Guid timeTrackerId { get; set; } = Guid.Empty;
        public TimeTrackerViewModel timeTracker { get; set; } = new TimeTrackerViewModel();
        
        public Guid projectId { get; set; } = Guid.Empty;
        public ProjectViewModel project { get; set; } = new ProjectViewModel();

        public string description { get; set; } = "";

        public decimal totalHours { get; set; } = 0.0000m;

        public bool isDeleted { get; set; } = false;

    }

}