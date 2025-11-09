using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Product;
using System.Threading.Tasks;

namespace OnlineStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        private readonly IProductService productService = productService;
        [HttpGet()]
        public async Task<IActionResult> GetAllasync( [FromQuery] int? page, [FromQuery] int? items)
        {
            var (success, prds, msg) = await productService.GetAllAsync(page, items);
            if (success)
            {
                if (prds != null)
                {
                    var newprds = prds.Adapt<List<ProductResponseDto>>();
                    return Ok(new {Products = newprds });
                }
                else
                {
                    return Ok(new { msg = msg });
                }
            }
            else
            {
                return BadRequest();
            }

        }
        [HttpPost()]
        public async Task<IActionResult> CreateAsync([FromForm] ProductRequestDto prdDto)
        {
            var product = prdDto.Adapt<Product>();
            var (success, prd, msg) = await productService.CreateAsync(product, prdDto.Image);
            if (success)
            {
                return Ok(new { msg = msg });
            }
            return BadRequest(new {msg = msg});
        }
        [HttpPut()]
        public async Task<IActionResult> EditAsync([FromForm] ProductUpdateRequestDto prdDto)
        {
            var product = prdDto.Adapt<Product>();
            var (success, prd, msg) = await productService.UpdateAsync(product, prdDto.Image);
            if (success)
            {
                return Ok(new { msg = msg });
            }
            return BadRequest(new { msg = msg });
        }

    }
}
