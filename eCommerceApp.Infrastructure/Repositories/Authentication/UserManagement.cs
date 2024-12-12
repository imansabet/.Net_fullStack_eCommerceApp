using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using eCommerceApp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace eCommerceApp.Infrastructure.Repositories.Authentication
{
    public class UserManagement : IUserManagement
    {
        private readonly IRoleManagement _roleManagement;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ILogger<UserManagement> _logger;

        public UserManagement(
            IRoleManagement roleManagement,
            UserManager<AppUser> userManager,
            AppDbContext context,
            ILogger<UserManagement> logger
        )
        {
            _roleManagement = roleManagement;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CreateUser(AppUser user, string rawPassword)
        {
            var _user = await GetUserByEmail(user.Email!);
            if (_user != null) return false;

            var result = await _userManager.CreateAsync(user, rawPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"Error creating user: {error.Description}");
                }
                return false;
            }

            return true;
        }

        public async Task<List<AppUser>> GetAllUsers() => await _context.Users.ToListAsync();

        public async Task<AppUser?> GetUserByEmail(string email) => await _userManager.FindByEmailAsync(email);

        public async Task<AppUser> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user!;
        }

        public async Task<List<Claim>> GetUserClaims(string email)
        {
            var _user = await GetUserByEmail(email);
            string? roleName = await _roleManagement.GetUserRole(_user!.Email!);

            return new List<Claim>
            {
                new Claim("FullName", _user!.FullName),
                new Claim(ClaimTypes.NameIdentifier, _user!.Id),
                new Claim(ClaimTypes.Email, _user!.Email!),
                new Claim(ClaimTypes.Role, roleName!),
            };
        }

        public async Task<bool> LoginUser(AppUser user, string rawPassword)
        {
            var _user = await GetUserByEmail(user.Email!);
            if (_user is null) return false;

            string? roleName = await _roleManagement.GetUserRole(_user.Email!);
            if (string.IsNullOrEmpty(roleName)) return false;

            return await _userManager.CheckPasswordAsync(_user, rawPassword);
        }

        public async Task<int> RemoveUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(_ => _.Email == email);
            if (user is null) return 0;

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync();
        }
    }
}
