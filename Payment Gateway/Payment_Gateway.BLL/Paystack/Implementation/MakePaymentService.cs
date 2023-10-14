using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Payment_Gateway.BLL.Infrastructure.Paystack;
using Payment_Gateway.BLL.Paystack.Interfaces;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Request;
using System.Net.Http.Headers;
using System.Net;
using Payment_Gateway.Shared.DataTransferObjects.Response;
using static System.Net.WebRequestMethods;

namespace Payment_Gateway.BLL.Paystack.Implementation
{
    public class MakePaymentService : IMakePaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Payin> _transRepo;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly PaystackConfig _paystackConfig;
        private readonly IPaystackPostRequest _PaystackPostRequest;
        private string? _ApiKey;

        IMapper _mapper;
        public MakePaymentService(IPaystackPostRequest paystackPostRequest, PaystackConfig paystackConfig, IUnitOfWork unitOfWork)
        {
            _PaystackPostRequest = paystackPostRequest;
            _paystackConfig = paystackConfig;
            _unitOfWork = unitOfWork;
            _httpClient = new HttpClient();
            _transRepo = _unitOfWork.GetRepository<Payin>();
        }


        public async Task<object> MakePayment(PaymentRequest paymentRequest)
        {
            var payload = new
            {
                reference = Guid.NewGuid(),
                email = paymentRequest.Email,
                amount = paymentRequest.amountInKobo,
                card = new
                {
                    number = paymentRequest.CardNumber,
                    cvv = paymentRequest.Cvv,
                    expiry_month = paymentRequest.expiryMonth,
                    expiry_year = paymentRequest.expiryYear,
                    pin = paymentRequest.Pin
                }
            };
            var recipientResponse = await _PaystackPostRequest.PostRequest(_paystackConfig.ChargeUrl, payload);
            if (!recipientResponse.IsSuccessStatusCode)
            {
                throw new NotImplementedException(" no payment made");
            }

            string responseContent = await recipientResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<PaymentResponse>(responseContent);

            return response;
        }


        public async Task<object> CheckChargeStatus(string reference)
        {

            // Create a new HttpClient.
            HttpClient client = new HttpClient();

            // Set the Authorization header.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _paystackConfig.ApiKey);

            // Make a GET request to the Paystack API.
            HttpResponseMessage response = await client.GetAsync($"https://api.paystack.co/transaction/verify/{reference}");

            // Check the response status code.
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Failed to get charge status: {response.StatusCode}");
            }

            // Read the response content as a string.
            string responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize the response content into a dynamic object.
            dynamic responseData = JsonConvert.DeserializeObject<dynamic>(responseContent);

            // Get the status of the charge from the response data.
            string status = responseData.data.status;

            // Return the status of the charge.
            return responseData;
        }



        /*public async Task<object> MakePayment(PaymentRequest paymentRequest)
        {
        *//*    string ApiKey = (string)_configuration.GetSection("Paystack").GetSection("ApiKey").Value;
            string Url = (string)_configuration.GetSection("Paystack").GetSection("Url").Value;*//*
            var payload = new
            {
                reference = paymentRequest.Reference,
                email = paymentRequest.email,
                amount = paymentRequest.amountInKobo,
                card = new
                {
                    number = paymentRequest.CardNumber,
                    cvv = paymentRequest.cvv,
                    expiry_month = paymentRequest.expiryMonth,
                    expiry_year = paymentRequest.expiryYear
                }
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _paystackConfig.ApiKey);
            var jasonContent = JsonConvert.SerializeObject(payload);
            var httpContent = new StringContent(jasonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_paystackConfig.ChargeUrl, httpContent);
            var responseContent = await response.Content.ReadAsStringAsync();


            var responseData = JsonConvert.DeserializeObject<PaymentResponse>(responseContent);


            if (responseData == null && responseData.data == null)
            {
                throw new NotImplementedException(" no payment");
            }
            string reference = responseData.data.reference;
            var amount = responseData.data.amount;
            var success = response.IsSuccessStatusCode;

            return responseData;
        }*/


        public Task<bool> VerifyPayment()
        {
            throw new NotImplementedException();
        }

    }
}
