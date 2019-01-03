using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    [Table("Logs")]
    public class Log
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid logId { get; set; }
        
        [ForeignKey("member")]
        public Guid memberId { get; set; }
        public virtual Member member { get; set; }
        
        public string activity { get; set; }

        public string controllerName { get; set; }
        
        public string methodName { get; set; }

        public string tableName { get; set; }

        public Guid tableId { get; set; }
        
        public DateTimeOffset createdDate { get; set; }

        public Log()
        {
            memberId            = Guid.Empty;
            activity            = "";
            controllerName      = "";
            methodName          = "";
            tableName           = "";
            tableId             = Guid.Empty;
            createdDate         = DateTimeOffset.UtcNow;
        }

    }
    
    public class LogViewModel
    {

        public Guid logId { get; set; }
        
        public Guid memberId { get; set; }
        public MemberViewModel member { get; set; }
        
        public string activity { get; set; }

        public string controllerName { get; set; }
        
        public string methodName { get; set; }

        public string tableName { get; set; }

        public Guid tableId { get; set; }
        
        public DateTimeOffset createdDate { get; set; }

        public LogViewModel()
        {
            logId               = Guid.Empty;
            memberId            = Guid.Empty;
            member              = new MemberViewModel();
            activity            = "";
            controllerName      = "";
            methodName          = "";
            tableName           = "";
            tableId             = Guid.Empty;
            createdDate         = DateTimeOffset.UtcNow;
        }

    }

    public class LogGetByTableNameViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public int page { get; set; }

        public int records { get; set; }
        
        public string search { get; set; }

        public string[] tableNames { get; set; }

        public LogGetByTableNameViewModel()
        {
            page        = 0;
            records     = 0;
            search      = "";
            tableNames  = new string[] { };
        }

    }

}