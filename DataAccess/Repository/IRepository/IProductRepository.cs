using OnlineStore.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task AddRangeAsync(List<Product> products);
    }
}
