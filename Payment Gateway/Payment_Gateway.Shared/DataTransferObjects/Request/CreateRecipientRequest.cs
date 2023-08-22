using Newtonsoft.Json;

namespace Payment_Gateway.Shared.DataTransferObjects.Request
{
    public class CreateRecipientRequest
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("account_number")]
        public string? AccountNumber { get; set; }
        [JsonProperty("bank_code")]
        public string? BankCode { get; set; }

        [JsonProperty("currency")]
        public string? Currency { get; set; } = "NGN";
    }

}
