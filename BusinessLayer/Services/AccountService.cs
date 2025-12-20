using BusinessLayer.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Web;




namespace BusinessLayer.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;
        private readonly ITokenService tokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork, IEmailSender emailSender, IConfiguration config
            , ITokenService tokenService , IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _config = config;
            this.tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
        }


        public async Task<(bool, string, List<string>)> Register(RegisterReqDTO reg)
        {

            ApplicationUser applicationUser = new()
            {
                Email = reg.Email,
                UserName = reg.UserName,
                Name = reg.Name,
                Address = reg.Address,
                Age = reg.Age
            };

            var result = await _userManager.CreateAsync(applicationUser, reg.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                //var frontendUrl = _config["Frontend:Host"] + "auth/ConfirmEmail";
                var backendUrl = _config["JWT:Issuer"] + "/api/Accounts/ConfirmEmail";
                var confirmationLink = $"{backendUrl}?userId={applicationUser.Id}&token={Uri.EscapeDataString(token)}"; // علشان يظبط المسافات اللي جوا التوكين


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

        public async Task<(bool, string, string?)> Login(LoginDTO login)
        {
            ApplicationUser? userByEmail = await _userManager.FindByEmailAsync(login.Account);
            ApplicationUser? userByName = await _userManager.FindByNameAsync(login.Account);

            ApplicationUser? appUser = userByEmail ?? userByName;
            if (appUser == null)
            {

                return (false, "Your Account doesn't exist", null);
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

                var token = tokenService.GenerateAccessToken(claims);
                var refreshToken = tokenService.GenerateRefreshToken();
                appUser.RefreshToken = refreshToken;
                appUser.RefreshTokenExpiryTime = DateTime.Now.AddDays(10);
                await _unitOfWork.CommitAsync();
                return (true, token, refreshToken);
            }
            return (false, "the password is incorrect", null);
        }
        public async Task<(bool, string)> ForgetPassword(ForgetPasswordDTO forgetPasswordDTO)
        {
            var applicationUser = _userManager.Users.Where(e => (e.UserName == forgetPasswordDTO.EmailOrUserName || e.Email == forgetPasswordDTO.EmailOrUserName)).FirstOrDefault();


            if (applicationUser is not null)
            {

                var token = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));  // علشان يعمل encode  لل  token
                var resetPasswordLink = $"{_config["Frontend:Host"]}/auth/resetPassword?userId={applicationUser.Id}&token={encodedToken}";



                await _emailSender.SendEmailAsync(applicationUser!.Email ?? "", "Reset Password", $"Please Reset Your Account Password By Clicking <a href='{resetPasswordLink}'>Here</a>");

                return (true, "Send Message Successfully, Check Your Email");

            }

            return (false, "Invalid Email Or User Name");
        }
        public async Task<(bool, string)> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByIdAsync(resetPasswordDTO.UserId);
            if (user is not null)
            {
                var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordDTO.Token));
                var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDTO.Password);
                if (result.Succeeded)
                    return (true, "Password has been reset successfully");
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Failed to reset password: {errors}");
            }
            return (false, "your account dosn't exist");
        }

        public async Task<(bool, string)> ChangePassword(ClaimsPrincipal claimsPrincipal, ChangePAsswordDTO changePAsswordDTO)
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
            return (false, string.Join(",", result.Errors.Select(e => e.Description)));
        }


        public async Task<(bool, string, string?)> RefreshToken(RefreshTokenDTO refreshTokenDTO)
        {
            if (refreshTokenDTO is null)
                return (false, "Invalid client request", null);

            string accessToken = refreshTokenDTO.AccessToken ?? "";
            string refreshToken = refreshTokenDTO.RefreshToken ?? "";
            var principal = tokenService.GetPrincipalFromExpiredToken(accessToken);
            if (principal is null)
            {
                return (false, "Invalid client request", null);
            }
            var username = principal.Identity?.Name;
            if (username == null)
            {
                return (false, "Invalid client Details", null);
            }
            var user = await _userManager.FindByNameAsync(username);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return (false, "Invalid client request", null);
            var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _unitOfWork.CommitAsync();
            return (true, newAccessToken, newRefreshToken);

        }


        public async Task<(bool, string, ExternalLoginResultDTO?)> ExternalLogin(ExternalLoginDTO loginDto)
        {
            try
            {
                // 1. Validate the token with Google/Facebook
                var userInfo = await ValidateExternalToken(loginDto.Provider, loginDto.IdToken);

                if (userInfo is null)
                {
                    return (false, "Invalid token from external provider", null);
                }

                // 2. Find or create user
                var user = await _userManager.FindByEmailAsync(userInfo.Email);

                if (user == null)
                {
                    // Create new user
                    user = new ApplicationUser
                    {
                        UserName = GenerateUniqueUsername(userInfo.Name),
                        Email = userInfo.Email,
                        EmailConfirmed = true
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        return (false, "Error creating user account", null);
                    }
                }

                // 3. Link external login if not already linked
                var existingLogins = await _userManager.GetLoginsAsync(user);
                var hasExternalLogin = existingLogins.Any(l =>
                    l.LoginProvider == loginDto.Provider &&
                    l.ProviderKey == userInfo.ProviderId);

                if (!hasExternalLogin)
                {
                    var loginInfo = new UserLoginInfo(
                        loginDto.Provider,
                        userInfo.ProviderId,
                        loginDto.Provider);

                    var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
                    if (!addLoginResult.Succeeded)
                    {
                        return (false, "Error linking external login", null);
                    }
                }

                // 4. Generate JWT tokens
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var token = tokenService.GenerateAccessToken(claims);
                var refreshToken = tokenService.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(15);
                await _unitOfWork.CommitAsync();

                var result = new ExternalLoginResultDTO
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Email = user.Email??"",
                    UserName = user.UserName ?? ""
                };

                return (true, "Login successful", result);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        private async Task<ExternalUserInfo?> ValidateExternalToken(string provider, string idToken)
        {
            if (provider == "Google" || provider == "GOOGLE")
            {
                return await ValidateGoogleToken(idToken);
            }
            else if (provider == "Facebook" || provider == "FACEBOOK")
            {
                return await ValidateFacebookToken(idToken);
            }

            return null;
        }

        private async Task<ExternalUserInfo?> ValidateGoogleToken(string idToken)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(
                    $"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();


                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var payload = JsonSerializer.Deserialize<GoogleTokenInfo>(content, options);

                return new ExternalUserInfo
                {
                    Email = payload!.Email,
                    Name = payload.Name,
                    ProviderId = payload.Sub
                };
            }
            catch
            {
                return null;
            }
        }

        private async Task<ExternalUserInfo?> ValidateFacebookToken(string accessToken)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(
                    $"https://graph.facebook.com/me?fields=id,name,email&access_token={accessToken}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                var payload = JsonSerializer.Deserialize<FacebookUserInfo>(content);

                return new ExternalUserInfo
                {
                    Email = payload!.Email,
                    Name = payload.Name,
                    ProviderId = payload.Id
                };
            }
            catch
            {
                return null;
            }
        }

        private string GenerateUniqueUsername(string name)
        {
            var username = name.Replace(" ", "");
            var random = new Random();
            return $"{username}{random.Next(1000, 9999)}";
        }


    }
}
