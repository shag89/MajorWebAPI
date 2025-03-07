using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Core.DataAccess;
using WebAPI.Core.Entity;
using WebAPI.Core.Model;
using WebAPI.Core.Service;

namespace WebAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _Configuration;
        private readonly IDataAccessLayer _dataAccessLayer;
        public AuthService(IConfiguration cnfiguration,IDataAccessLayer dataAccessLayer)
        {
            _Configuration = cnfiguration;
            _dataAccessLayer = dataAccessLayer;
        }

        public async Task<string> GenerateRefreshToken(int UserId)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {

                rng.GetBytes(randomNumber);
                double dateadd = Convert.ToDouble(_Configuration["JWT:RefreshTokenExpiryDays"]);

                var userrefreshtoken = new UserRefreshToken
                {
                    RefreshToken = Convert.ToBase64String(randomNumber),
                    RefreshTokenExpiryTime = DateTime.Now.AddDays(dateadd),
                    UserId = UserId
                };

                var id = await _dataAccessLayer.Repository<UserRefreshToken>().InsertAsync(userrefreshtoken);

                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<string> GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = _Configuration.GetSection("JWT");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, user.Role.ToString()),
                            new Claim(ClaimTypes.Name,user.UserName),
                            new Claim("DateOfBirth","2000-01-21")
                            // Add more claims as needed
                        },
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return tokenHandler.WriteToken(token);

        }

        public async Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                /*
                    The function GetPrincipalFromExpiredToken is designed to retrieve the claims(principal)
                    from an expired JWT token.This is typically used in token refresh scenarios where you want to validate
                    the identity and claims of a user without requiring the token to be unexpired. Here’s a breakdown of 
                    what each part of the function does
                */

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false, // Ignore token expiry for the validation process
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["JWT:Key"])),
                    ValidIssuer = _Configuration["JWT:Issuer"],
                    ValidAudience = _Configuration["JWT:Audience"]
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                /*
                 ValidateToken method verifies the token against the TokenValidationParameters, and if valid, returns a ClaimsPrincipal representing the user
                 */
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);


                /*   After validation, the function checks if the token is a JwtSecurityToken and if it uses the HmacSha256 algorithm   */

                if (securityToken is JwtSecurityToken jwtToken && jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return principal;
                }
                throw new SecurityTokenException("Invalid token");
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }

        }

        public Task<IActionResult> Login(LoginModel loginModel)
        {
            throw new NotImplementedException();
        }
    }

}
