using Microsoft.AspNetCore.Http;
using Models.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.IServices
{
    public interface IProductService
    {
        Task<(bool, List<Product>?, ProductPaginationDto, string?)> GetAllAsync(int? page = 1, int? items = 20);
        Task<(bool, Product?, string?)> CreateAsync(Product? prd, IFormFile img);
        Task<(bool, Product?, string?)> UpdateAsync(Product prd, IFormFile? img);
        (bool, Product?, string?) GetProductByIdAsync(int id);
        Task<(bool success, string msg)> DeleteProductAsync(int id);
    }
}
