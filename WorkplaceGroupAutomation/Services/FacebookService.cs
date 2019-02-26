using WorkplaceGroupAutomation.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WorkplaceGroupAutomation.Services
{
    public interface IFacebookService
    {
        Task<Community> GetCommunityAsync(string accessToken);
        Task<Group> GetGroupsInCommunityAsync(string accessToken, string CommunityID);
        Task<Member> GetFormerMembersInCommunityAsync(string accessToken, string CommunityID);
        Task<Member> GetMembersInCommunityAsync(string accessToken, string CommunityID);
        Task<Member> GetMembersInGroupAsync(string accessToken, string GroupID);
        Task PostAddMemberToGroup(string accessToken, long groupId, long memberId);
    }

    public class FacebookService : IFacebookService
    {
        private readonly IFacebookClient _facebookClient;

        public FacebookService(IFacebookClient facebookClient)
        {
            _facebookClient = facebookClient;
        }

        public async Task<Community> GetCommunityAsync(string accessToken)
        {
            Community result = await _facebookClient.GetAsync<Community>(
                accessToken, "community", "fields=id,name,privacy");

            if (result == null)
            {
                return new Community();
            }

            var communityObj = new Community
            {
                Id = result.Id,
                Name = result.Name,
                Privacy = result.Privacy
            };


            return communityObj;
        }

        public async Task<Group> GetGroupsInCommunityAsync(string accessToken, string CommunityID)
        {
            Group groups = new Group();
            try
            { 
                Group result = await _facebookClient.GetAsync<Group>(
                    accessToken, CommunityID+"/groups", "fields=id,cover,description,icon,is_workplace_default,name,owner,privacy,updated_time,archived&limit=400");

                if (result == null)
                {
                    return new Group();
                }

                groups = result;
            
                while (result.paging != null && result.paging.next != null )
                {
                    if(result.paging.next == "")
                    {
                        return groups;
                    }
                    result = await _facebookClient.GetAsyncDirectCall<Group>(accessToken,result.paging.next);

                    foreach (var r in result.data)
                    {
                        groups.data.Add(r);
                    }
                    if(result.paging.next == null)
                    {
                        return groups;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error on GetGroupsInCommunityAsync for " + CommunityID + " - " + ex.Message);
            }
            return groups;
        }


        public async Task<Member> GetMembersInCommunityAsync(string accessToken, string CommunityID)
        {
            Member members = new Member();
            try
            { 
                Member result = await _facebookClient.GetAsync<Member>(
                    accessToken, CommunityID + "/members", "fields=id,first_name,last_name,email,title,department,employee_number,primary_phone,primary_address,picture,link,locale,name,name_format,updated_time,account_invite_time,account_claim_time&limit=800");

                if (result == null)
                {
                    return new Member();
                }

                members = result;

                while (result.paging != null && result.paging.next != null )
                {
                    if(result.paging.next == "")
                    {
                        return members;
                    }
                    result = await _facebookClient.GetAsyncDirectCall<Member>(accessToken, result.paging.next);

                    foreach (var r in result.data)
                    {
                        members.data.Add(r);
                    }
                    if (result.paging.next == null)
                    {
                        return members;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error on GetMembersInCommunityAsync for " + CommunityID + " - " + ex.Message);
            }

            return members;
        }

        public async Task<Member> GetFormerMembersInCommunityAsync(string accessToken, string CommunityID)
        {
            Member members = new Member();
            try
            { 
                Member result = await _facebookClient.GetAsync<Member>(
                    accessToken, CommunityID + "/former_members", "fields=id,first_name,last_name,email,title,department,employee_number,primary_phone,primary_address,picture,link,locale,name,name_format,updated_time,account_invite_time,account_claim_time&limit=800");

                if (result == null)
                {
                    return new Member();
                }

                members = result;

                while (result.paging != null && result.paging.next != null)
                {
                    if (result.paging.next == "")
                    {
                        return members;
                    }
                    result = await _facebookClient.GetAsyncDirectCall<Member>(accessToken, result.paging.next);

                    foreach (var r in result.data)
                    {
                        members.data.Add(r);
                    }
                    if (result.paging.next == null)
                    {
                        return members;
                    }
                }
            }
                catch (Exception ex)
                {
                    throw new Exception("Error on GetFormerMembersInCommunityAsync for " + CommunityID + " - " + ex.Message);
            }
            return members;
        }


        public async Task<Member> GetMembersInGroupAsync(string accessToken, string GroupID)
        {
            Member members = new Member();
            try
            { 
                Member result = await _facebookClient.GetAsync<Member>(
                    accessToken, GroupID + "/members", "fields=id,first_name,last_name,email,title,department,employee_number,primary_phone,primary_address,picture,link,locale,name,name_format,updated_time,account_invite_time,account_claim_time&limit=800");

                if (result == null)
                {
                    return new Member();
                }

                members = result;

                while (result.paging != null && result.paging.next != null)
                {
                    if (result.paging.next == "")
                    {
                        return members;
                    }
                    result = await _facebookClient.GetAsyncDirectCall<Member>(accessToken, result.paging.next);

                    foreach (var r in result.data)
                    {
                        members.data.Add(r);
                    }
                    if (result.paging.next == null)
                    {
                        return members;
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error on GetMembersInGroupAsync for " + GroupID + " - " + ex.Message);
            }

            return members;
        }
        

        public async Task PostAddMemberToGroup(string accessToken, long groupId, long memberId)
        {
            try
            {
                await _facebookClient.PostCallAsync(accessToken, groupId + "/members/" + memberId);
                
            }
            catch
            {

            }
        }
    }  

}