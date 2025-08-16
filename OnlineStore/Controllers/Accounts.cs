
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Threading.Tasks;


namespace OnlineStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        private readonly IAccountService _accountService;

        public Accounts(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterReqDTO reg )
        {
            var (succes,msg,Err) = await _accountService.Register(reg);
            if (succes)
            {
                return Ok(new {message = msg});
            }
            return BadRequest(new {message = msg , errors = Err});
        }
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var (succes,msg,err) = await _accountService.ConfirmEmail(userId, token);
            if (succes)
            {
                return Ok(new { message = msg });
            }
            return BadRequest(new { message = msg, errors = err });
        }
    }
}
