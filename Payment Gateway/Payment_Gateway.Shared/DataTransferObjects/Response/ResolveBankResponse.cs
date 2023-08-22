namespace Payment_Gateway.Shared.DataTransferObjects.Response
{
    public class ResolveBankResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }

        public class Data
        {
            public string account_number { get; set; }
            public string account_name { get; set; }
            public string bank_id { get; set; }
        }
    }
}
