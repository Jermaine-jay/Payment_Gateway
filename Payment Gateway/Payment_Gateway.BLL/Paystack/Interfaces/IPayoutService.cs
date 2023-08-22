using Payment_Gateway.Shared.DataTransferObjects.Request;
using Payment_Gateway.Shared.DataTransferObjects.Response;

namespace Payment_Gateway.BLL.Paystack.Interfaces
{
    public interface IPayoutService
    {
        Task<ResolveBankResponse> ResolveAccountNumber(ResolveAccountNumberRequest res);
        Task<ListBankResponse> ListBanks(string Currency);
        Task<CreateRecipientResponse> CreateTransferRecipient(CreateRecipientRequest createRecipientRequest);
        Task<TransferResponse> InitiateTransfer(InitiateTransferRequest initiateTransferRequest);
        Task<FinalizeTransferResponse> FinilizeTransfer(string transferIdOrCode);
    }
}
