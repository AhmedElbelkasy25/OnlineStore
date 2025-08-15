

namespace OnlineStore.Repository.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<bool> DeleteRangeAsync(IEnumerable<Cart> entities);
    }
}
