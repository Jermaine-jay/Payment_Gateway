using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects;

namespace Payment_Gateway.BLL.Interfaces.IServices
{
    public interface IAdminProfileServices
    {
        Task<AdminProfile> CreateProfile(AdminProfileDto adminProfile);
        void DisplayProfile();
        Task<AdminProfile> UpdateProfile(AdminProfileDto adminProfile);
        void DeleteProfile();
    }
}
