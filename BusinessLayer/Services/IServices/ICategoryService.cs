using Models.DTOs.Category.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.IServices
{
    public interface ICategoryService
    {
        Task<(bool, List<Category>, string?)> GetAllCategoriesAsync();
        (bool, Category?, string?) GetCategoryById(int id);
        Task<(bool, Category?, string?)> CreateCategoryAsync(Category category);
        Task<(bool, Category?, string?)> UpdateCategoryAsync(int id, Category category);
        Task<bool> DeleteAsync(int id);
    }
}
