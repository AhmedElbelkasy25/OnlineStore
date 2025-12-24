
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models.DTOs.Product;
using System.Threading.Tasks;


namespace BusinessLayer.Services
{
    public class ProductService(IUnitOfWork unitOfWork, IWebHostEnvironment env, IOptions<FileSettings> fileSettings) : IProductService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IWebHostEnvironment env = env;
        private readonly IOptions<FileSettings> fileSettings = fileSettings;

        public async Task<(bool, List<Product>?, ProductPaginationDto?, string?)> GetAllAsync(int? page = 1, int? items = 20)
        {
            if (page < 1) page = 1;
            if (items < 1 || items > 100) items = 20;

            var totalItems = await _unitOfWork.ProductRepository.CountAsync();

            var skip = (page - 1) * items;
            var take = items;
            var products = await _unitOfWork.ProductRepository.GetAsync(include: query => query.Include(e => e.Category), orderBy: query => query.OrderBy(e => e.Id), tracked: false, skip: skip, take: take);

            if (!products.Any())
            {
                return (true, null, null ,"there is no Product");
            }
            var dto = new ProductPaginationDto() { CurrentPage = page ?? 1, PageSize = items ?? 20,
                TotalItems = totalItems  , TotalPages = (int)Math.Ceiling(totalItems / (double)items!)
            };
            return (true, products.ToList(), dto,null);

        }

        public (bool, Product?, string?) GetProductByIdAsync(int id)
        {
            var product = _unitOfWork.ProductRepository.GetOne( e=>e.Id==id , include:e=>e.Include(e=>e.Brand).Include(e=>e.Category) );
            if (product == null)
            {
                return (false, null, "Not Found");
            }
            return (true, product, null);
        }


        private async Task<(bool success, string? fileName, string? message)> SaveImageAsync(IFormFile image)
        {
            try
            {

                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

                //if (image.Length > 5 * 1024 * 1024)
                //{
                //    return (false, null, "File size must be less than 5MB");
                //}

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var imagesFolder = Path.Combine(env.ContentRootPath, "..", fileSettings.Value.ImagesFolder);

                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                var filePath = Path.Combine(imagesFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                return (true, fileName, null);

            }
            catch (Exception ex)
            {
                return (false, null, $"Error saving image: {ex.Message}");
            }
        }


        private bool DeleteImage(string fileName)
        {
            try
            {
                var filePath = Path.Combine(env.ContentRootPath, "..", fileSettings.Value.ImagesFolder, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        public async Task<(bool, Product?, string?)> CreateAsync(Product? prd, IFormFile img)
        {
            if (prd is null)
            {
                return (false, null, "the product is null");
            }
            if (img != null && img.Length > 0)
            {
                var (success, Image, msg) = await SaveImageAsync(img);
                if (!success)
                {
                    return (false, null, msg);
                }

                prd.MainImg = Image ?? "";

            }
            else
            {
                return (false, null, "Product image is required");
            }

            var created = await _unitOfWork.ProductRepository.CreateAsync(prd);
            if (!created)
            {
                DeleteImage(prd.MainImg);
                return (false, null, "Failed To Create Product");
            }

            return (true, prd, "Product created successfully");

        }


        public async Task<(bool, Product?, string?)> UpdateAsync(Product prd, IFormFile? img)
        {
            if (prd is null)
            {
                return (false, null, "the product is null");
            }
            var oldPrd = _unitOfWork.ProductRepository.GetOne(e => e.Id == prd.Id, tracked: false);
            if (oldPrd is null)
            {
                return (false, null, "this product is not exist");
            }
            string? newImageFileName = null;
            string? oldImageFileName = null;
            if (img != null && img.Length > 0)
            {
                //DeleteImage(oldPrd.MainImg);
                var (success, Image, msg) = await SaveImageAsync(img);
                if (!success)
                {
                    return (false, null, msg);
                }

                prd.MainImg = Image ?? "";
                newImageFileName = Image;
                oldImageFileName = oldPrd.MainImg;

            }
            else
            {
                prd.MainImg = oldPrd.MainImg;
            }

            var updated = await _unitOfWork.ProductRepository.EditAsync(prd);
            if (updated)
            {
                if (oldImageFileName is not null)
                {
                    DeleteImage(oldImageFileName);
                }
                return (true, prd, "Product updated successfully");

            }
            if (newImageFileName is not null)
            {
                DeleteImage(newImageFileName);

            }
            return (false, prd, "Product didn't updated");
        }

        public async Task<(bool success, string msg)> DeleteProductAsync(int id)
        {
            var product = _unitOfWork.ProductRepository.GetOne(e => e.Id == id);
            if (product == null)
            {
                return (false,"Not Found");
            }
            var img = product.MainImg;
            var deleted = await _unitOfWork.ProductRepository.DeleteAsync(product);

            if (!deleted)
            {
                return (false, "Sorry... Can't Delete this Product");
            }
            DeleteImage(img);
            return (true, "the Product has been deleted successfully");
        }

    }
}




