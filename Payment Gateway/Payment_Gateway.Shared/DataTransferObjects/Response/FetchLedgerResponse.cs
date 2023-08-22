namespace Payment_Gateway.Shared.DataTransferObjects.Response
{
    public class FetchLedgerResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data[] data { get; set; }

        public class Data
        {
            public string integration { get; set; }
            public string domain { get; set; }
            public string balance { get; set; }
            public string currency { get; set; }
            public string difference { get; set; }
            public string reason { get; set; }
            public string model_responsible { get; set; }
            public string model_row { get; set; }
            public string id { get; set; }
            public string createdAt { get; set; }
            public string updatedAt { get; set; }
        }
    }
}
