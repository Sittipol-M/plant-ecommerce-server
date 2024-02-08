using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using plant_ecommerce_server.Data;
using plant_ecommerce_server.Exceptions;
using plant_ecommerce_server.Models;
using plant_ecommerce_server.Requests;
using plant_ecommerce_server.Responses;

namespace plant_ecommerce_server.Services
{
    public interface IAuthenService
    {
        Task<LoginResponse> Login(LoginBodyRequest bodyRequest);
        Task Register(RegisterBodyRequest bodyRequest);
        Task<RefreshTokenResponse> RefreshToken(RefreshTokenBodyRequest bodyRequest, string? refreshToken);
        Task Logout(LogoutBodyRequest bodyRequest);
    }
    public class AuthenService : IAuthenService
    {
        private readonly IConfiguration _configuration;
        private readonly PlantEcommerceContext _plantEcommerceContext;
        public AuthenService(PlantEcommerceContext plantEcommerceContext, IConfiguration configuration)
        {
            _plantEcommerceContext = plantEcommerceContext;
            _configuration = configuration;
        }
        public async Task<LoginResponse> Login(LoginBodyRequest bodyRequest)
        {
            User? user = await _plantEcommerceContext.Users.FirstOrDefaultAsync(u => u.Username.Equals(bodyRequest.Username));
            if (user is null)
                throw new UnauthorizedException("Invalid username");
            bool isPasswordVerify = BCrypt.Net.BCrypt.Verify(bodyRequest.Password, user.Password);
            if (isPasswordVerify is false)
                throw new UnauthorizedException("Invalid password");
            string accessToken = GenerateAccessToken(user);
            string refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpireAt = DateTime.UtcNow.AddDays(1);

            _plantEcommerceContext.Users.Update(user);
            await _plantEcommerceContext.SaveChangesAsync();

            var response = new LoginResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new() { Username = user.Username, Role = user.Role }
            };
            return response;
        }
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            RandomNumberGenerator.Create().GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private string GenerateAccessToken(User user)
        {
            List<Claim> claims = new(){
                new(ClaimTypes.Name,user.Username),
            };
            if (user.Role is not null)
            {
                claims.Add(new(ClaimTypes.Role, user.Role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: credentials
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }

        public async Task<RefreshTokenResponse> RefreshToken(RefreshTokenBodyRequest bodyRequest, string? refreshToken)
        {
            if (bodyRequest.AccessToken is null)
            {
                throw new NotFoundException("Access token not found");
            }

            TokenValidationParameters validationParameters = new()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"])),
                ValidateLifetime = false
            };

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(bodyRequest.AccessToken, validationParameters, out SecurityToken validatedToken);

            string? username = principal?.Identity?.Name;

            User? user = await _plantEcommerceContext.Users.FirstOrDefaultAsync(u => u.Username.Equals(username));
            if (user is null)
            {
                throw new NotFoundException("Not found user");
            }
            if (!user.RefreshToken.Equals(refreshToken))
            {
                throw new UnauthorizedException("Invalid refresh token");
            }
            if (user.RefreshTokenExpireAt < DateTime.UtcNow)
            {
                throw new UnauthorizedException("Refresh token is expired");
            }

            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            _plantEcommerceContext.Users.Update(user);
            await _plantEcommerceContext.SaveChangesAsync();
            var response = new RefreshTokenResponse()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
            return response;
        }

        public async Task Register(RegisterBodyRequest bodyRequest)
        {
            if (!IsConfirmPasswordIsSame(bodyRequest))
                throw new UnauthorizedException("Password is not the same as confirm password");
            User? existUser = await _plantEcommerceContext.Users.FirstOrDefaultAsync(u => u.Username.Equals(bodyRequest.Username));
            if (existUser is not null)
                throw new ConflictException("Username is used");
            string hashPassword = BCrypt.Net.BCrypt.HashPassword(bodyRequest.Password);
            User newUser = new()
            {
                Username = bodyRequest.Username,
                Password = hashPassword,
            };
            await _plantEcommerceContext.Users.AddAsync(newUser);
            await _plantEcommerceContext.SaveChangesAsync();
        }
        private static bool IsConfirmPasswordIsSame(RegisterBodyRequest bodyRequest)
        {
            if (bodyRequest.Password.Equals(bodyRequest.ConfirmPassword)) return true;
            return false;
        }

        public async Task Logout(LogoutBodyRequest bodyRequest)
        {
            var user = await _plantEcommerceContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(bodyRequest.UserId));
            if (user is null)
            {
                throw new NotFoundException("Not found user");
            }
            user.RefreshToken = null;
            user.RefreshTokenExpireAt = null;
            _plantEcommerceContext.Users.Update(user);
            await _plantEcommerceContext.SaveChangesAsync();
        }
    }
}