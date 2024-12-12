using eCommerceApp.Domain.Entities.Identity;
using System.Security.Claims;

namespace eCommerceApp.Domain.Interfaces.Authentication;

public interface IUserManagement
{
    Task<bool> CreateUser(AppUser user, string rawPassword);
    Task<bool> LoginUser(AppUser user, string rawPassword);
    Task<AppUser?> GetUserByEmail(string email);
    Task<AppUser> GetUserById(string id);
    Task<List<AppUser>> GetAllUsers();
    Task<int> RemoveUserByEmail(string email);
    Task<List<Claim>> GetUserClaims(string email);
}
