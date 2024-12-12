using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;

namespace eCommerceApp.Application.Services.Interfaces.Authentication;

public interface IAuthenticationService
{
    Task<ServiceResponse> CreateUser(CreateUser user, string rawPassword); 
    Task<LoginResponse> LoginUser(string email, string rawPassword); 
    Task<LoginResponse> ReviveToken(string refreshToken);
}
