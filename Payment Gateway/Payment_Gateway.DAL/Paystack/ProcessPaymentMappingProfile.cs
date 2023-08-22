using AutoMapper;
using Payment_Gateway.Shared.DataTransferObjects.Request;
using PayStack.Net;

namespace Payment_Gateway.DAL.Paystack
{
    public class ProcessPaymentMappingProfile : Profile
    {
        public ProcessPaymentMappingProfile()
        {
            CreateMap<ProcessPaymentRequest, TransactionInitializeRequest>();
        }
    }
}
