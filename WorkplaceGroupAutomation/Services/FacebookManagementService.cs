using WorkplaceGroupAutomation.Data;
using WorkplaceGroupAutomation.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WorkplaceGroupAutomation.Services
{
    public interface IFacebookManagementService
    {
        UserAccount GetUserAccount(string accessToken, int count, int startIndex);
        string PostUserJSON(string accessToken, string json);
        Resource GetUserAccountByID(string accessToken, long userID);
        string GetUserResponseByID(string accessToken, long userID);
        UserAccount PostUserAccount(string accessToken, UserAccount user);
        void DeleteUserAccount(string accessToken, long userID);
    }

    public class FacebookManagementService : IFacebookManagementService
    {
        private readonly IFacebookManagementClient _facebookClient;

        public FacebookManagementService(IFacebookManagementClient facebookClient)
        {
            _facebookClient = facebookClient;
        }

        public UserAccount GetUserAccount(string token, int count, int startIndex)
        {
            var result = new UserAccount();

            var client = new RestClient("https://www.facebook.com/scim/v1/Users?count=" + count + "&startIndex=" + startIndex);
            var request = new RestRequest(Method.GET);

            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "Bearer " + token);

            var response = client.Execute(request);

            result = UserAccount.FromJson(response.Content);
            //result = JsonConvert.DeserializeObject<UserAccount>(response.Content);

            return result;
        }
        public Resource GetUserAccountByID(string token, long userID)
        {
            var result = new Resource();

            var client = new RestClient("https://www.facebook.com/scim/v1/Users/" + userID);
            var request = new RestRequest(Method.GET);

            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "Bearer " + token);

            var response = client.Execute(request);

            //result = UserAccount.FromJson(response.Content);
            result = JsonConvert.DeserializeObject<Resource>(response.Content);

            return result;
        }

        public string GetUserResponseByID(string token, long userID)
        {
            // var result = new Resource();

            var client = new RestClient("https://www.facebook.com/scim/v1/Users/" + userID);
            var request = new RestRequest(Method.GET);

            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "Bearer " + token);

            var response = client.Execute(request);

            //result = UserAccount.FromJson(response.Content);
            //result = JsonConvert.DeserializeObject<Resource>(response.Content);

            return response.Content;
        }

        public UserAccount PostUserAccount(string accessToken, UserAccount user)
        {
            var result = new UserAccount();

            var client = new RestClient("https://www.facebook.com/scim/v1/Users");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "Bearer " + accessToken);

            request.AddParameter("undefined", Serialize.ToJson(user), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            result = UserAccount.FromJson(response.Content);

            return result;
        }

        public string PostUserJSON(string accessToken, string json)
        {
            var result = "";

            var client = new RestClient("https://www.facebook.com/scim/v1/Users");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "Bearer " + accessToken);

            request.AddParameter("undefined", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            result = response.Content;

            return result;
        }

        public void DeleteUserAccount(string accessToken, long userID)
        {
            try
            {
                var client = new RestClient("https://www.facebook.com/scim/v1/Users/" + userID);
                var request = new RestRequest(Method.DELETE);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("authorization", "Bearer " + accessToken);
                IRestResponse response = client.Execute(request);
            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

    }
}