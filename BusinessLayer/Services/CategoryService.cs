using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger) : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CategoryService> _logger = logger;

        public async Task<(bool, List<Category>, string?)> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository.GetAsync();

                if (!categories.Any())
                    return (true, new List<Category>(), null);

                return (true, categories.ToList(), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving categories.");
                return (false, new List<Category>(), ex.Message);
            }
        }

        public (bool, Category?, string?) GetCategoryById(int id)
        {
            if (id <= 0)
                return (false, null, "Invalid category ID");

            var category = _unitOfWork.CategoryRepository.GetOne(e => e.Id == id);

            if (category is null)
                return (false, null, "Category not found");

            return (true, category, null);
        }

        public async Task<(bool, Category?, string?)> CreateCategoryAsync(Category category)
        {
            var succeeded = await _unitOfWork.CategoryRepository.CreateAsync(category);

            if (succeeded)
                return (true, category, "Category added successfully");

            return (false, null, "Failed to add the new category");
        }

        public async Task<(bool, Category?, string?)> UpdateCategoryAsync(int id ,Category category)
        {
            var categoryDB = _unitOfWork.CategoryRepository.GetOne(e => e.Id == id , tracked:false);
            if (categoryDB is null)
            {
                return (false, null, "Not Found");
            }
            categoryDB.Name= category.Name;
            categoryDB.Description= category.Description;
            categoryDB.Status= category.Status;
            var succeeded = await _unitOfWork.CategoryRepository.EditAsync(categoryDB);
            if (succeeded)
                return (true, category, "Category updated successfully");

            return (false, null, "Failed to update the category");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var (found, category, msg) = GetCategoryById(id);
            if (!found || category is null)
                return false;
            var result = await _unitOfWork.CategoryRepository.DeleteAsync(category);
            return result;
        }

       
    }
}
