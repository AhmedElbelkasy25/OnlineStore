using OnlineStore.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public IProductRepository ProductRepository { get; }
        public IBrandRepository BrandRepository { get; }
        public ICartRepository CartRepository { get; }
        public ICategoryRepository CategoryRepository {get; }
        public IOrderItemRepository OrderItemRepository {get; }
        public IOrderRepository OrderRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IUserRepository UserRepository {get; }
            

    }
}
