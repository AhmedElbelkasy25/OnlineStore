

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace OnlineStore.Utility.DBInitilizer
{
    public class DBInitilizer : IDBInitilizer
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DBInitilizer(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initilize()
        {
            try
            {

                if(_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }

                if (!_roleManager.Roles.Any())
                {
                    _roleManager.CreateAsync(new(SD.SuperAdmin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Company)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Customer)).GetAwaiter().GetResult();
                }

                _userManager.CreateAsync(new()
                {
                    Email = "Admin@eraasoft.com",
                    UserName = "Admin",
                }, "Admin123$").GetAwaiter().GetResult();

                var user = _userManager.FindByEmailAsync("Admin@eraasoft.com").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, SD.SuperAdmin).GetAwaiter().GetResult();
            } 
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
            
    }
}
