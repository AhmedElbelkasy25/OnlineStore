using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
     public class BrandService(IUnitOfWork unitOfWork, ILogger<BrandService> logger) : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<BrandService> _logger = logger;

        public async Task<(bool, List<Brand>, string?)> GetAllBrandesAsync()
        {
            try
            {
                var brandes = await _unitOfWork.BrandRepository.GetAsync();

                if (!brandes.Any())
                    return (true, new List<Brand>(), null);

                return (true, brandes.ToList(), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving brandes.");
                return (false, new List<Brand>(), ex.Message);
            }
        }

        public (bool, Brand?, string?) GetBrandById(int id)
        {
            if (id <= 0)
                return (false, null, "Invalid brand ID");

            var brand = _unitOfWork.BrandRepository.GetOne(e => e.Id == id);

            if (brand is null)
                return (false, null, "Brand not found");

            return (true, brand, null);
        }

        public async Task<(bool, Brand?, string?)> CreateBrandAsync(Brand brand)
        {
            var succeeded = await _unitOfWork.BrandRepository.CreateAsync(brand);

            if (succeeded)
                return (true, brand, "Brand added successfully");

            return (false, null, "Failed to add the new brand");
        }

        public async Task<(bool, Brand?, string?)> UpdateBrandAsync(int id ,Brand brand)
        {
            var brandDB = _unitOfWork.BrandRepository.GetOne(e => e.Id == id , tracked:false);
            if (brandDB is null)
            {
                return (false, null, "Not Found");
            }
            brandDB.Name= brand.Name;
            brandDB.Description= brand.Description;
            brandDB.Status= brand.Status;
            var succeeded = await _unitOfWork.BrandRepository.EditAsync(brandDB);
            if (succeeded)
                return (true, brand, "Brand updated successfully");

            return (false, null, "Failed to update the brand");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var (found, brand, msg) = GetBrandById(id);
            if (!found || brand is null)
                return false;
            var result = await _unitOfWork.BrandRepository.DeleteAsync(brand);
            return result;
        }

       
    }
}
