namespace Payment_Gateway.Shared.DataTransferObjects.Response
{
    public class CreateRecipientResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }

        public class Data
        {
            public string active { get; set; }
            public string currency { get; set; }
            public string description { get; set; }
            public string domain { get; set; }
            public string email { get; set; }
            public string id { get; set; }
            public string integration { get; set; }
            public string metadata { get; set; }
            public string recipient_code { get; set; }
            public string type { get; set; }
            public string createdAt { get; set; }
            public string updatedAt { get; set; }
            public string is_deleted { get; set; }
            public string isDeleted { get; set; }
            public Details details { get; set; }
            public class Details
            {
                public string account_name { get; set; }
                public string account_number { get; set; }
                public string bank_name { get; set; }
                public string bank_code { get; set; }
                public string authorization_code { get; set; }
            }
        }
    }
}
