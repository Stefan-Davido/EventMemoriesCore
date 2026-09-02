using DalEntities;
using EventMemoriesServices.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedItems;
using SharedItems.Exceptions;
using SharedItems.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventMemoriesServices.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationUserManager _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthService(
            ApplicationUserManager userManager, 
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration, 
            IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _userService = userService;
        }

        public async Task<ApplicationUser> GetUserAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<LoginResult> Login(LoginRequest model)
        {
            // Find user by email
            var user = await GetUserAsync(model.Email);

            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizeException("Invalid email or password");

            // Generate JWT token
            var token = await GenerateJwtToken(user);
            var userDto = await _userService.GetUserByIdAsync(user.Id);

            return new LoginResult
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = userDto,
                ExpiresIn = 3600 // 1 hour in seconds
            };
            
        }

        public async Task<LoginResult> Register(RegisterRequest model)
        {
            try
            {
                // Check if user already exists
                var existingUser = await GetUserAsync(model.Email);
                if(existingUser != null)
                    throw new InvalidOperationException("User already exists");

                // Create new user
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true,
                    CreatedTime = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException("Registration failed");
                }

                // Generate JWT token
                var token = await GenerateJwtToken(user);
                var userDto = await _userService.GetUserByIdAsync(user.Id);

                return new LoginResult
                {
                    Success = true,
                    Message = "Registration successful",
                    Token = token,
                    User = userDto,
                    ExpiresIn = 3600 // 1 hour in seconds
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during registration", ex);
            }
        }

        public async Task<LoginResult> RefreshToken(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new InvalidOperationException("User not found");

            var token = await GenerateJwtToken(user);

            return new LoginResult
            {
                Success = true,
                Message = "Token refreshed",
                Token = token,
                ExpiresIn = 3600
            };
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ApplicationUser> GetCurrentUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new InvalidOperationException("User not found");

            return user;
        }

        public async Task<bool> ChangePassword(Guid userId, ChangePasswordRequest model)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new InvalidOperationException("User not found");
            
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Password change failed: {errors}");
            }

            return true;
        }

        /// <summary>
        /// Helper method to generate JWT token
        /// </summary>
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "your-super-secret-key-change-this-in-production"));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(InternalClaimTypes.UserId, user.Id.ToString() ?? "")
            };

            // TODO:
            // add tenants
            // add events

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
