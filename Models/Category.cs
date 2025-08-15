using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        //[Length(3, 100)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; }

        public ICollection<Product> Products { get; } = [];
    }

}
