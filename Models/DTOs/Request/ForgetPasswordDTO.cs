using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Request
{
    public class ForgetPasswordDTO
    {
        [Required]
        [Display(Name ="Email Or UserName")]
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}
