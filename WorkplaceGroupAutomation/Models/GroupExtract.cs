using System;
using System.Collections.Generic;
using System.Text;

namespace WorkplaceGroupAutomation.Models
{
    public class GroupExtract
    {
        public long GroupId { get; set; }
        public string GroupCover { get; set; }
        public string GroupIcon { get; set; }
        public bool IsWorkplaceDefault { get; set; }
        public string GroupName { get; set; }
        public string GroupPrivacy { get; set; }
        public DateTime GroupUpdatedTime { get; set; }
        public bool GroupArchived { get; set; }
        public long? GroupOwnerID { get; set; }
        public string GroupOwnerName { get; set; }
        public string GroupDescription { get; set; }
        public long CommunityId { get; set; }
        public string CommunityName { get; set; }

    }
}
