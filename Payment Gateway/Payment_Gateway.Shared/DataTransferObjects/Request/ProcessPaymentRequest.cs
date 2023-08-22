using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Payment_Gateway.Shared.DataTransferObjects.Request
{
    public class PaymentRequest
    {

        public string Reference { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public int AmountInKobo { get; set; }

        [Required]
        public string Email { get; set; }

        //[JsonProperty("callback_url")]
        public string CallbackUrl { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = "NGN";

        public string Bearer { get; set; }

        public Card card { get; set; }

        public class Card
        {

            [JsonProperty("number")]
            public string cardNumber { get; set; }
            public string cvv { get; set; }

            [JsonProperty("expiry_month")]
            public int expiryMonth { get; set; }

            [JsonProperty("expiry_year")]
            public int expiryYear { get; set; }
        }
    }


    public class ProcessPaymentRequest
    {
        [Required]
        public string Reference { get; set; }

        [JsonProperty("amount")]
        [Required]
        public int AmountInKobo { get; set; }
        [Required]
        public string Email { get; set; }

        [JsonProperty("callback_url")]
        public string CallbackUrl { get; set; }

        [JsonProperty("subaccount")]
        public string SubAccount { get; set; }

        [JsonProperty("transaction_charge")]
        [Required]
        public int TransactionCharge { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = "NGN";

        public string Bearer { get; set; }


    }
}
