using AutoMapper;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment_Gateway.BLL.MappingProfiles.MappingConfiguration
{
    public class TransactionMappingProfile : Profile
    {
        public TransactionMappingProfile()
        {
            CreateMap<Transaction, TransactionDto>()
             .ForMember(dest => dest.Reference, opt => opt.MapFrom(src => src.Reference))
             .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
             .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.CreatedAt))
             .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Email));
        }
    }
}
