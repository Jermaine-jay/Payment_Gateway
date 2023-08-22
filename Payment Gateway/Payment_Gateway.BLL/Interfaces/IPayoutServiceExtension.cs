using Payment_Gateway.Shared.DataTransferObjects.Response;

namespace Payment_Gateway.BLL.Interfaces
{
    public interface IPayoutServiceExtension
    {
        Task<object> CreatePayout(string userId, TransferResponse response);
        Task<bool> UpdatePayout(string userId, FinalizeTransferResponse Response);

        Task<long> TranscationFees(long Amount);
        Task DebitTransfee(string userId, TransferResponse response);
        Task<object> CompleteTransfer(string userId, string pin, FinalizeTransferResponse response);
    }
}
