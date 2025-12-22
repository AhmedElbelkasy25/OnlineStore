using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Product
{
    public class ProductRequestDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string? Description { get; set; }
        //public string MainImg { get; set; } = string.Empty;
        [Required]
        public IFormFile Image { get; set; } 
        [Required]
        public double Price { get; set; }
        [Required]
        public int Quantity { get; set; }

        public double Discount { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public bool Status { get; set; }
    }



    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string MainImg { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int Traffic { get; set; }
        public int Rate { get; set; }
        public double Discount { get; set; }
        public string Category { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public bool Status { get; set; }
    }


    public class ProductUpdateRequestDto
    {
        public int Id { get; set; } 
        [Required]
        [MinLength(3)]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string? Description { get; set; }
        //public string MainImg { get; set; } = string.Empty;
        
        public IFormFile? Image { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int Quantity { get; set; }

        public double Discount { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public bool Status { get; set; }
    }
}
