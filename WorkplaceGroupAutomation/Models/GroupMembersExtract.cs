using System;
using System.Collections.Generic;
using System.Text;

namespace WorkplaceGroupAutomation.Models
{
    public class GroupMembersExtract
    {
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public long MemberId { get; set; }
        public string MemberName { get; set; }
    }
}
