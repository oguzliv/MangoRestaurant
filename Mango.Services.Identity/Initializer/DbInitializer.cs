using IdentityModel;
using Mango.Services.Identity.DbConstexts;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Mango.Services.Identity.Initializer
{
    public class DbInitializer: IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if(_roleManager.FindByNameAsync(SD.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();

            }
            else { return; }

            ApplicationUser adminUser = new ApplicationUser()
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "1111111",
                FirstName = "first",
                LastName = "last"
            };

            _userManager.CreateAsync(adminUser,"Oo413413*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();

            var temp1 = _userManager.AddClaimsAsync(adminUser, new Claim[]
            {
                new Claim(JwtClaimTypes.Name,adminUser.FirstName + " " + adminUser.LastName),
                new Claim(JwtClaimTypes.GivenName,adminUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName,adminUser.LastName),
                new Claim(JwtClaimTypes.Role,SD.Admin)

            }).Result;

            ApplicationUser customUser = new ApplicationUser()
            {
                UserName = "custom@gmail.com",
                Email = "custom@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "1111111",
                FirstName = "secfirst",
                LastName = "seclast"
            };

            _userManager.CreateAsync(customUser, "Oo123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(customUser, SD.Customer).GetAwaiter().GetResult();

            var temp2 = _userManager.AddClaimsAsync(customUser, new Claim[]
            {
                new Claim(JwtClaimTypes.Name,customUser.FirstName + " " + customUser.LastName),
                new Claim(JwtClaimTypes.GivenName,customUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName,customUser.LastName),
                new Claim(JwtClaimTypes.Role,SD.Customer)

            }).Result;
        }
    }
}
