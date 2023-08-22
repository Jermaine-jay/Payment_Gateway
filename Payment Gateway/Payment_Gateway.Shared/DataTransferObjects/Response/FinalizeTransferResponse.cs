namespace Payment_Gateway.Shared.DataTransferObjects.Response
{
    public class FinalizeTransferResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }

        public class Data
        {
            public string status { get; set; }
            public string amount { get; set; }
            public string reference { get; set; }
            public string currency { get; set; }
            public string domain { get; set; }
            public string reason { get; set; }
            public string recipient { get; set; }
            public string integration { get; set; }
            public string Id { get; set; }
        }
    }
}
