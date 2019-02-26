using System;
using System.Collections.Generic;
using System.Text;

namespace WorkplaceGroupAutomation.Models
{
    public class Group
    {
        public List<GroupData> data { get; set; }
        public GroupPaging paging { get; set; }
    }
    public class GroupCover
    {
        public string cover_id { get; set; }
        public int offset_x { get; set; }
        public int offset_y { get; set; }
        public string source { get; set; }
        public string id { get; set; }
    }

    public class GroupOwner
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class GroupData
    {
        public string id { get; set; }
        public GroupCover cover { get; set; }
        public string icon { get; set; }
        public bool is_workplace_default { get; set; }
        public string name { get; set; }
        public string privacy { get; set; }
        public DateTime updated_time { get; set; }
        public bool archived { get; set; }
        public GroupOwner owner { get; set; }
        public string description { get; set; }
    }

    public class GroupCursors
    {
        public string before { get; set; }
        public string after { get; set; }
    }

    public class GroupPaging
    {
        public GroupCursors cursors { get; set; }
        public string next { get; set; }
    }


}
