using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WorkplaceGroupAutomation.Services
{
    public interface IFacebookManagementClient
    {
        Task<T> GetAsync<T>(string accessToken, string endpoint, string args = null);
        T GetSync<T>(string accessToken, string endpoint, string args = null);
        T GetSyncDirectCall<T>(string accessToken, string path);
        Task<T> GetAsyncDirectCall<T>(string accessToken, string path);
        Task PostAsync(string accessToken, string endpoint, object data, string args = null);
    }

    public class FacebookManagementClient : IFacebookManagementClient
    {
        private readonly HttpClient _httpClient;

        public FacebookManagementClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.facebook.com/scim/v1")

            };
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<T> GetAsync<T>(string accessToken, string endpoint, string args = null)
        {

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var query = "";
            if (args != null)
            {
                query = "?" + args;
            }

            var response = await _httpClient.GetAsync($"{endpoint}" + query);
            //var response =  _httpClient.GetAsync($"{endpoint}" + query);
            if (!response.IsSuccessStatusCode)
                return default(T);

            var result = await response.Content.ReadAsStreamAsync();//.ReadAsStringAsync();
            var resultStr = "";
            using (StreamReader reader = new StreamReader(result, Encoding.UTF8))
            {
                resultStr = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<T>(resultStr);
        }



        public T GetSyncDirectCall<T>(string accessToken, string path)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //HttpResponseMessage response = client.GetStreamAsync(path);
            var response = client.GetStreamAsync(path);

            var result = response.Result;

            using (var reader = new StreamReader(result))
            {
                var responseFromServer = reader.ReadToEnd();
                var converter = new ExpandoObjectConverter();
                return JsonConvert.DeserializeObject<T>(responseFromServer, converter);

            }

        }

        public async Task<T> GetAsyncDirectCall<T>(string accessToken, string path)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await client.GetAsync(path);

            if (!response.IsSuccessStatusCode)
                return default(T);

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(result);

        }
        

        public T GetSync<T>(string accessToken, string endpoint, string query)
        {

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = _httpClient.GetStreamAsync($"{endpoint}" + query);

            var result = response.Result;

            using (var reader = new StreamReader(result))
            {
                var responseFromServer = reader.ReadToEnd();
                var converter = new ExpandoObjectConverter();
                return JsonConvert.DeserializeObject<T>(responseFromServer, converter);

            }

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