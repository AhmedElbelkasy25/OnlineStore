using Azure.Core;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public async Task<(bool,string,List<string>)> Register(RegisterReqDTO reg)
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
                var frontendUrl = _config["Frontend:Host"] + "/Account/ConfirmEmail";
                var confirmationLink = $"{frontendUrl}?userId={applicationUser.Id}&token={Uri.EscapeDataString(token)}"; // علشان يظبط المسافات اللي جوا التوكين


                await _emailSender.SendEmailAsync(applicationUser.Email,
               "Confirm Your Email",
               $"<p>Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.</p>");



                await _userManager.AddToRoleAsync(applicationUser, SD.Customer);

                return (true,token,new List<string>());
            }
            else
            {

                return (false, "Registration failed", result.Errors.Select(e=>e.Description).ToList());
            }
        }

        public async Task<(bool,string,List<string>)> ConfirmEmail(string userId, string token)
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
    }
}
