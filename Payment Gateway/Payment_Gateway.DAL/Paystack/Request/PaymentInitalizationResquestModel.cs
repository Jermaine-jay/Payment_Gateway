using Payment_Gateway.DAL.Paystack.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment_Gateway.DAL.Paystack.Request
{
  
    public class TransactionInitializationRequestModel
    {
        public string email { get; set; }
        public int amount { get; set; }
        public string subaccount { get; set; }
        public Int32 transaction_charge { get; set; } = 0;
        public string bearer { get; set; } = "account";
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string callbackUrl { get; set; }
        public string reference { get; set; }
        public string plan { get; set; }
        public Int32 invoice_limit { get; set; } = 0;
        public bool makeReferenceUnique { get; set; } = false;
    }
    public class PaymentInitalizationResquestModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public SubData data { get; set; }
    }

    public class SubData
    {
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
    }
}
