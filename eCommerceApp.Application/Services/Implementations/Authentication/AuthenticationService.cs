using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.Services.Interfaces.Authentication;
using eCommerceApp.Application.Services.Interfaces.Logging;
using eCommerceApp.Application.Validations;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using FluentValidation;

namespace eCommerceApp.Application.Services.Implementations.Authentication;

public class AuthenticationService
    (
    IValidator<CreateUser> createUserValidator,IValidator<LoginUser> loginUserValidator,
    IMapper mapper, IAppLogger<AuthenticationService> logger,
    IValidationService validationService,
    ITokenManagement tokenManagement, IUserManagement userManagement, IRoleManagement roleManagement
    ) : IAuthenticationService
{
    public async Task<ServiceResponse> CreateUser(CreateUser user, string rawPassword)
    {
        var _validationResult = await validationService.ValidateAsync(user, createUserValidator);
        if (!_validationResult.Success) return _validationResult;

        var mappedModel = mapper.Map<AppUser>(user);
        mappedModel.UserName = user.Email;

        var result = await userManagement.CreateUser(mappedModel, rawPassword);
        if (!result)
            return new ServiceResponse { Message = "Email Address Might be Already in Use Or Unknown Error Occurred" };

        var _user = await userManagement.GetUserByEmail(user.Email);
        var users = await userManagement.GetAllUsers();
        bool assignedResult = await roleManagement.AddUserToRole(_user!, users!.Count() > 1 ? "User" : "Admin");

        if (!assignedResult)
        {
            // Remove user
            int removeUserResult = await userManagement.RemoveUserByEmail(_user!.Email!);
            if (removeUserResult <= 0)
            {
                logger.LogError(
                    new Exception($"User with Email as {_user.Email} failed to be removed as a result of role assignment issue."),
                    "User could not be assigned"
                );

                return new ServiceResponse { Message = "Error Occurred When Creating Account" };
            }
        }

        return new ServiceResponse { Success = true, Message = "Account Created!" };
    }
    public async Task<LoginResponse> LoginUser(string email, string rawPassword)
    {
        var userDto = new LoginUser { Email = email, Password = rawPassword };
        var _validationResult = await validationService.ValidateAsync(userDto, loginUserValidator);
        if (!_validationResult.Success)
            return new LoginResponse(Message: _validationResult.Message);

        var _user = await userManagement.GetUserByEmail(email);
        if (_user is null)
            return new LoginResponse(Message: "Email not found");

        bool loginResult = await userManagement.LoginUser(_user, rawPassword);
        if (!loginResult)
            return new LoginResponse(Message: "Invalid credentials");

        var claims = await userManagement.GetUserClaims(_user.Email!);

        string jwtToken = tokenManagement.GenerateToken(claims);
        string refreshToken = tokenManagement.GetRefreshToken();

        int saveTokenResult = await tokenManagement.AddRefreshToken(_user.Id, refreshToken);
        return saveTokenResult <= 0
            ? new LoginResponse(Message: "Internal Error Occurred While Authenticating")
            : new LoginResponse(Success: true, Token: jwtToken, RefreshToken: refreshToken);
    }

    public async Task<LoginResponse> ReviveToken(string refreshToken)
    {
        bool validateTokenResult = await tokenManagement.ValidateRefreshToken(refreshToken);
        if (!validateTokenResult)
            return new LoginResponse(Message: "Invalid Token");

        string userId = await tokenManagement.GetUserIdByRefreshToken(refreshToken);
        AppUser? user = await userManagement.GetUserById(userId);
        var claims = await userManagement.GetUserClaims(user!.Email!);

        string newJwtToken = tokenManagement.GenerateToken(claims);
        string newRefreshToken = tokenManagement.GetRefreshToken();

        await tokenManagement.UpdateRefreshToken(userId, newRefreshToken);

        return new LoginResponse(Success: true, Token: newJwtToken, RefreshToken: newRefreshToken);
    }
}
