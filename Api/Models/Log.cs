using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{

    public class LogViewModel
    {

        public Guid logId { get; set; } = Guid.Empty;

        public Guid memberId { get; set; } = Guid.Empty;
        public MemberViewModel member { get; set; } = new MemberViewModel();

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string activity { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string controllerName { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string methodName { get; set; } = "";

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string tableName { get; set; } = "";

        public Guid tableId { get; set; } = Guid.Empty;

        public DateTimeOffset createdDate { get; set; } = DateTimeOffset.UtcNow;
        
    }

    public class LogGetByTableNameViewModel
    {
        
        public AuthenticationViewModel authentication { get; set; }
        
        public int page { get; set; } = 0;

        public int records { get; set; } = 0;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string search { get; set; } = "";

        public string[] tableNames { get; set; } = new string[] { };
        
    }

}