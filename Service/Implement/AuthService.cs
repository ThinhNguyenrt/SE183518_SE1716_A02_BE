using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Models;
using Repository.Repository.Interface;
using Repository.Requests;
using Repository.Responses;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<SystemAccount> _passwordHasher = new();

        public AuthService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            var repo = _unitOfWork.Repository<SystemAccount>();

            var existing = (await repo.FindAsync(x => x.AccountEmail == request.AccountEmail)).FirstOrDefault();
            if (existing != null)
                return null;

            var user = new SystemAccount
            {
                AccountId = Guid.NewGuid(),
                AccountName = request.AccountName,
                AccountEmail = request.AccountEmail,
                AccountRole = request.AccountRole,
            };

            user.AccountPassword = _passwordHasher.HashPassword(user, request.AccountPassword);
            await repo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponse
            {
                AccountName = user.AccountName,
                AccountEmail = user.AccountEmail,
                AccountRole = user.AccountRole,
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var repo = _unitOfWork.Repository<SystemAccount>();
            var user = (await repo.FindAsync(a => a.AccountEmail == request.Email)).FirstOrDefault();

            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.AccountPassword, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return null;

            return new AuthResponse
            {
                AccountName = user.AccountName,
                AccountEmail = user.AccountEmail,
                AccountRole = user.AccountRole,
                Token = GenerateJwtToken(user)
            };
        }

        private string GenerateJwtToken(SystemAccount user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString()),
                new Claim(ClaimTypes.Email, user.AccountEmail),
                new Claim(ClaimTypes.Role, user.AccountRole.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
