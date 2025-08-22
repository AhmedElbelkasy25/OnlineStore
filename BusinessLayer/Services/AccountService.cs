using Azure.Core;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;



namespace BusinessLayer.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork, IEmailSender emailSender, IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _config = config;
        }


        public async Task<(bool, string, List<string>)> Register(RegisterReqDTO reg)
        {

            ApplicationUser applicationUser = new()
            {
                Email = reg.Email,
                UserName = reg.UserName,
                Address = reg.Address,
                Age = reg.Age
            };

            var result = await _userManager.CreateAsync(applicationUser, reg.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                var frontendUrl = _config["Frontend:Host"] + "/ConfirmEmail";
                var confirmationLink = $"{frontendUrl}?userId={applicationUser.Id}&token={Uri.EscapeDataString(token)}"; // علشان يظبط المسافات اللي جوا التوكين


                await _emailSender.SendEmailAsync(applicationUser.Email,
               "Confirm Your Email",
               $"<p>Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.</p>");



                await _userManager.AddToRoleAsync(applicationUser, SD.Customer);

                return (true, "Welcome to our site", new List<string>());
            }
            else
            {

                return (false, "Registration failed", result.Errors.Select(e => e.Description).ToList());
            }
        }

        public async Task<(bool, string, List<string>)> ConfirmEmail(string userId, string token)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser is not null)
            {
                var result = await _userManager.ConfirmEmailAsync(appUser, token);
                if (result.Succeeded)
                {
                    return (true, "your Email has been confirmed successfully", []);
                }
                else
                {
                    return (false, "sorry...your Email has not confirmed ",
                        result.Errors.Select(e => e.Description).ToList());
                }
            }
            return (false, "sorry...your Account dosn't Exist", []);
        }

        public async Task<(bool, string)> Login(LoginDTO login)
        {
            ApplicationUser? userByEmail = await _userManager.FindByEmailAsync(login.Account);
            ApplicationUser? userByName = await _userManager.FindByNameAsync(login.Account);

            ApplicationUser? appUser = userByEmail ?? userByName;
            if (appUser == null)
            {

                return (false, "Your Account doesn't exist");
            }

            var result = await _userManager.CheckPasswordAsync(appUser, login.Password);
            if (result)
            {
                var roles = await _userManager.GetRolesAsync(appUser);
                List<Claim> claims = new List<Claim>()
                {
                    new Claim( ClaimTypes.NameIdentifier, appUser.Id),
                    new Claim(ClaimTypes.Name , appUser.UserName??""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                foreach (var item in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, item));
                }

                SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecretKey"] ?? ""));
                SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                JwtSecurityToken token = new JwtSecurityToken(
                    issuer: _config["JWT:Issuer"],
                    audience: _config["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: creds

                );
                return (true ,new JwtSecurityTokenHandler().WriteToken(token));
            }
            return (false, "the password is incorrect");
        }
        public async Task<(bool, string)> ForgetPassword(ForgetPasswordDTO forgetPasswordDTO)
        {
            var applicationUser =  _userManager.Users.Where(e=> (e.UserName == forgetPasswordDTO.EmailOrUserName || e.Email == forgetPasswordDTO.EmailOrUserName)).FirstOrDefault();


            if (applicationUser is not null)
            {

                var token = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);
                var encodedToken = WebUtility.UrlEncode(token); // علشان يعمل encode  لل  token
                var resetPasswordLink = $"{_config["Frontend:Host"]}/ResetPassword?userId={applicationUser.Id}&token={encodedToken}";

                

                await _emailSender.SendEmailAsync(applicationUser!.Email ?? "", "Reset Password", $"Please Reset Your Account Password By Clicking <a href='{resetPasswordLink}'>Here</a>");

                return (true, "Send Message Successfully, Check Your Email");

            }

            return(false , "Invalid Email Or User Name");
        }
        public async Task<(bool, string)> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByIdAsync(resetPasswordDTO.UserId);
            if (user is not null)
            {
                var token = WebUtility.UrlDecode(resetPasswordDTO.Token);
                var result = await _userManager.ResetPasswordAsync(user, token,resetPasswordDTO.Password);
                if (result.Succeeded)
                    return (true, "Password has been reset successfully");
                string errors = string.Join(", ", result.Errors.Select(e=>e.Description));
                return (false, $"Failed to reset password: {errors}");
            }
            return (false, "your account dosn't exist");
        }

        public async Task<(bool, string)> ChangePassword( ClaimsPrincipal claimsPrincipal , ChangePAsswordDTO changePAsswordDTO)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier); // to get the user Id from JWT token
            if (userId == null)
            {
                return (false, "you must login First");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return (false, " your account dosn't exist");
            }
            var result = await _userManager.ChangePasswordAsync(user, changePAsswordDTO.OldPass, changePAsswordDTO.NewPassword);
            if (result.Succeeded)
            {
                return (true, "the password has been changed successfully");
            }
            return (false, string.Join("," , result.Errors.Select(e=>e.Description)));
        }
    }
}
