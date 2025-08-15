using OnlineStore.Repository.IRepository;
using Microsoft.AspNetCore.Identity;

namespace OnlineStore.Repository
{
    public class RoleRepository : Repository<IdentityRole>, IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
