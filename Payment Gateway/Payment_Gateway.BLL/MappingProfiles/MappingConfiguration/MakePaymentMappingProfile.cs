using AutoMapper;
using Payment_Gateway.Shared.DataTransferObjects.Request;
using Payment_Gateway.Shared.DataTransferObjects.Response;
using PayStack.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Payment_Gateway.BLL.Paystack.Implementation.MakePaymentService;

namespace Payment_Gateway.BLL.MappingProfiles.MappingConfiguration
{
    public class MakePaymentMappingProfile : Profile
    {
        public MakePaymentMappingProfile()
        {
            CreateMap<PaymentRequest, PaymentResponse>();
        }


    }
}
