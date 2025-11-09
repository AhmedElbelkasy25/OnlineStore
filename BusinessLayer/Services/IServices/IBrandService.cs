using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.IServices
{
    public interface IBrandService
    {
        Task<(bool, List<Brand>, string?)> GetAllBrandesAsync();
        (bool, Brand?, string?) GetBrandById(int id);
        Task<(bool, Brand?, string?)> CreateBrandAsync(Brand brand);
        Task<(bool, Brand?, string?)> UpdateBrandAsync(int id, Brand brand);
        Task<bool> DeleteAsync(int id);
    }
}
