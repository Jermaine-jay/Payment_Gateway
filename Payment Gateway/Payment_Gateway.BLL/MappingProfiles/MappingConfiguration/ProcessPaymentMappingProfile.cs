using AutoMapper;
using Payment_Gateway.Shared.DataTransferObjects.Request;
using PayStack.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment_Gateway.BLL.Paystack
{
    public class ProcessPaymentMappingProfile : Profile
    {
        public ProcessPaymentMappingProfile()
        {
            CreateMap<ProcessPaymentRequest, TransactionInitializeRequest>();
        }
    }
}
