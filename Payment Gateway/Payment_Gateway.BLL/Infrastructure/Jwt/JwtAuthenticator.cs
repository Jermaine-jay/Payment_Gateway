using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Models.Enums;
using Payment_Gateway.Shared.DataTransferObjects.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Payment_Gateway.BLL.Infrastructure.jwt
{
    public class JwtAuthenticator : IJWTAuthenticator
    {
        private readonly JwtConfig _jwtConfig;
        private readonly IUnitOfWork _unitOfWork;

        public JwtAuthenticator(JwtConfig jwtConfig, IUnitOfWork unitOfWork)
        {
            _jwtConfig = jwtConfig;
            _unitOfWork = unitOfWork;
        }


        public async Task<JwtToken> GenerateJwtToken(ApplicationUser user)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            string userRole = user.UserType.GetStringValue();
            IdentityOptions _options = new();

            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
             /*   new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),*/
                new Claim(ClaimTypes.Role, userRole),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(_options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
                new Claim(_options.ClaimsIdentity.UserNameClaimType, user.UserName),
            };


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.Expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            await _unitOfWork.SaveChangesAsync();

            return new JwtToken
            {
                Token = jwtToken,
                Issued = DateTime.Now,
                Expires = tokenDescriptor.Expires.Value
            };

        }
    }
}
