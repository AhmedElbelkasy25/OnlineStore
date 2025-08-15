namespace Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        public OrderStatus OrderStatus { get; set; }
        public bool TransactionStatus { get; set; }

        public DateTime ShippedDate { get; set; }
        public string? CarrierName { get; set; }
        public string? CarrierId { get; set; }

        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public string? SessionId { get; set; }
        public string? TransctionId { get; set; }
    }

}
