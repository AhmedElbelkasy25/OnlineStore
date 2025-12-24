using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.User
{
    public class ChangeProfilePicDto
    {
        public string UserId { get; set;} = string.Empty;
        public IFormFile Pic { get; set;}
    }
}
