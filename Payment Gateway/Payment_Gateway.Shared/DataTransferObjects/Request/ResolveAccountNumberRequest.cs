using Newtonsoft.Json;

namespace Payment_Gateway.Shared.DataTransferObjects.Request
{
    public class ResolveAccountNumberRequest
    {
        [JsonProperty("account_number")]
        public string AccountNumber { get; set; }

        [JsonProperty("bank_code")]
        public string BankCode { get; set; }
    }

}
