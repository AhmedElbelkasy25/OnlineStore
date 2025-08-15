using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string MainImg { get; set; } = string.Empty;
        [Range(1, 500_000)]
        public decimal Price { get; set; }
        [Range(1, 1000)]
        public int Quantity { get; set; }
        [Range(0, 5)]
        public double Rate { get; set; }
        public bool Status { get; set; }
        [Range(1, 100)]
        public decimal Discount { get; set; }
        public int Traffic { get; set; }
        public int CategoryId { get; set; }
        
        public Category Category { get; set; } = null!;
        public int BrandId { get; set; }
        
        public Brand Brand { get; set; } = null!;
    }

}
