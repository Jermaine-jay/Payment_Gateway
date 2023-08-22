namespace Payment_Gateway.Shared.DataTransferObjects.Response
{
    public class CheckBalanceResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data[] data { get; set; }

        public class Data
        {
            public string currency { get; set; }
            public string balance { get; set; }
        }
    }
}
