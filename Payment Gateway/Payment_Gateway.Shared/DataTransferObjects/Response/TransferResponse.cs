using System.Text.Json.Serialization;

namespace Payment_Gateway.Shared.DataTransferObjects.Response
{
    public class TransferResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
        public class Data
        {
            public string reference { get; set; }
            public long amount { get; set; }
            public string currency { get; set; }
            public string source { get; set; }
            public string reason { get; set; }
            public string recipient { get; set; }
            public string createdAt { get; set; }
            public string status { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

        }
    }
}
