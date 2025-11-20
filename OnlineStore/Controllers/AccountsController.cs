using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterReqDTO reg)
        {
            var (succes, msg, Err) = await _accountService.Register(reg);
            if (succes)
            {
                return Ok(new { message = msg });
            }
            return BadRequest(new { message = msg, errors = Err });
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var (succes, msg, err) = await _accountService.ConfirmEmail(userId, token);
            if (succes)
            {
                return Ok(new { message = msg });
            }
            return BadRequest(new { message = msg, errors = err });
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO log)
        {
            var (success, msg ) = await _accountService.Login(log);
            if (success)
            {
                return Ok(new { token = msg });
            }
            return BadRequest(new { message = msg });
        }

        [HttpPost("Forget Password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDTO forgetpassword)
        {
            var (success, msg) = await _accountService.ForgetPassword(forgetpassword);
            if (success)
            {
                return Ok(new { message = msg });
            }
            return BadRequest(new { message = msg });
        }
        [HttpPost("Reset Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var (success, msg) = await _accountService.ResetPassword(resetPasswordDTO);
            if (success)
            {
                return Ok(new { message = msg });
            }
            return BadRequest(new { message = msg });
        }
        [Authorize]
        [HttpPost("Change Password")]
        public async Task<IActionResult> ChangePassword(ChangePAsswordDTO changePAsswordDTO)
        {
            var (success, msg) = await _accountService.ChangePassword(User, changePAsswordDTO);
            if (success)
                return Ok(new { message = msg });
            return BadRequest(new { message = msg });
        }
    }
}
