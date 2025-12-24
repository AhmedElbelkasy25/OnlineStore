using Azure.Core;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.User;
using System.Threading.Tasks;

namespace OnlineStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController (IUserService userService) : ControllerBase
    {
        private readonly IUserService userService = userService;

        [HttpGet("AllRoles")]
        public IActionResult GetAllRoles()
        {
            var roles = userService.GetAllRoles();
            return Ok(new {roles= roles});
        }

        [HttpGet("AllUsers")]
        public IActionResult GetAllUsers()
        {
            var (success, users) = userService.GetAllUsers();

            return Ok(new { users = users.Adapt<List<UserDto>>() });

        }

        [HttpGet("{userId}/GetUserRole")]
        public async Task<IActionResult> GetUserRole( [FromRoute] string userId)
        {
            var (success , roles , msg ) = await userService.GetUserRolesAsync(userId);
            if (success)
            {
                return Ok(new { roles = roles });
            }
            return NotFound(new { msg = msg});
        }

        [HttpGet("{userId}/blockUser")]
        public async Task<IActionResult> BlockUserAsync([FromRoute] string userId)
        {
            var (success, msg) = await userService.BlockUserAsync(userId);
            if (success)
            {
                return Ok(new { msg = msg });
            }
            return BadRequest(msg);
        }


        [HttpGet("{userId}/UnBlockUser")]
        public async Task<IActionResult> UnBlockUserAsync( [FromRoute]string userId)
        {
            var (success, msg) = await userService.UnblockUserAsync(userId);
            if (success)
            {
                return Ok(new { msg = msg });
            }
            return BadRequest(msg);
        }


        [HttpPost("ChangeUserRole")]
        public async Task<IActionResult> ChangeUserRolesAsync([FromBody] ChangeRoleRequestDto changeRole)
        {
            var (success, msg) = await userService.ChangeUserRolesAsync(changeRole);

            if (!success)
                return BadRequest(new { msg = msg });

            return Ok(new { msg =msg });
        }

        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateUserDetails([FromBody] UserUpdateDto user)
        {
            var (success , msg) = await userService.UpdateUserDetailsAsync(user);
            if (!success)
            {
                return BadRequest(new { msg = msg });
            }
            return Ok(new { msg = msg });
        }

        [HttpPost("ChangeProfileImg")]
        public async Task<IActionResult> ChangeProfilePic([FromForm]ChangeProfilePicDto req)
        {
            var (success, msg) = await userService.ChangeProfilePicAsync(req);
            if(success) 
            { 
                return Ok(new {msg = msg});
            }
            return BadRequest(new { msg = msg });
        }


    }
}
