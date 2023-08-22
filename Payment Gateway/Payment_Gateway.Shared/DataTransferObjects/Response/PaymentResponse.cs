namespace Payment_Gateway.Shared.DataTransferObjects.Response
{
    public class PaymentResponse
    {
        public string message { get; set; }
        public string status { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
        public long amount { get; set; }
        public string channel { get; set; }
        public string status { get; set; }
        public string currency { get; set; }
        public string message { get; set; }
        public string domain { get; set; }
        public string fees { get; set; }
        public string gateway_response { get; set; }
        public string transaction_date { get; set; }
        public string reference { get; set; }
        public string created_at { get; set; }
        public string paid_at { get; set; }
        public string ip_address { get; set; }
        public Authorization authorization { get; set; }

    }
    public class Authorization
    {
        public string authorization_code { get; set; }
        public string bank { get; set; }
        public string transaction_date { get; set; }
        public string country_code { get; set; }
        public string exp_month { get; set; }
        public string exp_year { get; set; }
        public string last4 { get; set; }
        public string account_name { get; set; }
        public string card_type { get; set; }

    }
}
