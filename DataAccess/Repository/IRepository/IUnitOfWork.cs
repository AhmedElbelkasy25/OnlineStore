using OnlineStore.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository ProductRepository { get; }
        IBrandRepository BrandRepository { get; }
        ICartRepository CartRepository { get; }
        ICategoryRepository CategoryRepository {get; }
        IOrderItemRepository OrderItemRepository {get; }
        IOrderRepository OrderRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRepository UserRepository {get; }
        Task CommitAsync();
            

    }
}
