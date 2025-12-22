using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task AddRangeAsync(List<Product> products)
        {
            await context.Products.AddRangeAsync(products);
        }
        public async Task<int> CountAsync()
        {
            var count = await context.Products.CountAsync();
            return count;
        }
    }
}
