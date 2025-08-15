using Microsoft.EntityFrameworkCore;

namespace Models
{
    [PrimaryKey(nameof(OrderId), nameof(ProductId))]
    public class OrderItems
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public decimal Price { get; set; }
        public int Count { get; set; }
    }

}
