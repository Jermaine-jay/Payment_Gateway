using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Payment_Gateway.BLL.Infrastructure.Paystack;
using Payment_Gateway.BLL.Paystack.Interfaces;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Models.Enums;
using Payment_Gateway.Shared.DataTransferObjects.Request;
using Payment_Gateway.Shared.DataTransferObjects.Response;
using PayStack.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Payment_Gateway.BLL.Paystack.Implementation
{
    public class MakePaymentService : IMakePaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Transaction> _transRepo;
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
            _transRepo = _unitOfWork.GetRepository<Transaction>();
        }

        public async Task<PaymentResponse> MakePayment(PaymentRequest paymentRequest)
        {
            var recipientResponse = await _PaystackPostRequest.PostRequest(_paystackConfig.ChargeUrl, paymentRequest);
            if (recipientResponse.IsSuccessStatusCode)
            { 
                string responseContent = await recipientResponse.Content.ReadAsStringAsync();
                PaymentResponse response = JsonConvert.DeserializeObject<PaymentResponse>(responseContent);
                return response;
            }

             throw new NotImplementedException();
        }


        public Task<bool> VerifyPayment()
        {
            throw new NotImplementedException();
        }

    }
}
