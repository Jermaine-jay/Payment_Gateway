using Newtonsoft.Json;
using Payment_Gateway.BLL.Infrastructure.Paystack;
using Payment_Gateway.BLL.LoggerService.Implementation;
using Payment_Gateway.BLL.Paystack.Interfaces;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Request;
using Payment_Gateway.Shared.DataTransferObjects.Response;

namespace Payment_Gateway.BLL.Paystack.Implementation
{
    public class PayoutService : IPayoutService
    {
        public readonly IRepository<Transaction> _transactionRepo;
        public readonly IRepository<Payout> _payoutRepo;
        private readonly ILoggerManager _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaystackPostRequest _PaystackPostRequest;
        private readonly PaystackConfig _paystackConfig;


        public PayoutService(IUnitOfWork unitOfWork, ILoggerManager logger, PaystackConfig paystackConfig, IPaystackPostRequest paystackPostRequest)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _payoutRepo = _unitOfWork.GetRepository<Payout>();
            _transactionRepo = unitOfWork.GetRepository<Transaction>();
            _PaystackPostRequest = paystackPostRequest;
            _paystackConfig = paystackConfig;
        }


        public async Task<CreateRecipientResponse> CreateTransferRecipient(CreateRecipientRequest createRecipientRequest)
        {
            _logger.LogInfo("Create Transfer Recipient");

            var recipientResponse = await _PaystackPostRequest.PostRequest(_paystackConfig.CreateTransferUrl, createRecipientRequest);
            if (recipientResponse.IsSuccessStatusCode)
            {
                string recipientResponseContent = await recipientResponse.Content.ReadAsStringAsync();
                var getResponse = JsonConvert.DeserializeObject<CreateRecipientResponse>(recipientResponseContent);

                _logger.LogInfo("Transfer recipient created successfully!");
                return getResponse;
            }
            throw new InvalidOperationException("Could not create transfer recipient");
        }



        public async Task<FinalizeTransferResponse> FinilizeTransfer(string transferIdOrCode)
        {
            _logger.LogInfo("Finalize Transfer");

            //var transaction = await _transactionRepo.GetByAsync(x=>x)

            var recipientResponse = await _PaystackPostRequest.PostRequest(_paystackConfig.FinalizeTransferUrl, transferIdOrCode);
            if (recipientResponse.IsSuccessStatusCode)
            {
                string recipientResponseContent = await recipientResponse.Content.ReadAsStringAsync();
                var getResponse = JsonConvert.DeserializeObject<FinalizeTransferResponse>(recipientResponseContent);

                _logger.LogInfo($"Transfer Done!");
                return getResponse;
            }
            throw new InvalidOperationException("could not finalize transfer");
        }



        public async Task<TransferResponse> InitiateTransfer(InitiateTransferRequest initiateTransferRequest)
        {

            _logger.LogInfo("Initiate Transfer");

            var recipientResponse = await _PaystackPostRequest.PostRequest(_paystackConfig.InitiateTransferUrl, initiateTransferRequest);
            if (recipientResponse.IsSuccessStatusCode)
            {
                string recipientResponseContent = await recipientResponse.Content.ReadAsStringAsync();
                var getResponse = JsonConvert.DeserializeObject<TransferResponse>(recipientResponseContent);

                _logger.LogInfo($"Transfer Initiated!");
                return getResponse;
            }
            throw new InvalidOperationException("Could not initiate ransfer!");

        }


        public async Task<ListBankResponse> ListBanks(string Currency)
        {
            _logger.LogInfo("Finalize Transfer");
            var Url = $"{_paystackConfig.ListBankUrl}{Currency}";

            var recipientResponse = await _PaystackPostRequest.GetRequest(Url);
            if (recipientResponse != null)
            {
                var listResponse = await recipientResponse.Content.ReadAsStringAsync();
                var getResponse = JsonConvert.DeserializeObject<ListBankResponse>(listResponse);

                _logger.LogInfo($"Banks Available!");
                return getResponse;
            }
            throw new InvalidOperationException("Could not get list of banks");
        }


        public async Task<ResolveBankResponse> ResolveAccountNumber(ResolveAccountNumberRequest res)
        {
            var apiUrl = $"{_paystackConfig.ResolveAccountNumberUrl}{res.AccountNumber}&bank_code={res.BankCode}";
            _logger.LogInfo("Verify Account Number");

            var recipientResponse = await _PaystackPostRequest.GetRequest(apiUrl);
            if (recipientResponse != null)
            {
                var listResponse = await recipientResponse.Content.ReadAsStringAsync();
                var getResponse = JsonConvert.DeserializeObject<ResolveBankResponse>(listResponse);

                _logger.LogInfo($"account Available!");
                return getResponse;
            }

            throw new InvalidOperationException("Account does not exist!");
        }
    }
}
