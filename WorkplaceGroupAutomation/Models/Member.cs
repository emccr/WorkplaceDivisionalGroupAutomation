using System;
using System.Collections.Generic;
using System.Text;

namespace WorkplaceGroupAutomation.Models
{
    public class Member
    {
        public List<MemberData> data { get; set; }
        public MemberPaging paging { get; set; }
    }

    public class MemberPictureData
    {
        public int height { get; set; }
        public bool is_silhouette { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class MemberPicture
    {
        public MemberPictureData data { get; set; }
    }

    public class MemberData
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string title { get; set; }
        public string department { get; set; }
        public string primary_phone { get; set; }
        public MemberPicture picture { get; set; }
        public string link { get; set; }
        public string locale { get; set; }
        public string name { get; set; }
        public string name_format { get; set; }
        public DateTime updated_time { get; set; }
        public DateTime account_invite_time { get; set; }
        public DateTime account_claim_time { get; set; }
        public string primary_address { get; set; }
    }

    public class MemberCursors
    {
        public string before { get; set; }
        public string after { get; set; }
    }

    public class MemberPaging
    {
        public MemberCursors cursors { get; set; }
        public string next { get; set; }
    }
    
}
