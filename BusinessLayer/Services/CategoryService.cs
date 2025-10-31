using Microsoft.Extensions.Logging;
using Models.DTOs.Category.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger) : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CategoryService> logger = logger;

        public async Task<(bool,List<CategoryDTOResponse>,string?)> GetAllCategoriesAsync()
        {
            try
            {
                var Categories = await _unitOfWork.CategoryRepository.GetAsync();
                if (!Categories.Any())
                {
                    return (true, new List<CategoryDTOResponse>(),null);
                }

                var catDto = Categories.Select(e => new CategoryDTOResponse()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Status = e.Status,
                }).ToList();
                return (true, catDto,null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "there is error habbened when tring to get Categories");
                return (false, new List<CategoryDTOResponse>(),ex.Message);
            }
        }



    }
}
