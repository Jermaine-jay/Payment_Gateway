using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Payment_Gateway.API.Extensions;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects.Requests;
using Payment_Gateway.Shared.DataTransferObjects.Response;
using System.Linq.Dynamic.Core;
using System.Net;

namespace Payment_Gateway.BLL.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IServiceFactory _serviceFactory;
        private readonly IMapper _mapper;
        private readonly IRepository<ApplicationRole> _roleRepo;
        private readonly IRepository<ApplicationRoleClaim> _roleClaimRepo;
        private readonly IUnitOfWork _unitOfWork;


        public RoleService(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            _unitOfWork = _serviceFactory.GetService<IUnitOfWork>();
            _userManager = _serviceFactory.GetService<UserManager<ApplicationUser>>();
            _roleManager = _serviceFactory.GetService<RoleManager<ApplicationRole>>();
            _roleRepo = _unitOfWork.GetRepository<ApplicationRole>();
            _roleClaimRepo = _unitOfWork.GetRepository<ApplicationRoleClaim>();
            _mapper = _serviceFactory.GetService<IMapper>();
        }



        public async Task<ServiceResponse<AddUserToRoleResponse>> AddUserToRole(AddUserToRoleRequest request)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(request.UserName.Trim().ToLower());
            if (user == null)
            {
                return new ServiceResponse<AddUserToRoleResponse>
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false
                };
            }

            var role = await _roleManager.FindByNameAsync(request.Role.ToLower().Trim());
            if (role == null)
            {
                return new ServiceResponse<AddUserToRoleResponse>
                {
                    Message = "Role Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false
                };
            }

            await _userManager.AddToRoleAsync(user, role.Name);
            return new ServiceResponse<AddUserToRoleResponse>
            {
                Message = $"{user} added to {role.Name}",
                StatusCode = HttpStatusCode.OK,
                Success = true
            };
        }


        public async Task<ServiceResponse<RoleDto>> CreateRoleAync(RoleDto request)
        {
            ApplicationRole role = await _roleManager.FindByNameAsync(request.Name.Trim().ToLower());
            if (role != null)
            {
                return new ServiceResponse<RoleDto>
                {
                    Message = "Role with name {request.Name} already exist",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false
                };
            }

            var applicationRole = new ApplicationRole
            {
                Name = request.Name,
            };
            //ApplicationRole roleToCreate = _mapper.Map<ApplicationRole>(request);

            await _roleManager.CreateAsync(applicationRole);
            return new ServiceResponse<RoleDto>
            {
                Message = $"{applicationRole} Created",
                StatusCode = HttpStatusCode.OK,
                Success = true
            };
        }


        public async Task<ServiceResponse> DeleteRole(string name)
        {
            ApplicationRole role = await _roleManager.FindByNameAsync(name.Trim().ToLower());
            if (role == null)
            {
                return new ServiceResponse
                {
                    Message = $"Role {name} does not Exist",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false
                };
            }

            await _roleManager.DeleteAsync(role);
            return new ServiceResponse
            {
                StatusCode = HttpStatusCode.OK,
                Success = true
            };
        }


        public async Task<ServiceResponse> EditRole(string id, string Name)
        {
            ApplicationRole role = await _roleManager.FindByNameAsync(id.Trim().ToLower());
            if (role == null)
            {
                return new ServiceResponse
                {
                    Message = $"Role {Name} Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false
                };
            }

            role.Name = Name;
            await _roleManager.UpdateAsync(role);

            return new ServiceResponse
            {
                StatusCode = HttpStatusCode.NotFound,
                Success = true
            };
        }


        public async Task<ServiceResponse> RemoveUserFromRole(AddUserToRoleRequest request)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(request.UserName.Trim().ToLower());
            if (user == null)
            {
                return new ServiceResponse
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false
                };
            }


            var myRoles = _roleManager.Roles.Select(x => x.Name);
            if (!myRoles.Contains(request.Role))
            {
                return new ServiceResponse
                {
                    Message = "Role does not Exist",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false
                };
            }

            var userIsInRole = await _userManager.RemoveFromRoleAsync(user, request.Role);
            return new ServiceResponse
            {
                StatusCode = HttpStatusCode.OK,
                Success = true
            };
        }


        public async Task<ServiceResponse<IEnumerable<string>>> GetUserRoles(string userName)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new ServiceResponse<IEnumerable<string>>
                {
                    Message = "User Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false
                };
            }

            List<string> userRoles = (List<string>)await _userManager.GetRolesAsync(user);
            if (!userRoles.Any())
            {
                return new ServiceResponse<IEnumerable<string>>
                {
                    Message = "User Roles Not Found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false
                };
            }


            return new ServiceResponse<IEnumerable<string>>
            {
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Data = userRoles
            };

        }



        public async Task<IEnumerable<RoleResponse>> GetAllRoles()
        {
            var roleQueryable = await _roleRepo.GetAllAsync(include: u => u.Include(x => x.RoleClaims));
            roleQueryable = roleQueryable.Where(r => r.Active);

            var roleResponseQueryable = roleQueryable.Select(s => new RoleResponse
            {
                Name = s.Name,
                Claims = s.RoleClaims.Where(r => r.ClaimValue.ToLower() is not null && r.Active),
                Active = s.Active
            }) ;

            return roleResponseQueryable;       
        }



       /* public async Task<IEnumerable<MenuClaimsResponse>> GetRoleClaims(string roleName)
        {
            IEnumerable<string> claimsInRole =
                 (await _roleRepo.GetQueryable().Include(x => x.UserRoles)
                     .Include(x => x.RoleClaims).Where(r => r.Name.ToLower() == roleName.ToLower() && r.Active)
                     .FirstOrDefaultAsync())?.RoleClaims.Select(s => s.ClaimValue) ?? new List<string>();


            IEnumerable<MenuClaimsResponse> menuClaims =
                (await _menuRepo.GetQueryable(m => m.Claims != null && m.Active).ToListAsync())
                .Where(m => claimsInRole != null && m.Claims.Intersect(claimsInRole).Any()).GroupBy(x => x.Name).Select(s =>
                    new MenuClaimsResponse
                    {
                        Menu = s.Key,
                        Claims = s.SelectMany(_ => _.Claims).ToList()
                    });

            return menuClaims;
        }*/



       /* public async Task UpdateRoleClaims(UpdateRoleClaimsDto request)
        {
            ApplicationRole? role = await _roleRepo.GetQueryable().Include(x => x.RoleClaims).FirstOrDefaultAsync(r => r.Name.ToLower() == request.Role.ToLower());

            if (role == null)
                throw new InvalidOperationException("Role does not exist");

            role.RoleClaims.Clear();

            IList<ApplicationRoleClaim> roleClaims = new List<ApplicationRoleClaim>(request.Claims.Count);

            foreach (string requestClaim in request.Claims)
            {
                roleClaims.Add(new ApplicationRoleClaim { ClaimType = ClaimTypes.Name, ClaimValue = requestClaim, RoleId = role.Id });

            }

            await _roleClaimRepo.AddRangeAsync(roleClaims);

            await _unitOfWork.SaveChangesAsync();
        }*/
    }
}
