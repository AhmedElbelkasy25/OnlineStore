using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Request
{
    public class ExternalLoginDTO
    {
        public string Provider { get; set; } =string.Empty;
        public string IdToken { get; set; }  = string.Empty;
    }

    public class ExternalLoginResultDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
