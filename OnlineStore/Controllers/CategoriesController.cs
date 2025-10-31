using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;
        [HttpGet("GetAll")]
        public async Task<IActionResult> Index()
        {
            var (success,Cats,msg) =await _categoryService.GetAllCategoriesAsync();
            if (success)
            {
                return Ok(new {categories = Cats});
            }else
            {
                return BadRequest(new { msg = msg });
            }
        }
    }
}
