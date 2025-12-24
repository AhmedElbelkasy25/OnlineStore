using BusinessLayer.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class UserService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager , IWebHostEnvironment env, IOptions<FileSettings> fileSettings) :IUserService
    {
        

        public List<string> GetAllRoles()
        {
            var roles = roleManager.Roles.Select(e => e.Name).ToList();
            return roles!;
        }



        public async Task<(bool, List<string>?, string)> GetUserRolesAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return (false, null, "we coudn't found this user !");

            var roles = await userManager.GetRolesAsync(user);
            return (true, roles.ToList(), "");
        }


        public (bool, List<ApplicationUser>) GetAllUsers()
        {
            var users = userManager.Users.ToList();
            return (true, users);
        }



        public async Task<(bool, string)> BlockUserAsync(string userId)
        {

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "we coudn't found this user !");
            var enableResult = await userManager.SetLockoutEnabledAsync(user, true);
            if (!enableResult.Succeeded)
                return (false, "Failed to enable lockout.");
            var lockResult = await userManager.SetLockoutEndDateAsync(
                user,
                DateTimeOffset.MaxValue
            );
            if (!lockResult.Succeeded)
                return (false, "Failed to block user.");

            return(true, "User blocked successfully!");
        }


        public async Task<(bool, string)> UnblockUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return (false, "User not found!");

           
            var result = await userManager.SetLockoutEndDateAsync(user, null);

            if (!result.Succeeded)
                return (false, "Failed to unblock user.");

            return (true, "User unblocked successfully!");
        }


        public async Task<(bool, string)> ChangeUserRolesAsync(ChangeRoleRequestDto  changeRole)
        {
            var user = await userManager.FindByIdAsync(changeRole.UserId);
            if (user == null)
                return (false, "User not found!");

            
            var currentRoles = await userManager.GetRolesAsync(user);

            
            var rolesToRemove = currentRoles.Except(changeRole.Roles).ToList();

            
            var rolesToAdd = changeRole.Roles.Except(currentRoles).ToList();

            
            if (rolesToRemove.Any())
            {
                var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                    return (false, "Failed to remove old roles!");
            }

            
            if (rolesToAdd.Any())
            {
                
                foreach (var role in rolesToAdd)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        return (false, $"Role '{role}' does not exist!");
                }

                var addResult = await userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                    return (false, "Failed to add new roles!");
            }

            return (true, "User roles updated successfully!");
        }


        public async Task<(bool, string)> UpdateUserDetailsAsync(UserUpdateDto user)
        {
            var userDB = await userManager.FindByIdAsync(user.Id);
            if (userDB == null)
                return (false, "User not found");

            userDB.Name = user.Name;
            userDB.Address = user.Address;
            userDB.PhoneNumber = user.PhoneNumber;
            userDB.Age = user.Age;

            if (userDB.Email != user.Email)
            {
                var emailResult = await userManager.SetEmailAsync(userDB, user.Email);
                if (!emailResult.Succeeded)
                    return (false, emailResult.Errors.FirstOrDefault()!.Description);
            }
            var result = await userManager.UpdateAsync(userDB);
            if (!result.Succeeded)
            {
                return ( false , result.Errors.FirstOrDefault()!.Description);
            }
            return (true, " successfully updated");

        }

        public async Task<(bool, string)> ChangeProfilePicAsync(ChangeProfilePicDto req)
        {
            var user = await userManager.FindByIdAsync(req.UserId);
            if (user == null)
                return (false, " this user isn't exist");

            var (imgSuccess, newImg, msg) =  await SaveImageAsync(req.Pic);

            if (imgSuccess)
            {
                var oldImg = user.ImgUrl; 
                
                user.ImgUrl = newImg;
                var UpdateResult = await userManager.UpdateAsync(user);
                if (!UpdateResult.Succeeded)
                {
                    DeleteImage(newImg!);
                    return (false, UpdateResult.Errors.First().Description);
                }
                if (oldImg != null)
                {
                    DeleteImage(oldImg);
                }
                return (true, "the profile picture has been changed successfully ");
            }
            return (false, msg!);



        }



        private async Task<(bool success, string? fileName, string? message)> SaveImageAsync(IFormFile image)
        {
            try
            {

                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

                //if (image.Length > 5 * 1024 * 1024)
                //{
                //    return (false, null, "File size must be less than 5MB");
                //}

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var imagesFolder = Path.Combine(env.ContentRootPath, "..", fileSettings.Value.ProfileImagesFolder);

                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                var filePath = Path.Combine(imagesFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                return (true, fileName, null);

            }
            catch (Exception ex)
            {
                return (false, null, $"Error saving image: {ex.Message}");
            }
        }


        private bool DeleteImage(string fileName)
        {
            try
            {
                var filePath = Path.Combine(env.ContentRootPath, "..", fileSettings.Value.ProfileImagesFolder, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

    }
}
