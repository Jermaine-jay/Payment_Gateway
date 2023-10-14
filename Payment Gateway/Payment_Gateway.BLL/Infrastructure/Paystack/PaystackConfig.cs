namespace Payment_Gateway.BLL.Infrastructure.Paystack
{
    public class PaystackConfig
    {
         public string ApiKey { get; set; }
         public string ChargeUrl { get; set; }
         public string CreateTransferUrl { get; set; }
         public string FinalizeTransferUrl { get; set; }
         public string InitiateTransferUrl { get; set; }
         public string ResolveAccountNumberUrl { get; set; }
         public string ListBankUrl { get; set; }
         public string CheckBalanceUrl { get; set; }
         public string FetchLedgerUrl { get; set; }
    }
}
