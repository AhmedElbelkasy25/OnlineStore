using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Brand
{
    public class BrandDTORequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]

        public string Name { get; set; } = string.Empty;
        [Required]
        public string? Description { get; set; }
        public bool Status { get; set; }
    }


    public class BrandDTOResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        public bool Status { get; set; }

    }
}
