
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkplaceGroupAutomation.Models;
using WorkplaceGroupAutomation.Services;

namespace WorkplaceGroupAutomation
{
    
    class Program
    {
        #region Properties

        public static FacebookService facebookService = null;
        public static FacebookClient facebookClient = null;
        public static FacebookManagementService facebookManagementService = null;
        public static FacebookManagementClient facebookManagementClient = null;

        public static Community community = new Community();
        public static Models.Group communityGroups = new Models.Group();
        public static Member communityMembers = new Member();
        public static Member communityFormerMembers = new Member();
        public static List<GroupMembersExtract> groupMembers = new List<GroupMembersExtract>();
        public static List<GroupExtract> groupExtract = new List<GroupExtract>();
        public static UserAccount userAccounts = new UserAccount();
        public static List<UserAccountExtract> userAccountsExtract = new List<UserAccountExtract>();

        #endregion


        static void Main(string[] args)
        {
            InitialiseData();

            //********************************************************
            // My version is Table driven but this will all be done in
            // Memory.
            //********************************************************

            //Get User Accounts
            GetUserAccounts();
            userAccountsExtract = MapUserAccountsToExtract(userAccounts.Resources);

            //Get Groups
            groupExtract = GetGroups();

            //Get Group Members
            groupMembers = GetGroupMembers();

            //Match Group Name to the Division field in User Account
            //You can change this to another field
            List<GroupMembersExtract> GeneratedGroupMemberList = MatchUserAccountFieldToGroupName();
            
            //Return a list of Members and Groups that need added
            List<GroupMembersExtract> MissingGroupMemberList = GeneratedGroupMemberList.Where(p => !groupMembers.Any(p2 => p2.MemberId == p.MemberId && p2.GroupId == p.GroupId)).ToList();
            
            //Add Members that have a field (Division) matching to a group name but are missing 
            //that in their groups they are added to
            if (MissingGroupMemberList != null && MissingGroupMemberList.Count() > 0)
            {
                AddMembersToMissingGroups(MissingGroupMemberList);
            }
        }

        private static List<GroupMembersExtract> MatchUserAccountFieldToGroupName()
        {
            List<GroupMembersExtract> gMembers = new List<GroupMembersExtract>();
            try
            {
                foreach (var user in userAccountsExtract)
                {
                    try
                    {
                        var Group = groupExtract.Find(g => g.GroupName.ToLower() == user.Division.ToLower());

                        if (Group != null)
                        {
                            GroupMembersExtract groupMember = new GroupMembersExtract();
                            groupMember.GroupId = Group.GroupId;
                            groupMember.GroupName = Group.GroupName;
                            groupMember.MemberId = user.Id;
                            groupMember.MemberName = user.FormattedName;
                            gMembers.Add(groupMember);
                        }

                    }
                    catch
                    { }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("MatchUserAccountFieldToGroupName Error: {0}", ex.Message.ToString()));
            }
            return gMembers;
        }

        private static void AddMembersToMissingGroups(List<GroupMembersExtract> missingDivisionalGroup)
        {
            try
            {
                foreach (var record in missingDivisionalGroup)
                {
                    try
                    {
                        var result = facebookService.PostAddMemberToGroup(FacebookSettings.AccessToken, record.GroupId, record.MemberId);
                        Task.WaitAll(result);
                        var count = missingDivisionalGroup.IndexOf(record) + 1;
                        Console.Write("\r GroupMember Records Processed {0} of {1}                             ", count.ToString(), missingDivisionalGroup.Count());
                    }
                    catch
                    { }
                }
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(string.Format("AddMembersToMissingGroups Error: {0}", ex.Message.ToString()));
            }
        }

        private static List<GroupMembersExtract> GetGroupMembers()
        {
            List<GroupMembersExtract> groupMemberList = new List<GroupMembersExtract>();

            try
            {
                if (!(CheckCommunityGroups(communityGroups)))
                {
                    communityGroups = GetCommunityGroups(facebookService, community);
                }


                foreach (var group in communityGroups.data)
                {
                    var getGroupMembersTask = facebookService.GetMembersInGroupAsync(FacebookSettings.AccessToken, group.id);

                    groupMemberList = MapGroupMembers(group.id, group.name, getGroupMembersTask.Result, groupMemberList);
                    var count = communityGroups.data.IndexOf(group);
                    Console.Write("\r Groups Processed {0} of {1} || Member Count = {2}                       ", count.ToString(), communityGroups.data.Count(), groupMemberList.Count());

                }
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(string.Format("GetGroupMembers Error: {0}", ex.Message.ToString()));
            }

            return groupMemberList;
        }

        private static void GetUserAccounts()
        {
            var startIndex = 1;
            var recordsShown = 100;
            try
            {
                var getUsersAccounts = facebookManagementService.GetUserAccount(FacebookSettings.AccessToken, recordsShown, startIndex);

                userAccounts = UpdateUserAccounts(getUsersAccounts, userAccounts);

                while (startIndex < userAccounts.TotalResults)
                {
                    try
                    {
                        startIndex = startIndex + recordsShown;
                        getUsersAccounts = facebookManagementService.GetUserAccount(FacebookSettings.AccessToken, recordsShown, startIndex);
                        userAccounts = UpdateUserAccounts(getUsersAccounts, userAccounts);
                    }
                    catch //(Exception ex)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("GetUserAccounts Error: {0}", ex.Message.ToString()));
            }
        }

        private static UserAccount UpdateUserAccounts(UserAccount recentCall, UserAccount finalObject)
        {
            if (finalObject.Resources == null)
            {
                finalObject.Resources = new List<Resource>();
            }

            if (recentCall.StartIndex == 1)
            {
                finalObject.TotalResults = recentCall.TotalResults;
            }

            foreach (var res in recentCall.Resources)
            {
                finalObject.Resources.Add(res);
            }

            return finalObject;
        }

        private static void InitialiseData()
        {
            try
            {
                facebookClient = new FacebookClient();
                facebookService = new FacebookService(facebookClient);
                facebookManagementClient = new FacebookManagementClient();
                facebookManagementService = new FacebookManagementService(facebookManagementClient);
                var getCommunityTask = facebookService.GetCommunityAsync(FacebookSettings.AccessToken);
                Task.WaitAll(getCommunityTask);
                community = getCommunityTask.Result;
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(string.Format("InitaliseData Error: {0}", ex.Message.ToString()));
            }
        }

        private static List<GroupExtract> GetGroups()
        {
            List<GroupExtract> gExtract = new List<GroupExtract>();

            try
            { 
                if (!(CheckCommunityGroups(communityGroups)))
                {
                    communityGroups = GetCommunityGroups(facebookService, community);
                }
                gExtract = MapGroups(communityGroups);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("GetGroups Error: {0}", ex.Message.ToString()));
            }
            return gExtract;
        }

        private static Models.Group GetCommunityGroups(FacebookService facebookService, Community community)
        {
            var getGroupsTask = facebookService.GetGroupsInCommunityAsync(FacebookSettings.AccessToken, community.Id);

            Task.WaitAll(getGroupsTask);

            return getGroupsTask.Result;
        }

        private static bool CheckCommunityGroups(Models.Group communityGroups)
        {
            bool result = true;

            if (communityGroups != null && communityGroups.data != null && communityGroups.data.Count > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        private static List<GroupExtract> MapGroups(Models.Group communityGroups)
        {
            List<GroupExtract> groupList = new List<GroupExtract>();

            foreach (var g in communityGroups.data)
            {
                long CommunityID;
                Int64.TryParse(community.Id, out CommunityID);
                long GroupID;
                Int64.TryParse(g.id, out GroupID);

                GroupExtract gExtract = new GroupExtract
                {
                    CommunityId = CommunityID,
                    CommunityName = community.Name,
                    GroupId = GroupID,
                    GroupName = g.name
                };
                if (g.cover == null)
                {
                    gExtract.GroupCover = "";
                }
                else
                {
                    gExtract.GroupCover = FormatString(g.cover.source, false);
                }
                gExtract.GroupIcon = g.icon;
                gExtract.IsWorkplaceDefault = g.is_workplace_default;
                gExtract.GroupName = g.name;
                gExtract.GroupPrivacy = g.privacy.ToString();
                gExtract.GroupUpdatedTime = g.updated_time;
                gExtract.GroupArchived = g.archived;
                if (g.owner == null)
                {
                    gExtract.GroupOwnerID = null;
                    gExtract.GroupOwnerName = "";
                }
                else
                {
                    long MemberID;
                    Int64.TryParse(g.owner.id, out MemberID);
                    gExtract.GroupOwnerID = MemberID;
                    gExtract.GroupOwnerName = FormatString(g.owner.name, false);
                }
                gExtract.GroupDescription = g.description;

                groupList.Add(gExtract);

            }

            return groupList;
        }


        #region Data Mapping
        private static List<UserAccountExtract> MapUserAccountsToExtract(List<Resource> resources)
        {
            var result = new List<UserAccountExtract>();
            try
            {
                foreach (var line in resources)
                {
                    UserAccountExtract utf = new UserAccountExtract();
                    utf.Id = line.Id;
                    utf.ExternalId = FormatString(line.ExternalId ?? "");
                    utf.UserName = FormatString(line.UserName ?? "");
                    if (line.Name == null)
                    {
                        utf.FormattedName = "";
                        utf.FamilyName = "";
                        utf.GivenName = "";

                    }
                    else
                    {
                        utf.FormattedName = FormatString(line.Name.Formatted ?? "");
                        utf.FamilyName = FormatString(line.Name.FamilyName ?? "");
                        utf.GivenName = FormatString(line.Name.GivenName ?? "");

                    }

                    utf.Title = FormatString(line.Title ?? "");
                    if (line.Locale == null)
                    {
                        utf.Locale = "";
                    }
                    else
                    {
                        utf.Locale = FormatString(line.Locale.Value.ToString());
                    }
                    utf.Active = line.Active;
                    utf.Location = "";
                    if (line.Addresses != null && line.Addresses.Count > 0)
                    {
                        foreach (var add in line.Addresses)
                        {
                            if (add.Type == TypeEnum.Work || add.Primary == true)
                            {
                                utf.Location = FormatString(add.Formatted ?? "");
                            }
                        }
                    }
                    if (line.UrnScimSchemasExtensionEnterprise10 == null)
                    {

                        utf.Department = "";
                        utf.ManagerId = 0;
                        utf.Division = "";
                    }
                    else
                    {
                        utf.Department = FormatString(line.UrnScimSchemasExtensionEnterprise10.Department ?? "");
                        if (line.UrnScimSchemasExtensionEnterprise10.Manager == null)
                        {
                            utf.ManagerId = 0;
                        }
                        else
                        {
                            utf.ManagerId = line.UrnScimSchemasExtensionEnterprise10.Manager.ManagerId;
                        }
                        utf.Division = FormatString(line.UrnScimSchemasExtensionEnterprise10.Division ?? "");

                    }

                    if (line.UrnScimSchemasExtensionFacebookAccountstatusdetails10 == null)
                    {
                        utf.ClaimDate = null;
                        utf.Invited = null;
                        utf.InviteDate = null;
                        utf.Claimed = null;
                        utf.CanDelete = null;
                        utf.AccessCode = "";
                        utf.AccessCodeExpirationDate = null;
                    }
                    else
                    {
                        utf.ClaimDate = FormatDateFromLong(line.UrnScimSchemasExtensionFacebookAccountstatusdetails10.ClaimDate);
                        utf.Invited = line.UrnScimSchemasExtensionFacebookAccountstatusdetails10.Invited;
                        utf.InviteDate = FormatDateFromLong(line.UrnScimSchemasExtensionFacebookAccountstatusdetails10.InviteDate);
                        utf.Claimed = line.UrnScimSchemasExtensionFacebookAccountstatusdetails10.Claimed;
                        utf.CanDelete = line.UrnScimSchemasExtensionFacebookAccountstatusdetails10.CanDelete;
                        utf.AccessCode = FormatString(line.UrnScimSchemasExtensionFacebookAccountstatusdetails10.AccessCode ?? "");
                        utf.AccessCodeExpirationDate = FormatDateFromLong(line.UrnScimSchemasExtensionFacebookAccountstatusdetails10.AccessCodeExpirationDate);

                    }

                    if (line.UrnScimSchemasExtensionFacebookStarttermdates10 == null)
                    {
                        utf.StartDate = null;
                    }
                    else
                    {
                        utf.StartDate = FormatDateFromLong(line.UrnScimSchemasExtensionFacebookStarttermdates10.StartDate);

                    }

                    result.Add(utf);
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("MapUserAccountsToExtract Error: {0}", ex.Message.ToString()));
            }
            return result;
        }

        private static List<GroupMembersExtract> MapGroupMembers(string groupID, string groupName, Member members, List<GroupMembersExtract> memberList)
        {
            if (members.data == null)
            {
                return memberList;
            }
            foreach (var m in members.data)
            {
                long GroupID;
                Int64.TryParse(groupID, out GroupID);
                long MemberID;
                Int64.TryParse(m.id, out MemberID);

                GroupMembersExtract mExtract = new GroupMembersExtract
                {
                    GroupId = GroupID,
                    GroupName = groupName,
                    MemberId = MemberID,
                    MemberName = m.name
                };

                memberList.Add(mExtract);

            }

            return memberList;
        }



        #endregion


        #region Formatting
        private static string FormatString(string value)
        {
            var formattedString = "";
            var ListOfStrings = value.Split('|').ToList();
            var finalStringList = new List<string>();
            foreach (var val in ListOfStrings)
            {
                var item = "";

                if (val.Contains(',') || val.Contains('\n') || val.Contains('\r') || val.Contains(Environment.NewLine))
                {
                    item = '"' + val + '"';
                }
                else
                {
                    item = val;
                }
                finalStringList.Add(item);
            }
            formattedString = string.Join("|", finalStringList.Select(item => item));

            return formattedString;

        }

        private static string ReplaceNewlines(string blockOfText, string replaceWith)
        {
            return blockOfText.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
        }

        private static string FormatString(string inputString, bool removeSpaces)
        {
            var formattedString = "";

            if ((inputString ?? "") != "")
            {
                formattedString = inputString.Trim();

                if (removeSpaces)
                {
                    formattedString = new string(formattedString.ToCharArray()
                                                    .Where(c => !Char.IsWhiteSpace(c))
                                                    .ToArray());
                }

                if (formattedString.Contains(","))
                {
                    formattedString = formattedString.Replace(",", "");
                }
            }

            return formattedString;
        }

        
        private static DateTime? FormatDateFromLong(long? v)
        {
            DateTime? result = null;

            if (v == null || v <= 0)
            {
                return result;
            }
            else
            {
                double timestamp = v.Value;

                // Format our new DateTime object to start at the UNIX Epoch
                System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

                // Add the timestamp (number of seconds since the Epoch) to be converted
                dateTime = dateTime.AddSeconds(timestamp);

                result = dateTime;
            }

            return result;
        }

        #endregion


    }
}
