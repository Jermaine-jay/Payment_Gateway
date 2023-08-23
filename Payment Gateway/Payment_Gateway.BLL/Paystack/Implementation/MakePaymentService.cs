using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Payment_Gateway.BLL.Infrastructure.Paystack;
using Payment_Gateway.BLL.Paystack.Interfaces;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Request;
using Payment_Gateway.Shared.DataTransferObjects.Response;

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
