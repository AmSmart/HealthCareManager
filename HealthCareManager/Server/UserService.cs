using HealthCareManager.Shared.DTOs;
using HealthCareManager.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HealthCareManager.Server
{
    public class UserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> CreateUserAsync(CreateUserDTO dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new Exception("Password and Confirm Passwprd do not match");

            if (dto.UserType is UserType.Admin)
                throw new Exception("Cannot create admin user");

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                FullName = dto.FullName,
                UserType = dto.UserType
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(',', $"{result.Errors.Select(x => x.Description)}"));

            var claim = new Claim(nameof(UserType), user.UserType.ToString());
            await _userManager.AddClaimAsync(user, claim);

            return user;
        }

        public JwtToken GenerateTokensAsync(Claim[] claims, DateTime now)
        {
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var jwtToken = new JwtSecurityToken(
                "HealthCareManager",
                "HealthCareManager",
                claims,
                expires: now.AddMinutes(60),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisShouldBeARelativelyLongSecret")),
                    SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var expiry = now.AddMinutes(60);
            return new JwtToken(accessToken, expiry);
        }

    }
}
