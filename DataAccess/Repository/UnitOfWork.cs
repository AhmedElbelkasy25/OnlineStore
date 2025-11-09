using DataAccess.Repository.IRepository;
using OnlineStore.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IBrandRepository brandRepository, ICartRepository cartRepository,
            ICategoryRepository categoryRepository, IOrderItemRepository orderItemRepository,
            IOrderRepository orderRepository, IRoleRepository roleRepository,
            IProductRepository productRepository , IUserRepository userRepository)
        {
            ProductRepository = productRepository;
            BrandRepository = brandRepository;
            CartRepository = cartRepository;
            CategoryRepository = categoryRepository;
            OrderItemRepository = orderItemRepository;
            OrderRepository = orderRepository;
            RoleRepository = roleRepository;
            UserRepository = userRepository;
        }

        public IProductRepository ProductRepository { get; private set; }
        public IBrandRepository BrandRepository { get; private set; }

        public ICartRepository CartRepository { get; private set; }

        public ICategoryRepository CategoryRepository { get; private set; }

        public IOrderItemRepository OrderItemRepository { get; private set; }

        public IOrderRepository OrderRepository { get; private set; }

        public IRoleRepository RoleRepository { get; private set; }

        public IUserRepository UserRepository { get; private set; }
    }
}
