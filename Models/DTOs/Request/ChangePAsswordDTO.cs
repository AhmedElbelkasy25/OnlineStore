using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Request
{
    public class ChangePAsswordDTO
    {
        [Required]
        [MinLength(8)]
        [Display(Name ="Current Password")]
        public string OldPass { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        [Display(Name = "new Password")]
        public string NewPassword { get; set; } = string.Empty ;
        [Required]
        [MinLength(8)]
        [Display(Name = "Old Password")]
        [Compare(nameof(NewPassword))]
        public string ConfirmPass { get; set; } =string.Empty ;
    }
}
