using BusinessLayer.Services;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Brand;


namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController (IBrandService brandService) : ControllerBase
    {
        private readonly IBrandService _brandService = brandService;

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var (success, brandes, msg) = await _brandService.GetAllBrandesAsync();

            if (!success)
                return BadRequest(new { msg });


            var brandDtos = brandes.Select(e => e.Adapt<BrandDTOResponse>()).ToList();

            return Ok(new { brandes = brandDtos });
        }

        [HttpGet("{id}")]
        public IActionResult GetBrandById([FromRoute] int id)
        {
            var (success, brand, msg) = _brandService.GetBrandById(id);

            if (!success || brand is null)
                return BadRequest(new { msg });

            var brandDto = brand.Adapt<BrandDTOResponse>();
            return Ok(brandDto);
        }

        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] BrandDTORequest catDto)
        {

            var brand = catDto.Adapt<Brand>();

            var (success, createdBrand, msg) = await _brandService.CreateBrandAsync(brand);

            if (!success || createdBrand is null)
                return BadRequest(new { msg });

            var createdDto = createdBrand.Adapt<BrandDTOResponse>();
            return Created($"{Request.Scheme}://{Request.Host}/api/Brandes/{createdDto.Id}", createdDto);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BrandDTORequest cat)
        {
            var (success, updatedBrand, msg) = await _brandService.UpdateBrandAsync(id, cat.Adapt<Brand>());
            if (!success || updatedBrand is null)
                return BadRequest(new { msg });

            var createdDto = updatedBrand.Adapt<BrandDTOResponse>();
            return Ok(new { msg });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _brandService.DeleteAsync(id);
            if (result)
            {
                return Ok(new { msg = "your Brand has been deleted successfully" });
            }
            return BadRequest(new { msg = " Failed to Delete Brand" });
        }
    }
}
