using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Category.Response;
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
        public async Task<IActionResult> GetAllasync( [FromQuery] int? page=1, [FromQuery] int? items =1)
        {
            var (success, prds, pagination , msg) = await productService.GetAllAsync(page, items);
            if (success)
            {
                if (prds != null)
                {
                    var newprds = prds.Adapt<List<ProductResponseDto>>();
                    pagination.Products = newprds;
                    return Ok(new {AllProducts = pagination });
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
        
        [HttpGet("{id}")]
        public  IActionResult GetProductByIdAsync([FromRoute] int id)
        {
            var (success, product, msg) = productService.GetProductByIdAsync(id);

            if (!success || product is null)
                return BadRequest(new { msg });

            var productDto = product.Adapt<ProductResponseDto>();
            productDto.Category = product.Category.Name;
            productDto.Brand = product.Brand.Name;
            return Ok(new { Product = productDto });
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var (success , msg) = await productService.DeleteProductAsync(id);
            if (success)
            {
                return Ok(new {msg =msg});
            }
            return BadRequest(new {msg = msg});
        }

    }
}
