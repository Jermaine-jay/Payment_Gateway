using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Payment_Gateway.BLL.Interfaces.IServices;
using Payment_Gateway.BLL.LoggerService.Implementation;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects;
using System.Security.Claims;

namespace Payment_Gateway.BLL.Implementation.Services
{
    public class AdminProfileServices : IAdminProfileServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<AdminProfile> _adminProfileRepo;
        private readonly IRepository<Admin> _adminRepo;
    
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerManager _logger;

        public AdminProfileServices(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
       
            _adminProfileRepo = _unitOfWork.GetRepository<AdminProfile>();
            _adminRepo = _unitOfWork.GetRepository<Admin>();
            _adminProfileRepo = _unitOfWork.GetRepository<AdminProfile>();
            _adminRepo = _unitOfWork.GetRepository<Admin>();
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }



        public async Task<AdminProfile> CreateProfile(AdminProfileDto adminProfile)
        {
            try
            {
                _logger.LogInfo("Creating Admin user profile");

                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    _logger.LogError("User is not authorised");
                    throw new Exception("Only admins are authorized to create admin profile.");
                }

                var profile = new AdminProfile
                {

                    FirstName = adminProfile.FirstName,
                    LastName = adminProfile.LastName,
                    PhoneNumber = adminProfile.PhoneNumber,
                    Email = adminProfile.Email,
                    Address = adminProfile.Address,
                    UserName = adminProfile.UserName

                };

                Admin admin = await _adminRepo.GetSingleByAsync(s => s.UserId == userId);

                if (admin == null)
                {
                    throw new Exception("Admin not found");
                }

                profile.AdminIdentity = admin.Id;


                AdminProfile AddProfile = await _adminProfileRepo.AddAsync(profile);

                return AddProfile;
            }

            catch (Exception ex)
            {

                _logger.LogError($"Something went wrong in the {nameof(CreateProfile)} service method {ex}");

                throw;
            }
        }

        public void DeleteProfile()
        {
            throw new NotImplementedException();
        }

        public void DisplayProfile()
        {
            throw new NotImplementedException();
        }

        public async Task<AdminProfile> UpdateProfile(AdminProfileDto adminProfile)
        {
            try
            {
                _logger.LogInfo("Updating Admin user profile");

                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    _logger.LogError("User is not authorised");
                    throw new Exception("Only admins are authorized to update admin profile.");
                }

                Admin admin = await _adminRepo.GetSingleByAsync(s => s.UserId == userId);

                if (admin == null)
                {
                    throw new Exception("Admin not found");
                }

                AdminProfile profile = await _adminProfileRepo.GetSingleByAsync(p => p.AdminIdentity == admin.Id);

                if (profile == null)
                {
                    throw new Exception("Admin profile not found");
                }

                // Update the AdminProfile object with the new values from the DTO
                profile.FirstName = adminProfile.FirstName;
                profile.LastName = adminProfile.LastName;
                profile.PhoneNumber = adminProfile.PhoneNumber;
                profile.Email = adminProfile.Email;
                profile.Address = adminProfile.Address;
                profile.UserName = adminProfile.UserName;

                await _adminProfileRepo.UpdateAsync(profile);
                await _unitOfWork.SaveChangesAsync();

                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong in the {nameof(UpdateProfile)} service method {ex}");
                throw;
            }
        }

    }
}
