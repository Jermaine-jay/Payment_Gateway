using Newtonsoft.Json;

namespace Payment_Gateway.Shared.DataTransferObjects.Request
{
    public class InitiateTransferRequest
    {
        [JsonProperty("amount")]
        public int AmountInKobo { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; } = "balance";

        [JsonProperty("recipient_code")]
        public string RecipientCode { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = "NGN";

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; } = Guid.NewGuid().ToString();
    }

}
