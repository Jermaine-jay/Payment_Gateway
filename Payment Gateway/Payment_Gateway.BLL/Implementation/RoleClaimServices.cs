using Payment_Gateway.API.Extensions;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Requests;
using System.Net;

namespace Payment_Gateway.BLL.Implementation
{

    public class RoleClaimService : IRoleClaimService
    {
        private readonly IRepository<ApplicationRoleClaim> _roleClaimRepo;
        private readonly IRepository<ApplicationRole> _roleRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceFactory _serviceFactory;


        public RoleClaimService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _serviceFactory = serviceFactory;
            _unitOfWork = _serviceFactory.GetService<IUnitOfWork>();
            _roleClaimRepo = _unitOfWork.GetRepository<ApplicationRoleClaim>();
            _roleRepo = _unitOfWork.GetRepository<ApplicationRole>();
        }


        public async Task<ServiceResponse<RoleClaimResponse>> AddClaim(RoleClaimRequest request)
        {
            var getRole = await _roleRepo.GetSingleByAsync(r => r.Name.ToLower() == request.Role.ToLower());
            if (getRole == null)
            {
                return new ServiceResponse<RoleClaimResponse>()
                {
                    Message = "Role does not Exist, Ensure there are no spaces in the text entered",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            var checkExisting = await _roleClaimRepo.GetSingleByAsync(x => x.ClaimType == request.ClaimType && x.RoleId == getRole.Id);
            if (checkExisting != null)
            {
                return new ServiceResponse<RoleClaimResponse>()
                {
                    Message = "Identical claim value already exist for this role",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }



            var newClaim = new ApplicationRoleClaim()
            {
                RoleId = getRole.Id,
                ClaimType = request.ClaimType,
            };

            await _roleClaimRepo.AddAsync(newClaim);

            return new ServiceResponse<RoleClaimResponse>()
            {
                Message = $"{request.ClaimType} Claim Added to {getRole} Role",
                StatusCode = HttpStatusCode.OK,
                Data = new RoleClaimResponse
                {
                    Role = getRole.Name,
                    ClaimType = newClaim.ClaimType
                }
            };
        }


        public async Task<ServiceResponse<IEnumerable<RoleClaimResponse>>> GetUserClaims(string? role)
        {

            ApplicationRole getRole = await _roleRepo.GetSingleByAsync(x => x.Name.ToLower() == role);
            if (getRole == null)
            {
                return new ServiceResponse<IEnumerable<RoleClaimResponse>>()
                {
                    Message = "Role does not Exist, Ensure there are no spaces in the text entered",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            IEnumerable<ApplicationRoleClaim> claims = await _roleClaimRepo.GetAllAsync();
            var result = claims.Where(x => x.RoleId == getRole.Id).Select(u => new RoleClaimResponse
            {
                Role = getRole.Name,
                ClaimType = u.ClaimType
            });


            return new ServiceResponse<IEnumerable<RoleClaimResponse>>()
            {
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Data = result,
            };
        }


        public async Task<ServiceResponse> RemoveUserClaims(string claimType, string role)
        {
            var getRole = await _roleRepo.GetSingleByAsync(x => x.Name.ToLower() == role.ToLower());
            if (getRole == null)
            {
                return new ServiceResponse()
                {
                    Message = "Role does not Exist, Ensure there are no spaces in the text entered",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            var claim = await _roleClaimRepo.GetSingleByAsync(x => x.ClaimType == claimType && x.RoleId == getRole.Id);
            if (claim == null)
            {
                return new ServiceResponse()
                {
                    Message = "Claim value does not exist for this role",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            await _roleClaimRepo.DeleteAsync(claim);
            return new ServiceResponse()
            {
                Message = $"{claim} claim Removed From {getRole} Role",
                StatusCode = HttpStatusCode.OK,
                Success = true
            };

        }


        public async Task<ServiceResponse<RoleClaimResponse>> UpdateRoleClaims(UpdateRoleClaimsDto request)
        {
            ApplicationRole getRole = await _roleRepo.GetSingleByAsync(x => x.Name.ToLower() == request.Role);
            if (getRole == null)
            {
                return new ServiceResponse<RoleClaimResponse>()
                {
                    Message = "Role does not Exist, Ensure there are no spaces in the text entered",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            IEnumerable<ApplicationRoleClaim> claims = await _roleClaimRepo.GetAllAsync();
            var result = claims.Where(x => x.ClaimType == request.ClaimType && x.RoleId == getRole.Id).FirstOrDefault();

            result.ClaimType = request.NewClaim;
            await _roleClaimRepo.UpdateAsync(result);

            return new ServiceResponse<RoleClaimResponse>()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Success = true,
            };
        }

    }

}
