using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.IServices
{
    public interface IAccountService
    {
        Task<(bool, string, List<string>)> Register(RegisterReqDTO reg);
    }
}
