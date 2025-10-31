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
        Task<(bool, List<CategoryDTOResponse>, string?)> GetAllCategoriesAsync();
    }
}
