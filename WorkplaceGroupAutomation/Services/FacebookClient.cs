using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WorkplaceGroupAutomation.Services
{
    public interface IFacebookClient
    {
        Task<T> GetAsync<T>(string accessToken, string endpoint, string args = null);
        Task<T> GetAsyncDirectCall<T>(string accessToken, string path);
        Task PostAsync(string accessToken, string endpoint, object data, string args = null);
        Task PostCallAsync(string accessToken, string endpoint);
    }

    public class FacebookClient : IFacebookClient
    {
        private readonly HttpClient _httpClient;

        public FacebookClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.facebook.com")

        };
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T> GetAsync<T>(string accessToken, string endpoint, string args = null)
        {

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var query = "";
            if(args != null)
            {
                query = "?" + args;
            }
            var response = await _httpClient.GetAsync($"{endpoint}"+ query);
            if (!response.IsSuccessStatusCode)
                return default(T);

            var result = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<T>(result);
        }


        public async Task<T> GetAsyncDirectCall<T>(string accessToken, string path)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var req_path = path.Replace("limit=25", "limit=300");
            HttpResponseMessage response = await client.GetAsync(req_path);

            if (!response.IsSuccessStatusCode)
                return default(T);

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(result);
            
        }

        //public async Task PostCallOnlyAsync(string accessToken, string endpoint)
        //{
        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //    await _httpClient.PostAsync($"{endpoint}");
        //}

        public async Task PostCallAsync(string accessToken, string endpoint)
        {
            var mediaType = new MediaTypeHeaderValue("application/json");
            var jsonSerializerSettings = new JsonSerializerSettings();
            //var jsonFormatter = new JsonNetFormatter(jsonSerializerSettings);
            var requestMessage = new HttpRequestMessage();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var result = await _httpClient.PostAsync($"{endpoint}", requestMessage.Content);
            //return result;
        }

        public async Task PostAsync(string accessToken, string endpoint, object data, string args = null)
        {
            var payload = GetPayload(data);
            await _httpClient.PostAsync($"{endpoint}?access_token={accessToken}&{args}", payload);
        }

        private static StringContent GetPayload(object data)
        {
            var json = JsonConvert.SerializeObject(data);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}