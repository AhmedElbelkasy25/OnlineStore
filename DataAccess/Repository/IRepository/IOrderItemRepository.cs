

namespace OnlineStore.Repository.IRepository
{
    public interface IOrderItemRepository : IRepository<OrderItems>
    {
        Task<bool> CreateRangeAsync(IEnumerable<OrderItems> entities);
    }
}
