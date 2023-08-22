using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Payment_Gateway.BLL.Paystack.Interfaces;
using System.Net.Http.Headers;
using System.Text;

namespace Payment_Gateway.BLL.Infrastructure.Paystack
{
    public class PaystackPostRequest : IPaystackPostRequest
    {
        private readonly HttpClient _httpClient;
        private readonly PaystackConfig _paystackConfig;


        public PaystackPostRequest(PaystackConfig paystackConfig)
        {
            _httpClient = new HttpClient();
            _paystackConfig = paystackConfig;
        }


        public async Task<HttpResponseMessage> GetRequest(string apiUrl)
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _paystackConfig.ApiKey);

            var recipientResponse = await _httpClient.GetAsync(apiUrl);
            return recipientResponse;
        }



        public async Task<HttpResponseMessage> PostRequest<T>(string url, T? request) where T : class
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _paystackConfig.ApiKey);

            var jsonContent = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var recipientResponse = await _httpClient.PostAsync(url, httpContent);
            return recipientResponse;
        }
    }
}
