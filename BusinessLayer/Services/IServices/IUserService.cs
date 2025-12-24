using Models.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.IServices
{
    public interface IUserService
    {
        List<string> GetAllRoles();
        Task<(bool, List<string>?, string)> GetUserRolesAsync(string userId);
        (bool, List<ApplicationUser>) GetAllUsers();
        Task<(bool, string)> BlockUserAsync(string userId);
        Task<(bool, string)> UnblockUserAsync(string userId);
        Task<(bool, string)> ChangeUserRolesAsync(ChangeRoleRequestDto changeRole);
        Task<(bool, string)> UpdateUserDetailsAsync(UserUpdateDto user);
        Task<(bool, string)> ChangeProfilePicAsync(ChangeProfilePicDto req);

    }
}
