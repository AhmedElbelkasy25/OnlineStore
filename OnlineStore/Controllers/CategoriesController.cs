using Mapster;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs.Category.Response;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var (success, categories, msg) = await _categoryService.GetAllCategoriesAsync();

            if (!success)
                return BadRequest(new { msg });

            
            var categoryDtos = categories.Select(e => e.Adapt<CategoryDTOResponse>()).ToList();

            return Ok(new { categories = categoryDtos });
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryById([FromRoute] int id)
        {
            var (success, category, msg) = _categoryService.GetCategoryById(id);

            if (!success || category is null)
                return BadRequest(new { msg });

            var categoryDto = category.Adapt<CategoryDTOResponse>();
            return Ok(categoryDto);
        }

        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] CategoryDTORequest catDto)
        {

            var category = catDto.Adapt<Category>();

            var (success, createdCategory, msg) = await _categoryService.CreateCategoryAsync(category);

            if (!success || createdCategory is null)
                return BadRequest(new { msg });

            var createdDto = createdCategory.Adapt<CategoryDTOResponse>();
            return Created($"{Request.Scheme}://{Request.Host}/api/Categories/{createdDto.Id}", createdDto);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update( [FromRoute] int id , [FromBody] CategoryDTORequest cat)
        {
            var (success, updatedCategory, msg) = await _categoryService.UpdateCategoryAsync(id, cat.Adapt<Category>());
            if (!success || updatedCategory is null)
                return BadRequest(new { msg });

            var createdDto = updatedCategory.Adapt<CategoryDTOResponse>();
            return Ok(new { msg });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete( [FromRoute] int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (result)
            {
                return Ok( new { msg = "your Category has been deleted successfully" });
            }
            return BadRequest(   new { msg = " Failed to Delete Category" });
        }
    }
}
