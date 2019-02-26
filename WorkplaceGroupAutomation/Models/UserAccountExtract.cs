
using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WorkplaceGroupAutomation.Models
{
    public class UserAccountExtract
    {
        public long Id { get; set; }
        public string ExternalId { get; set; }
        public string UserName { get; set; }
        public string FormattedName { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string Title { get; set; }
        public string Locale { get; set; }
        public bool? Active { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public long ManagerId { get; set; }
        public string Division { get; set; }
        public DateTime? ClaimDate { get; set; }
        public bool? Invited { get; set; }
        public DateTime? InviteDate { get; set; }
        public bool? Claimed { get; set; }
        public bool? CanDelete { get; set; }
        public string AccessCode { get; set; }
        public DateTime? AccessCodeExpirationDate { get; set; }
        public DateTime? StartDate { get; set; }

    }

}

