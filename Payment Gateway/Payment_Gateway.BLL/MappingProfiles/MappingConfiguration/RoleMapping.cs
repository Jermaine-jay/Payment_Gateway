using AutoMapper;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Requests;
using Payment_Gateway.Shared.DataTransferObjects.Response;

namespace Payment_Gateway.BLL.Configurations.MappingConfiguration
{
    public class RoleMapping : Profile
    {
        public RoleMapping()
        {
            CreateMap<RoleDto, ApplicationRole>();
            CreateMap<ApplicationRole, RoleDto>();
            CreateMap<ApplicationRole, RoleResponse>();
        }
    }
}
