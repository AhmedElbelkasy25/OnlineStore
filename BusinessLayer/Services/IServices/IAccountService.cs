
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.IServices
{
    public interface IAccountService
    {
        Task<(bool, string, List<string>)> Register(RegisterReqDTO reg);
        Task<(bool, string, List<string>)> ConfirmEmail(string userId, string token);
        Task<(bool, string)> Login(LoginDTO login);
        Task<(bool, string)> ForgetPassword(ForgetPasswordDTO forgetPasswordDTO);
        Task<(bool, string)> ResetPassword(ResetPasswordDTO resetPasswordDTO);
        Task<(bool, string)> ChangePassword(ClaimsPrincipal claimsPrincipal, ChangePAsswordDTO changePAsswordDTO);
    }
}
