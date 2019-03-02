using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{

    public class LogViewModel
    {

        public long logKey { get; set; } = 0;

        public Guid memberId { get; set; } = Guid.Empty;
        public MemberViewModel member { get; set; } = new MemberViewModel();
        
        public string activity { get; set; } = "";
        
        public string controllerName { get; set; } = "";
        
        public string methodName { get; set; } = "";
        
        public string tableName { get; set; } = "";

        public Guid tableId { get; set; } = Guid.Empty;

        public long tableKey { get; set; } = 0;

        public DateTimeOffset createdDate { get; set; } = DateTimeOffset.UtcNow;
        
    }

    public class LogGetByTableNameViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public int page { get; set; } = 0;

        public int records { get; set; } = 0;
        
        public string search { get; set; } = "";

        public string[] tableNames { get; set; } = new string[] { };
        
    }

}